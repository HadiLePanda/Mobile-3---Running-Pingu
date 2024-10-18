using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private float sessionScore;
    private int sessionCoinsScore;
    private int highscoreBeforeSession;
    private float difficultyModifier = 1f;
    private float difficultyIncreaseLastTick;
    private GameState gameState = GameState.MainMenu;

    private bool isGameStarted = false;
    private Coroutine gameOverRoutine;

    private UserData userData;
    public UserData UserData => userData;
    public void SetUsername(string username) => userData.username = username;
    public void SetHighscore(int highscore) => userData.highscore = highscore;
    public void SetCoins(int coins) => userData.coins = coins;

    public int SessionScore => Mathf.FloorToInt(sessionScore);
    public int SessionCoinsScore => sessionCoinsScore;
    public float DifficultyModifier => difficultyModifier;
    public GameState GameState => gameState;

    public Action onGameOver;
    public static Action onDataLoaded;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;

        LoadUserData();
    }

    private void Start()
    {
        Idle();
    }

    private void Update()
    {
        // start playing the game when we tap the screen while the race in in Idle
        //if (MobileInput.Instance.Tap &&
        //    gameState == GameState.Idle)
        //{
        //    
        //    StartRunning();
        //}

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
            sessionScore += (Time.deltaTime * difficultyModifier);
        }
    }

    public void LoadUserData()
    {
        userData = SaveManager.Instance.LoadUserData();
        onDataLoaded?.Invoke();
        Debug.Log("User data loaded.");
    }

    public void Idle()
    {
        gameState = GameState.Idle;
        isGameStarted = false;

        ResetStates();

        // teleport the player to the start and make them idle
        Player.Instance.TeleportToStart();
        Player.Instance.Idle();

        // teleport the camera to the player
        CameraController.Instance.SwitchToIdleCamera();

        // TODO: reset other stuff like spawners if we decide to not reload the scene
    }

    private void ResetStates()
    {
        // reset states
        difficultyModifier = 1f;
        difficultyIncreaseLastTick = Time.time;
        sessionScore = 0;
        sessionCoinsScore = 0;
    }

    public void StartRunning()
    {
        gameState = GameState.Playing;
        isGameStarted = true;

        // keep a reference of the current highscore before starting the session
        highscoreBeforeSession = userData.highscore;

        ResetStates();

        // start the run
        Player.Instance.Run();

        // switch to gameplay camera
        CameraController.Instance.SwitchToGameplayCamera();
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        onGameOver?.Invoke();

        // play game over sequence
        if (gameOverRoutine != null)
            StopCoroutine(gameOverRoutine);
        gameOverRoutine = StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // check if we beat our highscore
        if (sessionScore > userData.highscore)
        {
            // save new highscore
            var newHighscore = Mathf.FloorToInt(sessionScore);
            SetHighscore(newHighscore);
            SaveManager.Instance.SaveHighscore(newHighscore);
        }

        // add collected coins to the user total coins
        var newCoins = userData.coins + sessionCoinsScore;
        SetCoins(newCoins);
        SaveManager.Instance.SaveCoins(newCoins);

        yield return null;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        gameState = GameState.MainMenu;

        isGameStarted = false;
    }

    public void AddScore(int amount)
    {
        sessionScore = Mathf.Max(0, sessionScore + amount);
    }
    public void AddCoinsScore(int amount)
    {
        sessionCoinsScore = Mathf.Max(0, sessionCoinsScore + amount);
    }

    public void UpdateModifier(int newModifier)
    {
        difficultyModifier = newModifier;
    }
}
