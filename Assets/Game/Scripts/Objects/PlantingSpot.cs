using UnityEngine;

public class PlantingSpot : MonoBehaviour, Interactable
{
    public enum SpotType { Seed, Sapling }
    public SpotType spotType;
    public GameObject plantedPrefab; // Prefab to instantiate when planted

    private bool isOccupied = false;

    public void Interact()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player == null || isOccupied) return;

        if (spotType == SpotType.Seed && player.HasSeed())
        {
            player.RemoveSeed();
            Instantiate(plantedPrefab, transform.position, transform.rotation);
            isOccupied = true;
            PlantingManager.Instance?.RegisterPlanting(spotType);
        }
        else if (spotType == SpotType.Sapling)
        {
            var heldItem = player.GetHeldItem();
            if (heldItem != null && heldItem.CompareTag("Sapling"))
            {
                player.RemoveHeldItem();
                Instantiate(plantedPrefab, transform.position, transform.rotation);
                isOccupied = true;
                PlantingManager.Instance?.RegisterPlanting(spotType);
            }
        }
    }
}
