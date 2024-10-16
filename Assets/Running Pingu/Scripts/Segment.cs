using UnityEngine;

public class Segment : MonoBehaviour
{
    public int Id { get; set; }
    public bool isTransition;

    public int length;
    public int beginY1, beginY2, beginY3;
    public int endY1, endY2, endY3;

    private ObstacleSpawner[] obstacleSpawners;
    private CoinSpawner[] coinSpawners;

    private void Awake()
    {
        // get all the spawners in this segment
        obstacleSpawners = gameObject.GetComponentsInChildren<ObstacleSpawner>();
        coinSpawners = gameObject.GetComponentsInChildren<CoinSpawner>();

        // toggle colliders visibility
        for (int i = 0; i < obstacleSpawners.Length; i++)
            obstacleSpawners[i].SetCollidersVisibility(LevelManager.Instance.ShowCollider);
    }

    public void Spawn()
    {
        gameObject.SetActive(true);

        // spawn the obstacles
        for (int i = 0; i < obstacleSpawners.Length; i++)
            obstacleSpawners[i].Spawn();
    }

    public void Despawn()
    {
        gameObject.SetActive(false);

        // despawn the obstacles
        for (int i = 0; i < obstacleSpawners.Length; i++)
            obstacleSpawners[i].Despawn();
    }
}
