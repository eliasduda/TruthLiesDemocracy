using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Audio;

public class PupilManager : MonoBehaviour
{
    public Pupil pupilPrefab;
    private int pupilCount = 50;

    [SerializeField] private float _maxSpeed = 1f;
    public float MaxSpeed => _maxSpeed;

    [SerializeField] private float _discussDuration = 1f; // Duration to freeze on collision
    public float DiscussDuration => _discussDuration;
    [SerializeField] private float discussGroupSize = 1f; // Extra space for discussion ring

    public InfluencableStats visualizeStat = InfluencableStats.Support;
    private BoxCollider2D pupilArea;
    Bounds bounds;

    public List<Pupil> pupils = new List<Pupil>();
    public Pupil you;

    public List<AudioClip> bumpSounds;
    private int concurrentBumpSounds = 0;
    [SerializeField] private int maxConcurrentBump = 3; // Max concurrent bump sounds
    public AudioClip discussionSound;
    private AudioSource discussionSource;
    [SerializeField] private int discussionSoundThreshold = 4; // Minimum group size to play discussion sound

    void Start()
    {
        pupilArea = GetComponentInChildren<BoxCollider2D>();
        bounds = pupilArea.bounds;
        pupilCount = GameManager.instance.gamePlaySettings.signatureGoal;

        // Set areaSize to match the screen size in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        transform.position = (bottomLeft + topRight) / 2f;

        // Create screen boundary walls
        CreateScreenWalls();

        bool youSpawned = false;
        // Spawn pupils at random positions within the area
        for (int i = 0; i < pupilCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + new Vector2(
                Random.Range(bounds.min.x , bounds.max.x ),
                Random.Range(bounds.min.y , bounds.max.y )
            );
            Pupil pupil = Instantiate(pupilPrefab, pos, Quaternion.identity);
            pupil.manager = this;
            if (!youSpawned)
            {
                you = pupil;
                pupil.isYou = true;
                pupil.stats.CopyStats(GameManager.instance.gamePlaySettings.startStats);
                youSpawned = true;
            }
            pupils.Add(pupil);
        }

        discussionSource = GetComponent<AudioSource>();
    }

    void CreateScreenWalls()
    {
        float thickness = 1f; // Thickness of the wall

        // Bottom
        CreateWall(
            new Vector2(bounds.center.x, bounds.min.y - thickness / 2),
            new Vector2(bounds.max.x-bounds.min.x, thickness)
        );
        // Top
        CreateWall(
            new Vector2(bounds.center.x, bounds.max.y + thickness / 2),
            new Vector2(bounds.max.x - bounds.min.x, thickness)
        );
        // Left
        CreateWall(
            new Vector2(bounds.min.x - thickness / 2, bounds.center.y),
            new Vector2(thickness, bounds.max.y - bounds.min.y)
        );
        // Right
        CreateWall(
            new Vector2(bounds.max.x + thickness / 2, bounds.center.y),
            new Vector2(thickness, bounds.max.y - bounds.min.y)
        );
    }

    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = position;
        var collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;
    }

    public void PlayBumpSound(AudioSource source)
    {
        if (concurrentBumpSounds >= maxConcurrentBump) return; // Limit concurrent sounds
        int i = Random.Range(0, bumpSounds.Count-1);
        AudioClip audioClip = bumpSounds[i];
        StartCoroutine(PlayBump(source, audioClip));
    }

    IEnumerator PlayBump(AudioSource source, AudioClip clip)
    {
        concurrentBumpSounds++;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.volume = Random.Range(0.5f, 0.7f);
        source.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        concurrentBumpSounds--;
    }

    private void PlayDiscussionSound(Vector2 position)
    {
        if (discussionSource.isPlaying) return;
        discussionSource.transform.position = position;
        discussionSource.pitch = Random.Range(0.9f, 1.1f);
        discussionSource.volume = Random.Range(0.5f, 1f);
        discussionSource.PlayOneShot(discussionSound);
    }

    public float ArrangeRing(List<Pupil> group)
    {
        // Calculate center
        Vector2 center = Vector2.zero;
        foreach (var pupil in group)
            center += (Vector2)pupil.transform.position;
        center /= group.Count;

        // Choose radius
        float pupilRadius = group[0].GetComponent<CircleCollider2D>().radius * group[0].transform.lossyScale.x;
        float ringRadius = (pupilRadius * group.Count + discussGroupSize * (group.Count - 1)) / (2 * Mathf.PI);

        // Generate all possible ring positions
        List<Vector2> ringPositions = new List<Vector2>();
        for (int i = 0; i < group.Count; i++)
        {
            float angle = 2 * Mathf.PI * i / group.Count;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ringRadius;
            ringPositions.Add(center + offset);
        }

        // Assign each pupil to the closest available ring position
        List<Pupil> unassignedPupils = new List<Pupil>(group);
        List<Vector2> unassignedPositions = new List<Vector2>(ringPositions);

        while (unassignedPupils.Count > 0)
        {
            float minDist = float.MaxValue;
            int bestPupilIdx = -1;
            int bestPosIdx = -1;

            // Find the closest pair (pupil, position)
            for (int p = 0; p < unassignedPupils.Count; p++)
            {
                for (int r = 0; r < unassignedPositions.Count; r++)
                {
                    float dist = Vector2.Distance(unassignedPupils[p].transform.position, unassignedPositions[r]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestPupilIdx = p;
                        bestPosIdx = r;
                    }
                }
            }

            // Assign
            Pupil pupil = unassignedPupils[bestPupilIdx];
            Vector2 pos = unassignedPositions[bestPosIdx];
            pupil.ringTargetPosition = pos;
            pupil.ringCenter = center;

            // Set velocity outward from center (optional, for bounce effect)
            Vector2 outOfCircle = pupil.transform.position - (Vector3)center;
            pupil.pendingVelocity = outOfCircle.normalized * pupil.manager.MaxSpeed;

            pupil.moveToRing = true;

            // Remove assigned pupil and position
            unassignedPupils.RemoveAt(bestPupilIdx);
            unassignedPositions.RemoveAt(bestPosIdx);
        }

        if (group.Count >= discussionSoundThreshold) {
            PlayDiscussionSound(center);
        }

        float duration = _discussDuration;
        if (group.Count > 2)
            duration /= 2; // Don't freeze too long for large groups
        return duration;
    }
}
