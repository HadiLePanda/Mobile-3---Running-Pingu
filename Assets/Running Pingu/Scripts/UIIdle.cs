using UnityEngine;

public class UIIdle : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;

    private void Update()
    {
        panel.SetActive(GameManager.Instance.GameState == GameState.Idle);
    }
}
