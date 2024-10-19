using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : UIWindow
{
    [Header("References")]
    public TMP_Text sessionScoreText;
    public TMP_Text sessionCoinsText;
    public Button retryButton;
    public Button mainMenuButton;

    [Header("Settings")]
    public float timeBeforeRetry = 2f;
    
    private Coroutine gameOverRoutine;
    private bool canRetry;

    private void OnDestroy()
    {
        GameManager.Instance.onGameOver -= OnGameOver;
    }

    private void Start()
    {
        Close();

        GameManager.Instance.onGameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        // play game over sequence
        if (gameOverRoutine != null)
            StopCoroutine(gameOverRoutine);
        gameOverRoutine = StartCoroutine(GameOverUISequence());
    }

    private IEnumerator GameOverUISequence()
    {
        // hide retry button
        canRetry = false;
        retryButton.gameObject.SetActive(false);

        // update session UI data
        sessionCoinsText.text = GameManager.Instance.SessionCoinsScore.ToString();
        sessionScoreText.text = GameManager.Instance.SessionScore.ToString();

        // show game over UI
        // TODO: add animation
        Open();

        // wait for some time
        yield return new WaitForSeconds(timeBeforeRetry);

        // show retry button
        retryButton.gameObject.SetActive(true);
        canRetry = true;
    }

    public void Retry()
    {
        if (!canRetry)
            return;

        GameManager.Instance.ReloadLevel();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
