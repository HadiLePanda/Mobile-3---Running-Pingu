using NiobiumStudios;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TMP_Text highscoreText;
    public TMP_Text coinsText;
    public TMP_InputField usernameInputField;
    public Button playButton;

    private void OnEnable()
    {
        GameManager.onDataLoaded += OnUserDataLoaded;
        DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }
    private void OnDisable()
    {
        GameManager.onDataLoaded -= OnUserDataLoaded;
        DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
    }

    private void OnUserDataLoaded()
    {
        UpdateUserDataUI();
    }
    private void OnClaimPrizeDailyRewards(int day)
    {
        UpdateUserDataUI();
    }

    private void Start()
    {
        // load the stored username if any
        var savedUsername = SaveManager.Instance.LoadUsername();
        if (!string.IsNullOrEmpty(savedUsername))
            usernameInputField.text = savedUsername;

        UpdateUserDataUI();
    }

    private void Update()
    {
        // make play button only interactable if user entered a valid username
        playButton.interactable = IsUsernameValid();

        // show the home menu only when in idle before running
        panel.SetActive(GameManager.Instance.GameState == GameState.Idle);
    }

    public void Play()
    {
        // check if username is valid
        if (!IsUsernameValid())
            return;

        // update the player username
        var username = usernameInputField.text;
        GameManager.Instance.SetUsername(username);

        // save entered username
        SaveManager.Instance.SaveUsername(username);

        // save selected skin
        Player.Instance.SkinController.SaveSelectedSkin();

        // start the game
        GameManager.Instance.StartRunning();
    }

    public void UpdateUserDataUI()
    {
        highscoreText.text = GameManager.Instance.UserData.highscore.ToString();
        coinsText.text = GameManager.Instance.UserData.coins.ToString();
    }

    private bool IsUsernameValid()
    {
        return usernameInputField.text.Length > 2;
    }
}
