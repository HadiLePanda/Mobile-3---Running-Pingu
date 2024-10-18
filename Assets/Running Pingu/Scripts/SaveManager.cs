using UnityEngine;

public struct UserData
{
    public string username;
    public int highscore;
    public int coins;
}

public class SaveManager : MonoBehaviour
{
    public static string PREF_USERNAME = "Username";
    public static string PREF_HIGHSCORE = "Highscore";
    public static string PREF_COINS = "Coins";

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
    }

    public void SaveUsername(string username) => PlayerPrefs.SetString(PREF_USERNAME, username);
    public string LoadUsername() => PlayerPrefs.GetString(PREF_USERNAME, string.Empty);

    public void SaveHighscore(int value) => PlayerPrefs.SetInt(PREF_HIGHSCORE, value);
    public int LoadHighscore() => PlayerPrefs.GetInt(PREF_HIGHSCORE, 0);

    public void SaveCoins(int value) => PlayerPrefs.SetInt(PREF_COINS, value);
    public int LoadCoins() => PlayerPrefs.GetInt(PREF_COINS, 0);
}
