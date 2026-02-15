using UnityEngine;
using UnityEngine.Events;

public class Level2Transition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] lights; // Assign your light GameObjects here
    [SerializeField] private LevelTransition levelTransition; // Reference to LevelTransition

    [Header("Events")]
    public UnityEvent onAllLightsOff;

    public bool canTransition = false;

    private int lightsOffCount = 0;

    public void LightTurnedOff()
    {
        lightsOffCount++;
        Debug.Log("Light turned off! Count: " + lightsOffCount);
        if (lightsOffCount == 4 && canTransition)
        {
            onAllLightsOff.Invoke();
            if (levelTransition != null)
            {
                levelTransition.StartCoroutine("TransitionToNextLevel");
            }
        }
    }

    // Optional: Reset the counter if needed
    public void ResetLights()
    {
        lightsOffCount = 0;
    }

    public void EnableTransition()
    {
        canTransition = true;
    }

}
