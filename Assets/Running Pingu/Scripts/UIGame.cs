using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TMP_Text scoreText;
    public TMP_Text coinsText;
    public TMP_Text modifierText;

    private void Update()
    {
        var isPlaying = GameManager.Instance.GameState == GameState.Playing;
        if (isPlaying)
        {
            scoreText.text = GameManager.Instance.SessionScore.ToString();
            coinsText.text = GameManager.Instance.SessionCoinsScore.ToString();
            modifierText.text = $"x{GameManager.Instance.DifficultyModifier:N1}";

            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
