using UnityEngine;

public enum ObstacleType
{
    None = -1,
    Ramp = 0,
    Longblock = 1,
    Jump = 2,
    Slide = 3
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType type;
    public int visualIndex;
}
