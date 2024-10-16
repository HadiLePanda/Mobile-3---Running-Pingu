using UnityEngine;

public class Coin : Pickup
{
    private const string ANIM_COLLECTED = "Collected";
    private const string ANIM_SPAWN = "Spawn";

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Collider pickupCollider;

    [Header("Settings")]
    [SerializeField] private int coinsValue = 1;

    private bool canPickUp = true;

    public override bool CanPickUp() => canPickUp;

    // TODO: reset coins that have been disabled when spawning coins line

    private void OnEnable()
    {
        ResetPickup();
    }

    public void ResetPickup()
    {
        Appear();
    }

    public override void PickUp()
    {
        // mark as can't pickup so we can do an animation before destroying the coin
        canPickUp = false;

        // add coins value
        GameManager.Instance.AddCoinsScore(coinsValue);

        // play pickup animation
        anim.SetTrigger(ANIM_COLLECTED);

        // disappear after some time
        Invoke(nameof(Disappear), 1f);
    }

    private void Appear()
    {
        pickupCollider.enabled = true;
        gameObject.SetActive(true);
        anim.SetTrigger(ANIM_SPAWN);
        canPickUp = true;
    }

    private void Disappear()
    {
        pickupCollider.enabled = false;
        gameObject.SetActive(false);
    }
}
