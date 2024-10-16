using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public List<Obstacle> ramps = new();
    public List<Obstacle> longBlocks = new();
    public List<Obstacle> jumps = new();
    public List<Obstacle> slides = new();

    private List<Obstacle> spawnedObstacles = new(); // all the obstacles in the pool

    public static LevelManager Instance;
    
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
