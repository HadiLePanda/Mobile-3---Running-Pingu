using System.Collections;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Idle,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float difficultyncreaseInterval = 2.5f;
    [SerializeField] private float difficultyIncreaseAmount = 0.1f;

    private float score;
    private int coinsScore;
    private float difficultyModifier = 1f;
    private float difficultyIncreaseLastTick;
    private GameState gameState = GameState.MainMenu;

    private bool isGameStarted = false;
    private Coroutine gameOverRoutine;

    public int Score => Mathf.RoundToInt(score);
    public int CoinsScore => coinsScore;
    public float DifficultyModifier => difficultyModifier;
    public GameState GameState => gameState;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        // TODO: make sure we start in main menu once we've got a main menu working
        //gameState = GameState.Mainmenu;

        gameState = GameState.Idle;
    }

    private void Update()
    {
        // start playing the game when we tap the screen while the race in in Idle
        if (MobileInput.instance.Tap &&
            gameState == GameState.Idle)
        {
            isGameStarted = true;
            StartRunning();
        }

        // process playing state logic
        if (gameState == GameState.Playing)
        {
            // increase difficulty over time
            if (Time.time - difficultyIncreaseLastTick > difficultyncreaseInterval)
            {
                difficultyIncreaseLastTick = Time.time;
                difficultyModifier += difficultyIncreaseAmount;
            }

            // increase the score over time
            score += (Time.deltaTime * difficultyModifier);
        }
    }

    public void Idle()
    {
        gameState = GameState.Idle;
        isGameStarted = false;

        ResetStates();

        // teleport the player to the start and make them idle
        Player.instance.TeleportToStart();
        Player.instance.Idle();

        // teleport the camera to the player
        CameraController.instance.TeleportToTargetPosition();
    }

    private void ResetStates()
    {
        // reset states
        difficultyModifier = 1f;
        difficultyIncreaseLastTick = Time.time;
        score = 0;
        coinsScore = 0;
    }

    public void StartRunning()
    {
        gameState = GameState.Playing;

        Player.instance.Run();
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;

        if (gameOverRoutine != null)
            StopCoroutine(gameOverRoutine);
        gameOverRoutine = StartCoroutine(GameOverSequence());
    }

    // TODO: gameover sequence
    // that will ask to retry or go to main menu
    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2);
        Retry();
    }

    public void Retry()
    {
        Idle();
    }

    public void GoToMainMenu()
    {
        gameState = GameState.MainMenu;

        isGameStarted = false;
    }

    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
    }
    public void AddCoinsScore(int amount)
    {
        coinsScore = Mathf.Max(0, coinsScore + amount);
    }

    public void UpdateModifier(int newModifier)
    {
        difficultyModifier = newModifier;
    }
}
