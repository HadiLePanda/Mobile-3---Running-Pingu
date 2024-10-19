using UnityEngine;

public struct UserData
{
    public string username;
    public int highscore;
    public int coins;
}

public class SaveManager : MonoBehaviour
{
    public static readonly string PREF_USERNAME = "Username";
    public static readonly string PREF_HIGHSCORE = "Highscore";
    public static readonly string PREF_COINS = "Coins";
    public static readonly string PREF_SKIN_SELECTED_ID = "SelectedSkinId";

    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public UserData LoadUserData()
    {
        var data = new UserData
        {
            username = LoadUsername(),
            highscore = LoadHighscore(),
            coins = LoadCoins(),
        };

        return data;
    }

    public void SaveUserData(UserData data)
    {
        SaveUsername(data.username);
        SaveHighscore(data.highscore);
        SaveCoins(data.coins);

        PlayerPrefs.Save();
    }

    public string LoadUsername() => PlayerPrefs.GetString(PREF_USERNAME, string.Empty);
    public void SaveUsername(string username) => PlayerPrefs.SetString(PREF_USERNAME, username);

    public int LoadHighscore() => PlayerPrefs.GetInt(PREF_HIGHSCORE, 0);
    public void SaveHighscore(int value) => PlayerPrefs.SetInt(PREF_HIGHSCORE, value);

    public int LoadCoins() => PlayerPrefs.GetInt(PREF_COINS, 0);
    public void SaveCoins(int value) => PlayerPrefs.SetInt(PREF_COINS, value);

    public string LoadSelectedSkinId() => PlayerPrefs.GetString(PREF_SKIN_SELECTED_ID, string.Empty);
    public void SaveSelectedSkin(string skinId) => PlayerPrefs.SetString(PREF_SKIN_SELECTED_ID, skinId);
}
