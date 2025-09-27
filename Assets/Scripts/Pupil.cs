using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    public GameObject armLeft;
    public GameObject armRight;
    public float swingAmplitude = 40f;  // max angle (±40)
    public float swingSpeed = 3f;

    private Rigidbody2D rb;

    private bool inDiscussion = false;
    [HideInInspector] public Vector2 pendingVelocity;
    private Coroutine discussCoroutine;

    [HideInInspector] public Vector2 ringTargetPosition;
    [HideInInspector] public bool moveToRing = false;
    [HideInInspector] public Vector2 ringCenter;

    public HashSet<Pupil> connectedPupils = new HashSet<Pupil>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        velocity = Random.insideUnitCircle.normalized * manager.MaxSpeed;
        rb.linearVelocity = velocity;

        // --- Arm positioning ---
        // Get the radius from the CircleCollider2D
        var circle = GetComponent<CircleCollider2D>();
        float radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        Transform capsuleTransform = armLeft.transform.Find("Capsule");
        var armCollider = armLeft.GetComponent<CapsuleCollider>();
        CapsuleCollider capsuleCol = capsuleTransform.GetComponent<CapsuleCollider>();
        float margin = capsuleCol.radius*capsuleCol.transform.lossyScale.x;

        armLeft.transform.localPosition = new Vector3(-(radius + margin), 0, 0);
        armRight.transform.localPosition = new Vector3((radius + margin), 0, 0);
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
        }
        else
        {
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
            rotateAngle = Mathf.Atan2((ringCenter-rb.position).y, (ringCenter - rb.position).x) * Mathf.Rad2Deg;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length == 0) return;

        Pupil otherPupil = collision.gameObject.GetComponent<Pupil>();
        Vector2 normal = collision.contacts[0].normal;
        Vector2 bounceVelocity = normal * manager.MaxSpeed;

        if (otherPupil != null)
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

            // Set pending velocity for all
            foreach (var pupil in group)
            {
                pupil.pendingVelocity = bounceVelocity;
            }

            // Discuss and arrange ring for all
            foreach (var pupil in group)
            {
                pupil.Discuss();
            }
            PupilManager.ArrangeRing(new List<Pupil>(group));
        }
        else
        {
            // Wall collision
            velocity = bounceVelocity;
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
        discussCoroutine = StartCoroutine(DiscussCoroutine(manager.discussDuration));
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
}