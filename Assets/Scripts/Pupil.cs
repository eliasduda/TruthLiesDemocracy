using UnityEngine;
using System.Collections;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    private Rigidbody2D rb;

    private bool isFrozen = false;
    private Vector2 pendingVelocity;
    public float freezeDuration = 0.5f; // Duration to freeze on collision

    void Start()
    {
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
            Debug.Log("Colliding with some object");
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceVelocity = normal * manager.MaxSpeed;
            if (collision.gameObject.GetComponent<Pupil>() != null)
            {
                if (!isFrozen)
                {
                    pendingVelocity = bounceVelocity;
                    StartCoroutine(FreezeCoroutine(freezeDuration));
                }  
            }
            else
            {
                // Wall collision
                velocity = bounceVelocity;
            }
        }

        // Only freeze if colliding with another pupil
        if (collision.gameObject.GetComponent<Pupil>() != null && !isFrozen)
        {
            Vector2 normal = collision.contacts[0].normal;
            pendingVelocity = normal * manager.MaxSpeed;
            StartCoroutine(FreezeCoroutine(freezeDuration));
        }
        else
        {
            // Wall collision
            if (collision.contacts.Length > 0)
            {
                Vector2 normal = collision.contacts[0].normal;
                velocity = normal * manager.MaxSpeed;
            }
        }
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        Debug.Log("Starting freezing");
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        velocity = pendingVelocity;
    }
}