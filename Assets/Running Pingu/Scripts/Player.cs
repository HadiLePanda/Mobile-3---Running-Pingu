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
    [SerializeField] private Animator anim;
    [field:SerializeField] public PlayerController Controller { get; private set; }

    private PlayerState playerState = PlayerState.Idle;

    public PlayerState State => playerState;

    public static Player instance;

    private void Awake()
    {
        instance = this;
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
        Controller.StopRunning();
    }

    public void Run()
    {
        playerState = PlayerState.Running;
        Controller.StartRunning();
    }

    public void Die()
    {
        playerState = PlayerState.Dead;
        Controller.StopRunning();
    }
}
