using UnityEngine;
using UnityEngine.UI;

public class UISkinSelection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button nextSkinButton;
    [SerializeField] private Button previousSkinButton;

    private void Update()
    {
        if (panel.activeSelf)
        {
            nextSkinButton.interactable = Player.Instance.SkinController.IsThereNextSkin();
            previousSkinButton.interactable = Player.Instance.SkinController.IsTherePreviousSkin();
        }
    }

    public void SelectNextSkin()
    {
        Player.Instance.SkinController.SelectNextSkin();
    }

    public void SelectPreviousSkin()
    {
        Player.Instance.SkinController.SelectPreviousSkin();
    }
}
