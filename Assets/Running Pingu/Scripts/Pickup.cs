using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] protected AudioClip pickupSound;
    [SerializeField] protected ParticleSystem pickupEffect;

    public abstract bool CanPickUp();
    public abstract void PickUp();
}
