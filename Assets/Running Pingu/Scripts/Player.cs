using UnityEngine;

public enum PlayerState
{
    Idle,
    Running,
    Dead
}

public class Player : MonoBehaviour
{
    public const string ANIM_DEAD = "Dead";
    public const string ANIM_IDLE = "Idle";

    [Header("References")]
    public Animator defaultAnimator;
    public Animator anim;
    [field:SerializeField] public PlayerController Controller { get; private set; }
    [field:SerializeField] public PlayerSkinController SkinController { get; private set; }

    [Header("Sounds")]
    public AudioClip crashSound;

    [Header("Effects")]
    public ParticleSystem crashEffect;
    public ParticleSystem pickupCoinEffect;

    private PlayerState playerState = PlayerState.Idle;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public PlayerState State => playerState;

    public static Player Instance;

    private void Awake()
    {
        Instance = this;

        startPosition = transform.position;
    }

    private void Start()
    {
        if (anim == null)
            anim = defaultAnimator;

        // setup effects state
        crashEffect.gameObject.SetActive(false);
        pickupCoinEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        // update animations based on their states
        anim.SetBool(ANIM_DEAD, playerState == PlayerState.Dead);
        anim.SetBool(ANIM_IDLE, playerState == PlayerState.Idle);
    }

    public void Idle()
    {
        playerState = PlayerState.Idle;
        Controller.Idle();
    }

    public void Run()
    {
        playerState = PlayerState.Running;
        Controller.StartRunning();
    }

    public void Crash()
    {
        playerState = PlayerState.Dead;

        // stop running
        Controller.StopRunning();

        // play crash sound
        AudioManager.Instance.PlaySound2DOneShot(crashSound);

        // play crash effect
        crashEffect.gameObject.SetActive(true);
        crashEffect.Play();

        // trigger game over
        GameManager.Instance.GameOver();
    }

    public void TeleportToStart()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
