using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    // Reference to the prefab to respawn (assign in Inspector)
    public GameObject objectPrefab;

    // Reference to the spawn point (assign a Transform in Inspector)
    public Transform spawnPoint;

    // Internal reference to the current instance
    private GameObject currentInstance;

    // Call this from your button event
    public void RespawnObject()
    {
        // Disable the current instance if it exists
        if (currentInstance != null)
        {
            currentInstance.SetActive(false);
            Destroy(currentInstance);
        }

        // Instantiate a new object from the prefab at the spawn point
        if (objectPrefab != null && spawnPoint != null)
        {
            currentInstance = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
