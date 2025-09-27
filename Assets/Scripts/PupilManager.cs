using System.Collections.Generic;
using UnityEngine;

public class PupilManager : MonoBehaviour
{
    public Pupil pupilPrefab;
    public int pupilCount = 50;

    [SerializeField] private float _maxSpeed = 1f;
    public float MaxSpeed => _maxSpeed;

    private Vector2 areaSize; // box width/height

    [HideInInspector] public List<Pupil> pupils = new List<Pupil>();
    public Pupil you;

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

        bool youSpawned = false;
        // Spawn pupils at random positions within the area
        for (int i = 0; i < pupilCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );
            Pupil pupil = Instantiate(pupilPrefab, pos, Quaternion.identity);
            pupil.manager = this;
            if (!youSpawned)
            {
                you = pupil;
                pupil.isYou = true;
                youSpawned = true;
            }
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
}
