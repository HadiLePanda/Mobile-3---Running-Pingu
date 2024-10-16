using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private Transform menuViewParent; // TODO: assign menu transform that acts as a reference for where the camera will be in the main menu state

    public static CameraController instance;

    private Vector3 offset;

    private void Awake()
    {
        instance = this;

        offset = transform.position - target.transform.position;
    }

    private void Start()
    {
        // snap directly to the position on start
        TeleportToTargetPosition();
    }

    public void TeleportToTargetPosition()
    {
        transform.position = target.position + offset;
        transform.rotation = target.rotation;
    }

    public void TeleportToMainMenuPosition()
    {
        transform.position = menuViewParent.position;
        transform.rotation = menuViewParent.rotation;
    }

    private void LateUpdate()
    {
        // follow the target while keeping the camera's starting distance
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = 0; // don't follow the player on the left and right sides
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
