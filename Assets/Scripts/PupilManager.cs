using System.Collections.Generic;
using UnityEngine;

public class PupilManager : MonoBehaviour
{
    public Pupil pupilPrefab;
    public int pupilCount = 50;
    public Vector2 areaSize = new Vector2(20f, 20f); // box width/height

    [Header("Boid Settings")]
    public float neighborRadius = 3f;
    public float separationDistance = 1f;
    public float maxSpeed = 5f;
    public float maxForce = 0.5f;

    [Header("Behavior Weights")]
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float boundaryWeight = 5f;

    [HideInInspector] public List<Pupil> pupils = new List<Pupil>();

    void Start()
    {
        for (int i = 0; i < pupilCount; i++)
        {
            Debug.Log("Spawning pupil " + i);
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
}
