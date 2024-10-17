using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera idleVirtualCam;
    [SerializeField] private CinemachineVirtualCamera gameplayVirtualCam;

    public static CameraController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SwitchToIdleCamera();
    }

    public void SwitchToIdleCamera()
    {
        CameraManager.SwitchCamera(idleVirtualCam);
    }

    public void SwitchToGameplayCamera()
    {
        CameraManager.SwitchCamera(gameplayVirtualCam);
    }
}
