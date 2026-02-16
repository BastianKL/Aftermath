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
        int offCount = 0;
        foreach (var lightObj in lights)
        {
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
