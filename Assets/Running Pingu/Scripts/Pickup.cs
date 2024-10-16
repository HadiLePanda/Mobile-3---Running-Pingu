using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public abstract bool CanPickUp();
    public abstract void PickUp();
}
