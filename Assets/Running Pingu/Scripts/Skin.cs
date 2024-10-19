using UnityEngine;

public class Skin : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    
    [Header("Settings")]
    //[SerializeField] private bool isUnlocked = false;
    [SerializeField] private SkinData data;

    public SkinData Data => data;

    //public bool SetUnlocked(bool state) => isUnlocked = state;
}
