using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float followSpeed = 1f;

    private Vector3 offset;

    private void Awake()
    {
        offset = transform.position - target.transform.position;
    }

    private void Start()
    {
        // snap directly to the position on start
        transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        // follow the target while keeping the camera's starting distance
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = 0; // don't follow the player on the left and right sides
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
