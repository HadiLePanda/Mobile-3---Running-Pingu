using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Settings")]
    public int maxCoin = 5;
    public float changeToSpawn = 0.5f;
    public bool forceSpawnAll = false;

    private GameObject[] coins;

    private void Awake()
    {
        // store reference of child coins
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            coins[i] = transform.GetChild(i).gameObject;
        }

        // disable all the coins
        foreach (GameObject go in coins)
            go.SetActive(false);
    }

    private void OnEnable()
    {
        if (Random.Range(0f, 1f) > changeToSpawn)
            return;

        // force spawn max coins
        if (forceSpawnAll)
        {
            for (int i = 0; i < maxCoin; i++)
                coins[i].SetActive(true);
        }
        // spawn a random amount of coins
        else
        {
            if (coins.Length < 1)
                return;

            int randomCoinsAmount = Random.Range(1, maxCoin);
            for (int i = 0; i < randomCoinsAmount; i++)
                coins[i].SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach(GameObject go in coins)
            go.SetActive(false);
    }
}
