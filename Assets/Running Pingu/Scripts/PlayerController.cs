using UnityEngine;

public enum MovementDirection
{
    Left,
    Right
}

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;
    private const string ANIM_GROUNDED = "IsGrounded";
    private const string ANIM_RUNNING = "Running";
    private const string ANIM_JUMP = "Jump";

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    [Header("Settings")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float gravity = 12f;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float rotationDuration = 0.05f;
    [SerializeField] private float groundedRayOffsetY = 0.2f;
    [SerializeField] private float groundedRayTreshold = 0.1f;
    [SerializeField] private LayerMask groundedLayerMask;

    private bool isRunning;
    private bool isGrounded;
    private float verticalVelocity;
    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right

    public bool IsRunning => isRunning;
    public bool IsGrounded => isGrounded;

    public void StartRunning()
    {
        isRunning = true;
    }
    public void StopRunning()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (player.State != PlayerState.Running)
            return;

        // handle player input for switching lanes
        if (MobileInput.instance.SwipeLeft)
        {
            MoveLane(MovementDirection.Left);
            Debug.Log("Moving left");
        }
        else if (MobileInput.instance.SwipeRight)
        {
            MoveLane(MovementDirection.Right);
            Debug.Log("Moving right");
        }

        // calculate where we move at
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        // calculate move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        // check grounded state
        isGrounded = GroundedRaycast();
        
        // calculate Y
        // we're grounded
        if (isGrounded)
        {
            verticalVelocity = -0.1f;

            if (MobileInput.instance.SwipeUp)
            {
                Jump();
            }
        }
        // we're airborne
        else
        {
            // apply gravity
            verticalVelocity -= (gravity * Time.deltaTime);

            // fast falling mechanic
            if (MobileInput.instance.SwipeDown)
            {
                FastFall();
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // move the player
        controller.Move(moveVector * Time.deltaTime);

        // rotate the player to face where he is going
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, rotationDuration);
        }

        // update animations
        anim.SetBool(ANIM_RUNNING, isRunning);
        anim.SetBool(ANIM_GROUNDED, isGrounded);
    }

    private void FastFall()
    {
        verticalVelocity = -jumpForce;
    }

    private void Jump()
    {
        anim.SetTrigger(ANIM_JUMP);
        verticalVelocity = jumpForce;
        Debug.Log("Jumped");
    }

    private bool GroundedRaycast()
    {
        Ray groundRay = new(
            new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + groundedRayOffsetY, controller.bounds.center.z),
                Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, groundedRayOffsetY + groundedRayTreshold, groundedLayerMask);

    }

    private void MoveLane(MovementDirection moveDirection)
    {
        // switch lane reference based on given input
        desiredLane += (moveDirection == MovementDirection.Right) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }
}
