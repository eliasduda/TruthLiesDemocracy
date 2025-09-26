using UnityEngine;

public class Pupil : MonoBehaviour
{
    [HideInInspector] public PupilManager manager;
    public Vector2 velocity;

    void Start()
    {
        velocity = Random.insideUnitCircle * manager.maxSpeed;
    }

    void Update()
    {
        Vector2 separation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;

        int neighborCount = 0;
        int separationCount = 0;

        foreach (var pupil in manager.pupils)
        {
            if (pupil == this) continue;

            float d = Vector2.Distance(transform.position, pupil.transform.position);

            // Within neighbor radius -> affects alignment & cohesion
            if (d < manager.neighborRadius)
            {
                alignment += pupil.velocity;
                cohesion += (Vector2)pupil.transform.position;
                neighborCount++;
            }

            // Within separation radius -> steer away
            if (d < manager.separationDistance && d > 0)
            {
                separation += ((Vector2)transform.position - (Vector2)pupil.transform.position).normalized / d;
                separationCount++;
            }
        }

        // Finalize averages
        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized * manager.maxSpeed - velocity;
            cohesion = ((cohesion / neighborCount) - (Vector2)transform.position).normalized;
        }

        if (separationCount > 0)
        {
            separation /= separationCount;
        }

        // Add boundary force
        Vector2 boundary = ComputeBoundaryForce();

        // Combine forces with weights
        Vector2 acceleration = Vector2.zero;
        acceleration += separation * manager.separationWeight;
        acceleration += alignment * manager.alignmentWeight;
        acceleration += cohesion * manager.cohesionWeight;
        acceleration += boundary * manager.boundaryWeight;

        // Update velocity
        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, manager.maxSpeed);

        // Move
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Rotate to face direction of movement (optional)
        if (velocity.sqrMagnitude > 0.01f)
            transform.up = velocity.normalized;
    }

    Vector2 ComputeBoundaryForce()
    {
        Rect bounds = manager.GetBounds();
        Vector2 pos = transform.position;
        Vector2 force = Vector2.zero;

        float margin = 1f; // distance before turning back

        if (pos.x < bounds.xMin + margin) force.x = 1;
        else if (pos.x > bounds.xMax - margin) force.x = -1;

        if (pos.y < bounds.yMin + margin) force.y = 1;
        else if (pos.y > bounds.yMax - margin) force.y = -1;

        return force;
    }

}
