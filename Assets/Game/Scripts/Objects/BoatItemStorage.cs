using UnityEngine;
using System.Collections.Generic;

public class BoatItemStorage : MonoBehaviour
{
    public Transform itemArea; // Assign in inspector
    public int maxItems = 8;
    public Vector3 areaSize = new Vector3(1.5f, 0.5f, 2f);

    private List<PickupItem> storedItems = new List<PickupItem>();

    public bool CanStoreItem() => storedItems.Count < maxItems;

    public bool StoreItem(PickupItem item)
    {
        if (!CanStoreItem()) return false;

        storedItems.Add(item);

        // Place item at random position within area
        Vector3 localPos = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(0, areaSize.y),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
        item.transform.SetParent(itemArea, true);
        item.transform.localPosition = localPos;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;

        // Make the item "stick" to the boat
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Disable all floaters so the item doesn't float away
        foreach (var floater in item.GetComponentsInChildren<Floater>())
            floater.enabled = false;

        // Keep colliders enabled for interaction
        foreach (var col in item.GetComponentsInChildren<Collider>())
            col.enabled = true;

        // Remove from BoatItemMover if present
        var mover = GetComponentInParent<BoatItemMover>();
        if (mover != null && rb != null)
        {
            mover.RemoveItemRigidbody(rb);
        }

        item.SetHeld(false);
        return true;
    }

    public void RemoveItem(PickupItem item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            item.transform.SetParent(null, true);

            // Re-enable floaters so the item floats again
            foreach (var floater in item.GetComponentsInChildren<Floater>())
                floater.enabled = true;

            foreach (var col in item.GetComponentsInChildren<Collider>())
                col.enabled = true;

            var rb = item.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }

    public List<PickupItem> GetStoredItems() => storedItems;
}
