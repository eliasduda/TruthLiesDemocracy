using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        velocity = Random.insideUnitCircle.normalized * manager.MaxSpeed;
        rb.linearVelocity = velocity;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = velocity; // Keep moving in the current direction
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            velocity = normal * manager.MaxSpeed;
        }
    }
}