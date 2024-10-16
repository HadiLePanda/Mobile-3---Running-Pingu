using UnityEngine;

public class Coin : Pickup
{
    private const string ANIM_COLLECTED = "Collected";

    [Header("Header")]
    [SerializeField] private Animator anim;

    [Header("Settings")]
    [SerializeField] private int coinsValue = 1;

    private bool canPickUp = true;

    public override bool CanPickUp()
    {
        return canPickUp;
    }

    public override void PickUp()
    {
        // mark as can't pickup so we can do an animation before destroying the coin
        canPickUp = false;

        // add coins value
        GameManager.instance.AddCoinsScore(coinsValue);

        // play pickup animation
        anim.SetTrigger(ANIM_COLLECTED);

        // destroy pickup object
        Destroy(gameObject, 1f);
    }
}
