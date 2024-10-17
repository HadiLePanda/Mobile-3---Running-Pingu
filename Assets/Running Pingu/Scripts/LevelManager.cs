using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public List<Obstacle> ramps = new();
    public List<Obstacle> longBlocks = new();
    public List<Obstacle> jumps = new();
    public List<Obstacle> slides = new();
    [SerializeField] private List<Segment> availableSegments = new();
    [SerializeField] private List<Segment> availableTransitions = new();
    
    [Header("Settings")]
    [SerializeField] private float distanceBeforeSpawn = 100f;
    [SerializeField] private int initialSegments = 10;
    [SerializeField] private int initialTransitionSegments = 2;
    [SerializeField] private int maxSegmentsOnScreen = 15;
    [SerializeField] private bool showCollider = false;

    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continuousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    private List<Obstacle> spawnedObstacles = new(); // all the obstacles in the pool
    private List<Segment> spawnedSegments = new();

    public bool ShowCollider => showCollider;
    
    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;

        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        GenerateInitialSegments();
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.GameOver)
            return;

        // check if the camera is close enough to generate new segments
        if (currentSpawnZ - cameraContainer.position.z < distanceBeforeSpawn)
        {
            GenerateSegment();
        }

        // we've spawned a new segment that exceeds the max number of segments at one time
        // so we despawn the oldest one
        if (amountOfActiveSegments >= maxSegmentsOnScreen)
        {
            // we despawn the oldest segment
            spawnedSegments[amountOfActiveSegments - 1].Despawn();
            amountOfActiveSegments--;
        }
    }

    private void GenerateInitialSegments()
    {
        for (int i = 0; i < initialSegments; i++)
        {
            if (i < initialTransitionSegments)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        if (Random.Range(0f, 1f) < (continuousSegments * 0.25f))
        {
            // spawn transition segment
            continuousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continuousSegments++;
        }
    }

    private void SpawnSegment()
    {
        var possibleSegments = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSegments.Count);

        Segment segment = GetSegment(id, false);

        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;

        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += segment.length;
        amountOfActiveSegments++;
        segment.Spawn();
    }

    private void SpawnTransition()
    {
        var possibleTransitions = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransitions.Count);

        Segment segment = GetSegment(id, true);

        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;

        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += segment.length;
        amountOfActiveSegments++;
        segment.Spawn();
    }

    public Segment GetSegment(int id, bool isTransition)
    {
        Segment segment = null;
        segment = spawnedSegments.Find(x => x.Id == id && x.isTransition == isTransition && !x.gameObject.activeSelf);

        // not found, spawn a new segment
        if (segment == null)
        {
            GameObject go = Instantiate(isTransition ? availableTransitions[id].gameObject : availableSegments[id].gameObject);
            segment = go.GetComponent<Segment>();

            segment.Id = id;
            segment.isTransition = isTransition;

            spawnedSegments.Insert(0, segment);
        }
        // segment found, reorder it
        else
        {
            spawnedSegments.Remove(segment);
            spawnedSegments.Insert(0, segment);
        }

        return segment;
    }

    public Obstacle GetObstacle(ObstacleType type, int visualIndex)
    {
        // get an obstacle that is of:
        // same type, same visual index, not currently active
        Obstacle obstacle = spawnedObstacles.Find(x => x.type == type && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        // obstacle not found, spawn it
        if (obstacle == null)
        {
            GameObject obstacleObject = null;
            if (type == ObstacleType.Ramp)
                obstacleObject = ramps[visualIndex].gameObject;
            else if (type == ObstacleType.Longblock)
                obstacleObject = longBlocks[visualIndex].gameObject;
            else if (type == ObstacleType.Jump)
                obstacleObject = jumps[visualIndex].gameObject;
            else if (type == ObstacleType.Slide)
                obstacleObject = slides[visualIndex].gameObject;

            // spawn new obstacle
            var obstacleInstance = Instantiate(obstacleObject);
            obstacle = obstacleInstance.GetComponent<Obstacle>();
            // store it in the list of spawned obstacles
            spawnedObstacles.Add(obstacle);
        }

        return obstacle;
    }
}
