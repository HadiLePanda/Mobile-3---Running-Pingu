using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleType type;

    private Obstacle currentObstacle;

    public void Spawn()
    {
        int numberOfVisuals = type switch
        {
            ObstacleType.Jump => LevelManager.Instance.jumps.Count,
            ObstacleType.Slide => LevelManager.Instance.slides.Count,
            ObstacleType.Longblock => LevelManager.Instance.longBlocks.Count,
            ObstacleType.Ramp => LevelManager.Instance.ramps.Count,
            _ => 0
        };

        // spawn the corresponding obstacle with a randomly selected variation
        var randomVisualIndex = Random.Range(0, numberOfVisuals);
        currentObstacle = LevelManager.Instance.GetObstacle(type, randomVisualIndex);
        currentObstacle.gameObject.SetActive(true);
        currentObstacle.transform.SetParent(transform, false);
    }

    public void Despawn()
    {
        // put back in pool
        currentObstacle.gameObject.SetActive(false);
    }

    public void SetCollidersVisibility(bool visible)
    {
        foreach (MeshRenderer renderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = visible;
        }
    }
}
