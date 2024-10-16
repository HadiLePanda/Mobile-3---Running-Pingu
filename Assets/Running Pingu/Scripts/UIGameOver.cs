using TMPro;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TMP_Text sessionScoreText;
    public TMP_Text sessionCoinsText;

    private void OnDestroy()
    {
        GameManager.Instance.onGameOver -= OnGameOver;
    }

    private void Start()
    {
        panel.SetActive(false);

        GameManager.Instance.onGameOver += OnGameOver;
    }

    private void Update()
    {
        if (panel.activeSelf)
        {
            sessionCoinsText.text = GameManager.Instance.CoinsScore.ToString();
            sessionScoreText.text = GameManager.Instance.Score.ToString();
        }
    }

    private void OnGameOver()
    {
        OpenGameOverUI();
    }

    public void OpenGameOverUI()
    {
        // TODO: add animation
        panel.SetActive(true);
    }

    public void Retry()
    {
        GameManager.Instance.ReloadLevel();
    }
}
