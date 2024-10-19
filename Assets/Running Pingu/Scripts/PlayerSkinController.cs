using System.Linq;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform playerModelRoot;

    private Skin[] skins;
    private Skin selectedSkin;
    private int selectedSkinIndex;

    private void Start()
    {
        skins = playerModelRoot.GetComponentsInChildren<Skin>(true);

        LoadSelectedSkin();
    }

    public void LoadSelectedSkin()
    {
        var savedSkinId = SaveManager.Instance.LoadSelectedSkinId();

        if (!string.IsNullOrEmpty(savedSkinId))
        {
            // select the saved skin
            SelectSkinById(savedSkinId);
        }
        else
        {
            // select the first skin by default
            var firstSkinId = skins[0].Data.id;
            SelectSkinById(firstSkinId);
        }
    }

    public void SaveSelectedSkin()
    {
        string selectedSkinId = selectedSkin ? selectedSkin.Data.id.ToString() : string.Empty;
        SaveManager.Instance.SaveSelectedSkin(selectedSkinId);
        PlayerPrefs.Save();
    }

    public void SelectNextSkin()
    {
        // get the index of the next skin to select
        // wrap around to first skin if we go above last skin
        var skinIndex = IsThereNextSkin() ? selectedSkinIndex + 1 : 0;
        SelectSkin(skins[skinIndex]);
    }
    public void SelectPreviousSkin()
    {
        // get the index of the previous skin to select
        // wrap around to last skin if below 0
        var skinIndex = IsTherePreviousSkin() ? selectedSkinIndex - 1 : skins.Length - 1;
        SelectSkin(skins[skinIndex]);
    }

    public bool IsThereNextSkin() => selectedSkinIndex + 1 <= skins.Length - 1;
    public bool IsTherePreviousSkin() => selectedSkinIndex - 1 >= 0;

    public void SelectSkinById(string skinId)
    {
        if (!IsSkinUnlocked(skinId))
            return;

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].Data.id == skinId)
            {
                skins[i].gameObject.SetActive(true);
                selectedSkin = skins[i];
                selectedSkinIndex = i;

                // update the player animator to the new character's animator
                player.anim = selectedSkin.anim;
            }
            else
            {
                skins[i].gameObject.SetActive(false);
            }
        }

        // just in case there was no skin loaded, use the default animator as fallback
        if (player.anim == null)
            player.anim = player.defaultAnimator;
    }

    private void SelectSkin(Skin newSkin)
    {
        SelectSkinById(newSkin.Data.id);
    }

    public Skin GetSkinById(string skinId)
    {
        return skins.FirstOrDefault(x => x.Data.id == skinId);
    }

    public bool IsSkinUnlocked(string skinId)
    {
        // TODO: if we add the ability to unlock skins
        // we'll check in the player data to see if the skin is part of their unlocked skins.
        // For now all skins are unlocked
        return true;
    }
}
