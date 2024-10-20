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
    private const float DISTANCE_BETWEEN_LANES = 3.0f;
    private static readonly int ANIM_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int ANIM_RUNNING = Animator.StringToHash("Running");
    private static readonly int ANIM_SLIDING = Animator.StringToHash("Sliding");
    private static readonly int ANIM_JUMP = Animator.StringToHash("Jump");

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private CharacterController controller;
    [SerializeField] private TrailRenderer trail;

    [Header("Settings")]
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float lookRotationDuration = 0.05f;
    [SerializeField] private float groundedVelocityY = -1.0f;

    [Header("Sounds")]
    [SerializeField] private AudioSource slideSource;
    [SerializeField] private AudioClip swipeSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float swipePitchVariation = 0.1f;
    [SerializeField] private float slidePitchVariation = 0.1f;

    [Header("Speed")]
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private float baseSidewaySpeed = 10.0f;
    [SerializeField] private float terminalVelocity = 20f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 4f;

    [Header("Sliding")]
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float slideHitboxMutliplier = 0.5f;

    private MovementState movementState = MovementState.Idle;
    private int currentLane = 0; // -1 = left, 0 = middle, 1 = right
    private bool isGrounded;
    private bool isFastFalling;
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
        isFastFalling = false;
        currentLane = 0; // middle lane
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
        if (player.State == PlayerState.Running)
        {
            // calculate speed
            // speed is capped at terminal velocity
            speed = Mathf.Min(baseSpeed * GameManager.Instance.DifficultyModifier, terminalVelocity);

            // check grounded state
            isGrounded = GroundedCheck();

            HandleInput();
            Move();
            ApplyGravity();
            HandleRotation();

            HandleTrailDisplay();
            UpdateAnimations();

            wasGroundedLastFrame = isGrounded;
        }
        else
        {
            speed = 0f;
            ApplyGravity();
        }
    }

    private void HandleInput()
    {
        // handle player input for switching lanes
        if (MobileInput.Instance.SwipeLeft)
        {
            // play swipe sfx
            AudioManager.Instance.PlaySound2DOneShot(swipeSound, pitchVariation: swipePitchVariation);

            // move towards left lane
            ChangeLane(MovementDirection.Left);
        }
        else if (MobileInput.Instance.SwipeRight)
        {
            // play swipe sfx
            AudioManager.Instance.PlaySound2DOneShot(swipeSound, pitchVariation: swipePitchVariation);

            // move towards right lane
            ChangeLane(MovementDirection.Right);
        }

        // we landed
        if (!wasGroundedLastFrame && isGrounded)
        {
            // if we were fast falling, chain to a slide
            if (isFastFalling)
            {
                if (slideRoutine != null)
                    StopCoroutine(slideRoutine);
                slideRoutine = StartCoroutine(Slide());
            }
            // otherwise land normally
            else
            {
                Land();
            }
        }

        // we're on the ground
        if (isGrounded)
        {
            // detected jump input
            if (MobileInput.Instance.SwipeUp)
            {
                // if we were sliding cancel the slide?
                if (movementState == MovementState.Sliding)
                    CancelSlide();

                Jump();
            }
            // detected slide input
            else if (MobileInput.Instance.SwipeDown)
            {
                // only slide if we're not already sliding
                if (movementState != MovementState.Sliding)
                {
                    if (slideRoutine != null)
                        StopCoroutine(slideRoutine);
                    slideRoutine = StartCoroutine(Slide());
                }
            }
        }
        // we're in the air
        else
        {
            // fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                FastFall();
            }
        }
    }

    private void Jump()
    {
        isGrounded = false;
        movementState = MovementState.Airborne;
        verticalVelocity = jumpForce;

        player.anim.SetTrigger(ANIM_JUMP);

        // play jump sound
        AudioManager.Instance.PlaySound2DOneShot(jumpSound, pitchVariation: 0.1f);
    }

    private void FastFall()
    {
        verticalVelocity = -jumpForce;
        isFastFalling = true;
    }

    private void Land()
    {
        movementState = MovementState.Running;
        isFastFalling = false;
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

        isFastFalling = false;

        // change the collider size
        SetRegularHitbox();
        SetSlidingHitbox();

        // randomize slide pitch
        slideSource.pitch = Random.Range(1 - slidePitchVariation, 1 + slidePitchVariation);
        // enable sliding sfx
        slideSource.Play();
    }

    private void SetSlidingHitbox()
    {
        controller.height *= slideHitboxMutliplier;
        controller.center = new Vector3(controller.center.x, controller.center.y * slideHitboxMutliplier, controller.center.z);
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

        // disable sliding sfx
        slideSource.Stop();
    }

    private void Move()
    {
        // calculate move delta
        Vector3 moveVector = Vector3.zero;

        moveVector.x = SnapToLane();
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // move the player
        controller.Move(moveVector * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            verticalVelocity = groundedVelocityY;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (verticalVelocity < -terminalVelocity)
                verticalVelocity = -terminalVelocity;
        }
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

    private void HandleTrailDisplay()
    {
        trail.emitting = MovementState is MovementState.Airborne or MovementState.Sliding;
    }

    private void UpdateAnimations()
    {
        player.anim.SetBool(ANIM_RUNNING, movementState == MovementState.Running);
        player.anim.SetBool(ANIM_SLIDING, movementState == MovementState.Sliding);
        player.anim.SetBool(ANIM_GROUNDED, isGrounded);
    }

    private bool GroundedCheck()
    {
        return controller.isGrounded;
    }

    private void ChangeLane(MovementDirection moveDirection)
    {
        // switch lane reference based on given input
        currentLane += (moveDirection == MovementDirection.Right) ? 1 : -1;
        currentLane = Mathf.Clamp(currentLane, -1, 1);
    }
    
    public float SnapToLane()
    {
        float r = 0.0f;

        // if we're not directly on top of a lane
        if (transform.position.x != (currentLane * DISTANCE_BETWEEN_LANES))
        {
            float deltaToDesiredPosition = (currentLane * DISTANCE_BETWEEN_LANES) - transform.position.x;
            r = (deltaToDesiredPosition > 0) ? 1 : -1;
            r *= baseSidewaySpeed;

            float actualDistance = r * Time.deltaTime;
            if (Mathf.Abs(actualDistance) > Mathf.Abs(deltaToDesiredPosition))
                r = deltaToDesiredPosition * (1 / Time.deltaTime);
        }
        else
        {
            r = 0;
        }

        return r;
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // only detect collision when running
        if (player.State != PlayerState.Running)
            return;
        
        // check hit by layer name
        string hitLayerName = LayerMask.LayerToName(hit.gameObject.layer);

        // hit an obstacle
        if (hitLayerName == "Obstacle")
        {
            // make the player crash and trigger game over
            player.Crash();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // only detect when running
        if (Player.Instance.State != PlayerState.Running)
            return;

        // check hit by layer name
        string otherLayerName = LayerMask.LayerToName(other.gameObject.layer);
        
        // hit an obstacle
        if (otherLayerName == "Obstacle")
        {
            // make the player crash and trigger game over
            player.Crash();
            return;
        }

        // entered a pickup, and we're able to pick it up
        var pickup = other.GetComponentInParent<Pickup>();
        if (pickup &&  pickup.CanPickUp())
        {
            // pick it up
            pickup.PickUp();
        }
    }
}
