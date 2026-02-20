using UnityEngine;
using System.Collections.Generic;

public class BoatCrateRack : MonoBehaviour
{
    [SerializeField] private Transform[] slots = new Transform[4];
    [SerializeField] private int maxCrates = 4;
    [SerializeField] private string storedCrateLayerName = "StoredCrate";
    [SerializeField] private bool disableFloaters = true;

    private PickupItem[] stored;
    private int storedLayer;
    private Dictionary<PickupItem, int> originalLayers = new Dictionary<PickupItem, int>();

    void Awake()
    {
        int n = Mathf.Clamp(maxCrates, 1, 4);
        stored = new PickupItem[n];
        storedLayer = LayerMask.NameToLayer(storedCrateLayerName);
    }

    public void TryStoreFromPlayer(PlayerMovement player)
    {
        if (player == null) return;

        var heldObj = player.GetHeldItem();
        if (heldObj == null) return;

        var item = heldObj.GetComponentInParent<PickupItem>();
        if (item == null) return;

        int slot = FindFreeSlot();
        if (slot == -1) return;
        if (slot >= slots.Length || slots[slot] == null) return;

        player.RemoveHeldItemReference(item);
        item.SetHeld(false);

        if (!originalLayers.ContainsKey(item))
            originalLayers[item] = item.gameObject.layer;

        stored[slot] = item;

        item.IsStoredOnBoat = true;
        item.StoredRack = this;
        item.StoredSlot = slot;

        item.transform.SetParent(null, true);

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        foreach (var col in item.GetComponentsInChildren<Collider>())
        {
            col.enabled = true;     // så raycast kan ramme den
            col.isTrigger = true;  // så den ikke presser båden
        }

        if (disableFloaters)
        {
            foreach (var floater in item.GetComponentsInChildren<Floater>())
                floater.enabled = false;
        }

 

        item.transform.SetParent(slots[slot], false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
    }

    public void UnstoreLast()
    {
        int slot = FindLastFilledSlot();
        if (slot == -1) return;
        Unstore(slot);
    }

    public void Unstore(int slot)
    {
        if (slot < 0 || slot >= stored.Length) return;

        var item = stored[slot];
        if (item == null) return;

        stored[slot] = null;

        item.IsStoredOnBoat = false;
        item.StoredRack = null;
        item.StoredSlot = -1;

        item.transform.SetParent(null, true);

        if (slot < slots.Length && slots[slot] != null)
            item.transform.position = slots[slot].position + transform.right * 0.6f + Vector3.up * 0.2f;

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        foreach (var col in item.GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        if (disableFloaters)
        {
            foreach (var floater in item.GetComponentsInChildren<Floater>())
                floater.enabled = true;
        }
    }

    int FindFreeSlot()
    {
        for (int i = 0; i < stored.Length; i++)
            if (stored[i] == null) return i;
        return -1;
    }

    int FindLastFilledSlot()
    {
        for (int i = stored.Length - 1; i >= 0; i--)
            if (stored[i] != null) return i;
        return -1;
    }

    void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursive(t.gameObject, layer);
    }
}