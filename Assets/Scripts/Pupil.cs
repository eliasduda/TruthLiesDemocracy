using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Rendering.Universal;

public class Pupil : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public PupilManager manager;
    private Vector2 velocity;
    private float colliderRadius;

    public Sprite unawareSprite;
    public Sprite awareSprite;
    public Sprite supportingSprite;
    public Sprite youSprite;
    private SpriteRenderer spriteRenderer;

    public GameObject armLeft;
    public GameObject armRight;
    public float swingAmplitude = 40f;  // max angle (�40)
    public float swingSpeed = 3f;

    public PupilStats stats = new PupilStats();
    public bool isOccupied = false;
    public bool displayRadius = false;
    public Sprite circleSprite;
    public Color radiusColor = new Color(1f, 1f, 1f, 0.10f);
    private GameObject radiusObject;

    private bool updatedSupportSinceLastVote;

    private Rigidbody2D rb;
    private Vector2? lastPos = null;

    private bool inDiscussion = false;
    [HideInInspector] public Vector2 pendingVelocity;
    private Coroutine discussCoroutine;

    public event Action<Pupil, List<Pupil>> OnBump;

    [HideInInspector] public Vector2 ringTargetPosition;
    [HideInInspector] public bool moveToRing = false;
    [HideInInspector] public Vector2 ringCenter;

    private bool _isFrozen = false;
    public bool IsFrozen
    {
        get { return _isFrozen; }
        set
        {
            _isFrozen = value;
            if (value) // if freezing, stop all movement
            {
                rb.angularVelocity = 0f;
                rb.linearVelocity = Vector2.zero;
                lastPos = null;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            else rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public HashSet<Pupil> connectedPupils = new HashSet<Pupil>();

    private AudioSource audioSource;

    public bool isYou;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        velocity = UnityEngine.Random.insideUnitCircle.normalized * manager.MaxSpeed;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateSprite();


        // --- Arm positioning ---
        // Get the radius from the CircleCollider2D
        var circle = GetComponent<CircleCollider2D>();
        colliderRadius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        Transform capsuleTransform = armLeft.transform.Find("Capsule");
        var armCollider = armLeft.GetComponent<CapsuleCollider>();
        CapsuleCollider capsuleCol = capsuleTransform.GetComponent<CapsuleCollider>();
        float margin = capsuleCol.radius * capsuleCol.transform.lossyScale.x;

        armLeft.transform.localPosition = new Vector3(-(colliderRadius + margin), 0, 0);
        armRight.transform.localPosition = new Vector3((colliderRadius + margin), 0, 0);

        audioSource = GetComponent<AudioSource>();

        GameManager.instance.eventManager.onPupilStatInfluenced.AddListener(ApplyStatChange);
        GameManager.instance.eventManager.onTimedEventEnded.AddListener(OnRecastVote);
    }

    void FixedUpdate()
    {
        if (_isFrozen) return;

        // --- Move ---
        if (inDiscussion)
        {
            if (moveToRing)
            {
                Vector2 toTarget = ringTargetPosition - rb.position;
                float distance = toTarget.magnitude;

                if (distance > 0.01f)
                {
                    rb.linearVelocity = toTarget.normalized * manager.MaxSpeed;
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
            lastPos = null; // Reset last position tracking while in discussion
        }
        else
        {
            // Detect if stuck against wall
            if (lastPos != null) //only check if they moved normally last frame
            {
                Vector2 intendedMove = velocity * Time.fixedDeltaTime;
                Vector2 actualMove = (Vector2)transform.position - (Vector2)lastPos;

                float intendedMag = intendedMove.magnitude;
                float actualMag = actualMove.magnitude;

                // If actual movement is much smaller, we probably hit or are stuck against a wall
                if (actualMove.magnitude < intendedMove.magnitude * 0.9f)
                {
                    TryBounce();
                }
            }
            lastPos = transform.position;

            // Normal movement
            rb.linearVelocity = velocity;
        }


        // --- Rotate Arms ---
        float rotateAngle = 0f;
        if (!inDiscussion)
        {
            rotateAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        }
        else if (moveToRing)
        {
            rotateAngle = Mathf.Atan2((ringCenter - rb.position).y, (ringCenter - rb.position).x) * Mathf.Rad2Deg;
        }
        rb.MoveRotation(rotateAngle - 90f);
    }

    void TryBounce()
    {
        // Move them back slightly to make them bounce off the wall
        transform.position = (Vector2)transform.position + -velocity.normalized * 0.1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length == 0) return;

        Pupil otherPupil = collision.gameObject.GetComponent<Pupil>();
        Vector2 normal = collision.contacts[0].normal;
        Vector2 bounceVelocity = normal * manager.MaxSpeed;

        if (otherPupil != null)  //collision with another pupil
        {
            if (connectedPupils.Contains(otherPupil))
            {
                return; // ignore collisions with other pupils in the same group
            }

            // Build the full group (including all connected pupils recursively)
            HashSet<Pupil> group = new HashSet<Pupil>();
            CollectGroup(this, group);
            CollectGroup(otherPupil, group);

            List<Pupil> groupList = new List<Pupil>(group);

            // Trigger bump event 
            if (connectedPupils.Count == 0 && otherPupil.connectedPupils.Count == 0)
            {
                if (this.GetInstanceID() < otherPupil.GetInstanceID())
                {
                    OnBump?.Invoke(this, groupList);
                }
            }
            else if (connectedPupils.Count == 0)
            {
                OnBump?.Invoke(this, groupList);
            }

            // Update all group members' connectedPupils sets
            foreach (var pupil in group)
            {
                pupil.connectedPupils = new HashSet<Pupil>(group);
                pupil.connectedPupils.Remove(pupil); // Don't include self
            }

            float discussDuration = manager.ArrangeRing(groupList);

            // Discuss and arrange ring for all
            foreach (var pupil in group)
            {
                pupil.Discuss(discussDuration);
            }

            // Play bump sounds
            manager.PlayBumpSound(audioSource);
        }
        else
        {
            // Wall collision
            bounceVelocity = Vector2.Reflect(velocity, bounceVelocity);
            velocity = bounceVelocity;
            lastPos = null; // Reset last position tracking after bounce
        }
    }

    // Recursively collect all connected pupils
    static void CollectGroup(Pupil pupil, HashSet<Pupil> group)
    {
        if (group.Contains(pupil)) return;
        group.Add(pupil);
        foreach (var p in pupil.connectedPupils)
            CollectGroup(p, group);
    }

    public void Discuss(float duration)
    {
        if (discussCoroutine != null)
            StopCoroutine(discussCoroutine);
        discussCoroutine = StartCoroutine(DiscussCoroutine(duration));
    }

    IEnumerator DiscussCoroutine(float duration)
    {
        inDiscussion = true;
        moveToRing = true;
        yield return new WaitForSeconds(duration);

        inDiscussion = false;
        moveToRing = false;
        velocity = pendingVelocity;
        discussCoroutine = null;
        connectedPupils.Clear();
    }
    void ApplyStatChange(InfluencableStats stat, float amount, Pupil pupil)
    {
        UpdateSprite();
        if (pupil == this && !isYou)
        {
            if (stat == InfluencableStats.Support)
            {
                updatedSupportSinceLastVote = true;
                amount *= GameManager.instance.gamePlaySettings.trustSupportMultiplier.Evaluate(stats.trust) +1;
            }
            stats.ApplyChange(stat, amount);
        }
    }

    void OnRecastVote(EventData eventData)
    {
        if(isYou && eventData.occupiesYou) isOccupied = false;
        if (IsFrozen) IsFrozen = false;
        if (updatedSupportSinceLastVote)
        {
            updatedSupportSinceLastVote = false;
            if (stats.AskToSign())
            {
                GameManager.instance.eventManager.onOneTimeStatInfluenced.Invoke(InfluencableStats.Signatures, 1);
            }
        }
    }

    public bool IsInMyRadius(Pupil other)
    {
        //Debug.Log("Checking if " + other.name + " is in radius of " + name + " dist "+(other.transform.position - transform.position).magnitude + " rad "+ stats.radius);
        return (other.transform.position - transform.position).magnitude < stats.radius;
    }

    void UpdateSprite()
    {
        Sprite shouldHaveSprite;
        if (isYou) shouldHaveSprite = youSprite;
        else if (stats.isAware == 0f) shouldHaveSprite = unawareSprite;
        else if (stats.support < 0.5f) shouldHaveSprite = awareSprite;
        else shouldHaveSprite = supportingSprite;

        if (spriteRenderer.sprite != shouldHaveSprite)
        {
            spriteRenderer.sprite = shouldHaveSprite;
        }
    }

    void Update()
    {
        if (displayRadius && radiusObject == null)
        {
            CreateRadius();
        }
        else if (!displayRadius && radiusObject != null)
        {
            Destroy(radiusObject);
        }

        if (radiusObject != null)
        {
            float diameter = stats.radius * 2f;
            radiusObject.transform.localScale = new Vector3(diameter, diameter, 1f);
        }

        // Swing arms
        if (!inDiscussion)
        {
            float swingAngle = Mathf.Sin(Time.time * swingSpeed) * swingAmplitude;
            armLeft.transform.localRotation = Quaternion.Euler(swingAngle, 0f, 0f);
            armRight.transform.localRotation = Quaternion.Euler(-swingAngle, 0f, 0f);
        }
        else
        {
            armLeft.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            armRight.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void CreateRadius()
    {
        radiusObject = new GameObject("RadiusVisual");
        radiusObject.transform.SetParent(transform);
        radiusObject.transform.localPosition = Vector3.zero;

        var sr = radiusObject.AddComponent<SpriteRenderer>();
        sr.sprite = circleSprite;
        sr.color = radiusColor;
        sr.sortingOrder = 0;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Pupil");
        GameManager.instance.eventManager.onClickedPupil.Invoke(this);
    }
}


[System.Serializable]
public class PupilStats
{
    public float support;
    public float trust;
    public float isAware;

    public float radius;
    public string name;
    public bool hasSigned;

    public PupilStats()
    {
        support = 0f;
        trust = 0f;
        isAware = 0f;
        radius = 0f;
        
    }

    public float GetStat(InfluencableStats stat)
    {
        return stat switch
        {
            InfluencableStats.Support => trust,
            InfluencableStats.Trust => support,
            _ => 0f,
        };
    }

    public void ApplyChange(InfluencableStats stat, float amount)
    {
        switch (stat)
        {
            case InfluencableStats.Trust:
                trust = Mathf.Clamp01(trust + amount);
               // Debug.Log("Pupil's trust changed to " + trust);
                break;
            case InfluencableStats.Support:
                support = Mathf.Clamp01(support + amount);
                //Debug.Log("Pupil's support changed to " + support);
                break;
            case InfluencableStats.Awareness:
                isAware = Mathf.Clamp01(isAware + amount);
                //Debug.Log("Pupil's awareness changed to " + isAware);
                break;
        }
    }

    public bool AskToSign()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        if (r < support && !hasSigned)
        {
            hasSigned = true;
            return true;
        }
        return false;
    }

    public static bool IsPerPupilStat(InfluencableStats stat)
    {
        return stat == InfluencableStats.Support || stat == InfluencableStats.Trust || stat == InfluencableStats.Awareness;
    }

    public void CopyStats(PupilStats stats)
    {
        this.support = stats.support;
        this.trust = stats.trust;
        this.isAware = stats.isAware;
        this.radius = stats.radius;
        this.hasSigned = stats.hasSigned;
    }

    public void SetRandomName()
    {
        name = PupilStats.names[Random.Range(0, PupilStats.names.Length)];
    }

    public static string[] names = {
    "Paul", "Jakob", "Maximilian", "Elias", "Felix", "Leon", "Tobias", "Jonas", "Noah", "Lukas",
    "Alexander", "Moritz", "Leo", "Julian", "Simon", "Matteo", "Fabian", "Valentin", "Raphael", "Emil",
    "Luca", "Samuel", "Anton", "Florian", "David", "Philipp", "Felix", "Lukas", "Noah", "Paul",
    "Maximilian", "Elias", "Jonas", "Leon", "Tobias", "Gunnar", "Maximilian", "Elias", "David", "Felix",
    "Leon", "Tobias", "Jonas", "Noah", "Lukas", "Alexander", "Moritz", "Leo", "Julian", "Simon",
    "Matteo", "Fabian", "Valentin", "Raphael", "Emil", "Luca", "Samuel", "Anton", "Florian", "David",
    "Emilia", "Emma", "Marie", "Anna", "Sophia", "Mia", "Lena", "Valentina", "Laura", "Lea",
    "Hannah", "Lina", "Sophie", "Johanna", "Leonie", "Lina", "Mia", "Valentina", "Anna", "Sophia",
    "Emilia", "Marie", "Emma", "Anna", "Sophia", "Mia", "Lena", "Valentina", "Laura", "Lea",
    "Hannah", "Lina", "Sophie", "Johanna", "Leonie", "Lina", "Mia", "Valentina", "Anna", "Sophia"
    };
}
