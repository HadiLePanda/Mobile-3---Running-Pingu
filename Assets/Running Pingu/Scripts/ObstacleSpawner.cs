using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleType type;

    private Obstacle currentObstacle;

    public void Spawn()
    {
        currentObstacle = LevelManager.Instance.GetObstacle(type, 0); // TODO: later randomize visual index
        currentObstacle.gameObject.SetActive(true);
        currentObstacle.transform.SetParent(transform, false);
    }

    public void Despawn()
    {
        // put back in pool
        currentObstacle.gameObject.SetActive(false);
    }
}
