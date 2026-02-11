using UnityEngine;
using UnityEngine.Events;

public class PlantingManager : MonoBehaviour
{
    public static PlantingManager Instance { get; private set; }

    public int SeedsPlanted { get; private set; }
    public int SaplingsPlanted { get; private set; }

    public UnityEvent onThresholdReached; // Add this

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlanting(PlantingSpot.SpotType type)
    {
        if (type == PlantingSpot.SpotType.Seed)
            SeedsPlanted++;
        else if (type == PlantingSpot.SpotType.Sapling)
            SaplingsPlanted++;

        if (SeedsPlanted >= 5 && SaplingsPlanted >= 1)
        {
            if (onThresholdReached != null)
                onThresholdReached.Invoke();
        }
    }
}
