using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [Header("Window References")]
    public GameObject panel;

    public virtual void Open()
    {
        panel.SetActive(true);
    }

    public virtual void Close()
    {
        panel.SetActive(false);
    }
}
