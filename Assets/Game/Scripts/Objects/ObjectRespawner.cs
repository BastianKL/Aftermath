using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform spawnPoint;

    private GameObject currentInstance;

    public void RespawnObject()
    {
        if (currentInstance != null)
        {
            currentInstance.SetActive(false);
            Destroy(currentInstance);
        }

        if (objectPrefab != null && spawnPoint != null)
        {
            currentInstance = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
