using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    private Vector2 velocity;
    private float colliderRadius;

    public Sprite unawareSprite;
    public Sprite awareSprite;
    public Sprite supportingSprite;
    private SpriteRenderer spriteRenderer;

    public GameObject armLeft;
    public GameObject armRight;
    public float swingAmplitude = 40f;  // max angle (±40)
    public float swingSpeed = 3f;

    public PupilStats stats = new PupilStats();

    private Rigidbody2D rb;
    private Vector2? lastPos = null;

    private bool inDiscussion = false;
    [HideInInspector] public Vector2 pendingVelocity;
    private Coroutine discussCoroutine;

    [HideInInspector] public Vector2 ringTargetPosition;
    [HideInInspector] public bool moveToRing = false;
    [HideInInspector] public Vector2 ringCenter;

    public HashSet<Pupil> connectedPupils = new HashSet<Pupil>();

    private AudioSource audioSource;

    public bool isYou;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        velocity = Random.insideUnitCircle.normalized * manager.MaxSpeed;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        stats.CopyStats(GameManager.instance.gamePlaySettings.startStats);
    }

    void FixedUpdate()
    {
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

            // Update all group members' connectedPupils sets
            foreach (var pupil in group)
            {
                pupil.connectedPupils = new HashSet<Pupil>(group);
                pupil.connectedPupils.Remove(pupil); // Don't include self
            }

            // Discuss and arrange ring for all
            foreach (var pupil in group)
            {
                pupil.Discuss();
            }
            manager.ArrangeRing(new List<Pupil>(group));

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

    public void Discuss()
    {
        if (discussCoroutine != null)
            StopCoroutine(discussCoroutine);
        discussCoroutine = StartCoroutine(DiscussCoroutine(manager.DiscussDuration));
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
        if(pupil == this && !isYou) stats.ApplyChange(stat, amount);
    }

    public bool IsInMyRadius(Pupil other)
    {
        return (other.transform.position - transform.position).magnitude < stats.radius;
    }

    void UpdateSprite()
    {
        if (stats.isAware == 0f) spriteRenderer.sprite = unawareSprite;
        else if (stats.support < 0.5f) spriteRenderer.sprite = awareSprite;
        else spriteRenderer.sprite = supportingSprite;

    }
}

public class PupilStats
{
    public float support;
    public float trust;
    public float isAware;

    public float radius;

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
            case InfluencableStats.Support:
                trust = Mathf.Clamp01(trust + amount);
                break;
            case InfluencableStats.Trust:
                support = Mathf.Clamp01(support + amount);
                break;
        }
    }

    public static bool IsPerPupilStat(InfluencableStats stat)
    {
        return stat == InfluencableStats.Support || stat == InfluencableStats.Trust;
    }

    public void CopyStats(PupilStats stats)
    {
        this.support = stats.support;
        this.trust = stats.trust;
        this.isAware = stats.isAware;
        this.radius = stats.radius;
    }
}
