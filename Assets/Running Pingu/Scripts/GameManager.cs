using UnityEngine;

public enum GameState
{
    Menu,
    Idle,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private int score;
    private int coins;
    private int scoreModifier;
    private GameState gameState;

    private bool isGameStarted = false;

    public int Score => score;
    public int Coins => coins;
    public int ScoreModifier => scoreModifier;
    public GameState GameState => gameState;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        gameState = GameState.Menu;
    }

    private void Update()
    {
        // start playing the game when we tap the screen while the game hasn't started
        if (MobileInput.instance.Tap &&
            !isGameStarted)
        {
            StartRunning();
        }
    }

    public void StartRunning()
    {
        gameState = GameState.Playing;

        Player.instance.Run();
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;

        Player.instance.Die();
    }

    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
    }

    public void AddCoins(int amount)
    {
        coins = Mathf.Max(0, coins + amount);
    }
}
