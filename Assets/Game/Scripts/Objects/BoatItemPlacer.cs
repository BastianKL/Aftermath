using UnityEngine;

public class BoatItemPlacer : MonoBehaviour, Interactable
{
    private BoatItemStorage storage;
    private PlayerMovement player;

    void Awake()
    {
        storage = GetComponent<BoatItemStorage>();
    }

    public void Interact()
    {
        if (player == null)
            player = FindObjectOfType<PlayerMovement>();

        var heldItemObj = player.GetHeldItem();
        if (heldItemObj == null) return;

        var pickup = heldItemObj.GetComponentInParent<PickupItem>();

        if (pickup != null && storage.CanStoreItem())
        {
            player.RemoveHeldItemReference(pickup); // <-- Only remove reference, don't destroy!
            storage.StoreItem(pickup);
        }
    }
}
