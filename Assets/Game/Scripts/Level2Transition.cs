using UnityEngine;

public class Level2Transition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] lights; // Assign your light GameObjects here
    [SerializeField] private BoxCollider transitionCollider; // Reference to the trigger collider

    private void Start()
    {
        if (transitionCollider != null)
            transitionCollider.enabled = false;
    }

    // Call this method whenever a light is turned off
    public void CheckLights()
    {
        if (lights == null || lights.Length == 0)
        {
            Debug.LogWarning("Lights array is not assigned in Level2Transition.");
            return;
        }

        int offCount = 0;

        foreach (var lightObj in lights)
        {
            if (lightObj == null) continue;

            var lightComp = lightObj.GetComponent<Light>();
            if (lightComp != null && !lightComp.enabled)
                offCount++;
        }

        if (offCount == lights.Length)
        {
            Debug.Log("All lights are off! Enabling collider.");
            if (transitionCollider != null)
                transitionCollider.enabled = true;
        }
    }
}
