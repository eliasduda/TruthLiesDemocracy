using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    private Rigidbody2D rb;

    private bool inDiscussion = false;
    [HideInInspector] public Vector2 pendingVelocity;
    private Coroutine discussCoroutine;

    [HideInInspector] public Vector2 ringTargetPosition;
    [HideInInspector] public bool moveToRing = false;

    public HashSet<Pupil> connectedPupils = new HashSet<Pupil>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        velocity = Random.insideUnitCircle.normalized * manager.MaxSpeed;
        rb.linearVelocity = velocity;
    }

    void FixedUpdate()
    {
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