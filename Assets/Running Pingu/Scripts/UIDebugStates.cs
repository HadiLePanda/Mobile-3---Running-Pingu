using TMPro;
using UnityEngine;

public class UIDebugStates : MonoBehaviour
{
    [Header("References")]
    public TMP_Text groundedText;
    public TMP_Text playerState;
    public TMP_Text movementState;
    public TMP_Text gameState;
    public TMP_Text speedText;

    private void Update()
    {
        groundedText.text = "IsGrounded: " + Player.Instance.Controller.IsGrounded;
        playerState.text = "Player: " + Player.Instance.State.ToString();
        movementState.text = "Movement: " + Player.Instance.Controller.MovementState.ToString();
        gameState.text = "Game: " + GameManager.Instance.GameState.ToString();
        speedText.text = "Speed: " + Player.Instance.Controller.Speed.ToString("N1");
    }
}
