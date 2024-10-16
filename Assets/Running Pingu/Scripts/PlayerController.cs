using System.Collections;
using UnityEngine;

public enum MovementDirection
{
    Left,
    Right
}

public enum MovementState
{
    Idle,
    Running,
    Sliding,
    Airborne
}

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 3.0f;
    private const string ANIM_GROUNDED = "IsGrounded";
    private const string ANIM_RUNNING = "Running";
    private const string ANIM_SLIDING = "Sliding";
    private const string ANIM_JUMP = "Jump";

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    [Header("Settings")]
    [SerializeField] private float gravity = 12f;
    [SerializeField] private float lookRotationDuration = 0.05f;

    [Header("Speed")]
    [SerializeField] private float baseSpeed = 7f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 4f;

    [Header("Sliding")]
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float slideHitboxMutliplier = 0.5f;

    [Header("Grounded")]
    [SerializeField] private LayerMask groundedLayerMask;
    [SerializeField] private float groundedRayOffsetY = 0.2f;
    [SerializeField] private float groundedRayTreshold = 0.1f;

    private MovementState movementState = MovementState.Idle;
    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private float verticalVelocity;
    private float startingHitboxHeight;
    private Vector3 startingHitboxCenter;
    private float speed;
    private Coroutine slideRoutine;

    public bool IsGrounded => isGrounded;
    public float Speed => speed;
    public MovementState MovementState => movementState;

    private void Start()
    {
        // store original collider size
        startingHitboxHeight = controller.height;
        startingHitboxCenter = controller.center;

        // start at idle
        Idle();
    }

    public void Idle()
    {
        movementState = MovementState.Idle;

        // reset states
        isGrounded = true;
        wasGroundedLastFrame = true;
        desiredLane = 1; // middle lane
    }

    public void StartRunning()
    {
        movementState = MovementState.Running;
    }
    public void StopRunning()
    {
        Idle();
    }

    private void Update()
    {
        if (player.State != PlayerState.Running)
            return;

        // calculate speed
        speed = baseSpeed * GameManager.instance.DifficultyModifier;

        // check grounded state
        wasGroundedLastFrame = isGrounded;
        isGrounded = GroundedRaycast();

        HandleInput();
        Move();
        HandleRotation();

        UpdateAnimations();
    }

    private void HandleInput()
    {
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

        // we're on the ground
        if (isGrounded)
        {
            // detected jump input
            if (MobileInput.instance.SwipeUp)
            {
                // if we were sliding cancel the slide?
                if (movementState == MovementState.Sliding)
                    CancelSlide();

                Jump();
            }
            // detected slide input
            else if (MobileInput.instance.SwipeDown)
            {
                // only slide if we're not already sliding
                if (movementState != MovementState.Sliding)
                {
                    slideRoutine = StartCoroutine(Slide());
                }
            }
        }
        // we're in the air
        else
        {
            // fast falling mechanic
            if (MobileInput.instance.SwipeDown)
            {
                FastFall();
            }
        }

        if (!wasGroundedLastFrame && isGrounded)
        {
            Land();
        }
    }

    private void Jump()
    {
        isGrounded = false;
        movementState = MovementState.Airborne;
        verticalVelocity = jumpForce;

        anim.SetTrigger(ANIM_JUMP);

        Debug.Log("Jump");
    }

    private void FastFall()
    {
        verticalVelocity = -jumpForce;
        Debug.Log("Fast Fall");
    }

    private void Land()
    {
        movementState = MovementState.Running;

        Debug.Log("Landed");
    }

    private IEnumerator Slide()
    {
        StartSliding();
        yield return new WaitForSeconds(slideDuration);
        StopSliding();
    }
    private void StartSliding()
    {
        movementState = MovementState.Sliding;

        // change the collider size
        SetRegularHitbox();
        SetSlidingHitbox();

        Debug.Log("Slide");
    }

    private void SetSlidingHitbox()
    {
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }
    private void SetRegularHitbox()
    {
        controller.height = startingHitboxHeight;
        controller.center = startingHitboxCenter;
    }

    private void CancelSlide()
    {
        // cancel slide routine execution
        if (slideRoutine != null)
            StopCoroutine(slideRoutine);
        StopSliding();
    }

    private void StopSliding()
    {
        // go back to running or airborne state
        movementState = isGrounded ? MovementState.Running : MovementState.Airborne;

        // set the collider back to original size
        SetRegularHitbox();
    }

    private void Move()
    {
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

        // calculate Y
        // we're grounded
        if (isGrounded)
        {
            verticalVelocity = -0.1f;
        }
        // we're airborne
        else
        {
            // apply gravity
            verticalVelocity -= (gravity * Time.deltaTime);
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // move the player
        controller.Move(moveVector * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // rotate the player to face where he is going
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, lookRotationDuration);
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool(ANIM_RUNNING, movementState == MovementState.Running);
        anim.SetBool(ANIM_SLIDING, movementState == MovementState.Sliding);
        anim.SetBool(ANIM_GROUNDED, isGrounded);
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // only detect collision when running
        if (player.State != PlayerState.Running)
            return;

        // hit an obstacle
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            // make the player crash and trigger game over
            player.Crash();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // only detect when running
        if (Player.instance.State != PlayerState.Running)
            return;

        // entered a pickup
        var pickup = other.GetComponentInParent<Pickup>();
        if (pickup)
        {
            if (pickup.CanPickUp())
            {
                // pick it up
                pickup.PickUp();
            }
        }
    }
}
