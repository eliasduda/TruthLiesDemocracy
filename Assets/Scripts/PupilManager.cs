using System.Collections.Generic;
using UnityEngine;

public class PupilManager : MonoBehaviour
{
    public Pupil pupilPrefab;
    public int pupilCount = 50;

    [SerializeField] private float _maxSpeed = 1f;
    public float MaxSpeed => _maxSpeed;
    public float discussDuration = 1f; // Duration to freeze on collision
    static public float discussGroupSize = 1f; // Extra space for discussion ring

    private Vector2 areaSize; // box width/height

    [HideInInspector] public List<Pupil> pupils = new List<Pupil>();

    void Start()
    {
        // Set areaSize to match the screen size in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        areaSize = new Vector2(
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );
        transform.position = (bottomLeft + topRight) / 2f;

        // Create screen boundary walls
        CreateScreenWalls();

        // Spawn pupils at random positions within the area
        for (int i = 0; i < pupilCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );
            Pupil pupil = Instantiate(pupilPrefab, pos, Quaternion.identity);
            pupil.manager = this;
            pupils.Add(pupil);
        }
    }

    // Helper to get the box bounds in world space
    public Rect GetBounds()
    {
        return new Rect(
            (Vector2)transform.position - areaSize / 2,
            areaSize
        );
    }

    void CreateScreenWalls()
    {
        Rect bounds = GetBounds();
        float thickness = 1f; // Thickness of the wall

        // Bottom
        CreateWall(
            new Vector2(bounds.center.x, bounds.yMin - thickness / 2),
            new Vector2(bounds.width + thickness * 2, thickness)
        );
        // Top
        CreateWall(
            new Vector2(bounds.center.x, bounds.yMax + thickness / 2),
            new Vector2(bounds.width + thickness * 2, thickness)
        );
        // Left
        CreateWall(
            new Vector2(bounds.xMin - thickness / 2, bounds.center.y),
            new Vector2(thickness, bounds.height + thickness * 2)
        );
        // Right
        CreateWall(
            new Vector2(bounds.xMax + thickness / 2, bounds.center.y),
            new Vector2(thickness, bounds.height + thickness * 2)
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

    public static void ArrangeRing(List<Pupil> group)
    {
        // Calculate center
        Vector2 center = Vector2.zero;
        foreach (var pupil in group)
            center += (Vector2)pupil.transform.position;
        center /= group.Count;

        // Choose radius
        float pupilRadius = group[0].GetComponent<CircleCollider2D>().radius * group[0].transform.lossyScale.x;
        float ringRadius = (pupilRadius * group.Count + discussGroupSize) / (2 * Mathf.PI);

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
    }
}
