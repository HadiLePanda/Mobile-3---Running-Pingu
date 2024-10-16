using UnityEngine;

public class Lane : MonoBehaviour
{
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = Player.Instance.transform;
    }

    private void Update()
    {
        // follow the player on the z axis
        transform.position = Vector3.forward * playerTransform.position.z;
    }
}
