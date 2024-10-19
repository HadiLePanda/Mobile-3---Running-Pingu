using NiobiumStudios;
using UnityEngine;

public class DailyRewardsManager : MonoBehaviour
{
    public static DailyRewardsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }

    private void OnDisable()
    {
        DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
    }

    public void OnClaimPrizeDailyRewards(int day)
    {
        // get the claimed reward
        Reward claimedReward = DailyRewards.instance.GetReward(day);

        if (claimedReward.unit == "Coins")
        {
            // add coins
            var newCoins = GameManager.Instance.UserData.coins + claimedReward.reward;
            GameManager.Instance.SetCoins(newCoins);
            SaveManager.Instance.SaveCoins(newCoins);

            Debug.Log($"Claimed {claimedReward.reward} coins");
        }
    }
}
