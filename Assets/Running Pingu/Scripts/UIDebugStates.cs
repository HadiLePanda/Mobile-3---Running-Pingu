using TMPro;
using UnityEngine;

public class UIDebugStates : MonoBehaviour
{
    [Header("References")]
    public TMP_Text playerState;
    public TMP_Text movementState;
    public TMP_Text gameState;
    public TMP_Text speedText;

    private void Update()
    {
        playerState.text = "Player: " + Player.instance.State.ToString();
        movementState.text = "Movement: " + Player.instance.Controller.MovementState.ToString();
        gameState.text = "Game: " + GameManager.instance.GameState.ToString();
        speedText.text = "Speed: " + Player.instance.Controller.Speed.ToString("N1");
    }
}
