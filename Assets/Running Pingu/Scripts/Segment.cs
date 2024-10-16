using UnityEngine;

public class Segment : MonoBehaviour
{
    public int Id { get; set; }
    public bool isTransition;

    public int length;
    public int beginY1, beginY2, beginY3;
    public int endY1, endY2, endY3;

    private Obstacle[] obstacles;

    private void Awake()
    {
        obstacles = gameObject.GetComponentsInChildren<Obstacle>();
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
