using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    public PupilStats stats = new PupilStats();

    private Rigidbody2D rb;

    private bool isFrozen = false;
    private Vector2 pendingVelocity;
    public float freezeDuration = 0.5f; // Duration to freeze on collision
    private Coroutine freezeCoroutine;

    public List<Pupil> connectedPupils = new List<Pupil>();

    void Start()
    {
        GameManager.instance.eventManager.onPupilStatInfluenced.AddListener(ApplyStatChange);

        velocity = Random.insideUnitCircle * manager.maxSpeed;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        velocity = Random.insideUnitCircle.normalized * manager.MaxSpeed;
        rb.linearVelocity = velocity;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = isFrozen ? Vector2.zero : velocity; // Keep moving in the current direction
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceVelocity = normal * manager.MaxSpeed;
            if (collision.gameObject.GetComponent<Pupil>() != null)
            {
                pendingVelocity = bounceVelocity;
                Pupil otherPupil = collision.gameObject.GetComponent<Pupil>();

                if (isFrozen) // already part of group -> add new pupil to everyones group and re-freeze all
                {
                    foreach (Pupil pupil in new List<Pupil>(connectedPupils))
                    {
                        pupil.Freeze(freezeDuration);
                        pupil.connectedPupils.Add(otherPupil);
                    }

                }
                connectedPupils.Add(otherPupil);
                Freeze(freezeDuration);
            }
            else
            {
                // Wall collision
                velocity = bounceVelocity;
            }
        }
    }

    public void Freeze(float duration)
    {
        if (freezeCoroutine != null)
            StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(FreezeCoroutine(duration));
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        velocity = pendingVelocity;
        freezeCoroutine = null;
        connectedPupils.Clear();
    }
    void ApplyStatChange(InfluencableStats stat, float amount, Pupil pupil)
    {
        if(pupil == this) stats.ApplyChange(stat, amount);
    }

}

public class PupilStats
{
    public float alignment;
    public float engagement;

    public float GetStat(InfluencableStats stat)
    {
        return stat switch
        {
            InfluencableStats.Alignment => alignment,
            InfluencableStats.Engagement => engagement,
            _ => 0f,
        };
    }

    public void ApplyChange(InfluencableStats stat, float amount)
    {
        switch (stat)
        {
            case InfluencableStats.Alignment:
                alignment = Mathf.Clamp01(alignment + amount);
                break;
            case InfluencableStats.Engagement:
                engagement = Mathf.Clamp01(engagement + amount);
                break;
        }
    }

    public static bool IsPerPupilStat(InfluencableStats stat)
    {
        return stat == InfluencableStats.Alignment || stat == InfluencableStats.Engagement;
    }
}
