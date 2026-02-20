using UnityEngine;
using System.Collections.Generic;

public class BoatItemMover : MonoBehaviour
{
    public List<Rigidbody> itemsOnBoat = new List<Rigidbody>();
    private Vector3 lastBoatPosition;
    public Transform boatRoot; // Assign the boat's root transform in Inspector

    void Start()
    {
        if (boatRoot == null)
            boatRoot = transform.parent;
        lastBoatPosition = boatRoot.position;
    }

    void FixedUpdate()
    {
        Vector3 delta = boatRoot.position - lastBoatPosition;
        foreach (var rb in itemsOnBoat)
        {
            // Only move items that are not parented to the boat and are not kinematic
            if (rb != null && !rb.isKinematic && rb.transform.parent != boatRoot)
            {
                rb.MovePosition(rb.position + delta);
            }
        }
        lastBoatPosition = boatRoot.position;
    }

    void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && other.GetComponent<PickupItem>() != null && !itemsOnBoat.Contains(rb))
        {
            itemsOnBoat.Add(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null)
        {
            itemsOnBoat.Remove(rb);

            // If this is a PickupItem, re-enable floaters and set kinematic to false
            var pickup = other.GetComponent<PickupItem>();
            if (pickup != null)
            {
                foreach (var floater in pickup.GetComponentsInChildren<Floater>())
                    floater.enabled = true;
                rb.isKinematic = false;
            }
        }
    }

    // Helper for BoatItemStorage to remove rigidbodies
    public void RemoveItemRigidbody(Rigidbody rb)
    {
        if (rb != null)
            itemsOnBoat.Remove(rb);
    }
}
