using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private MonoBehaviour doorScript; // Assign DoorRotater or DoorSlider
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Try both door types
            if (doorScript is DoorRotater rotater)
            {
                rotater.RequestOpen();
            }
            else if (doorScript is DoorSlider slider)
            {
                slider.RequestOpen();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (doorScript is DoorRotater rotater)
            {
                rotater.RequestClose();
            }
            else if (doorScript is DoorSlider slider)
            {
                slider.RequestClose();
            }
        }
    }
}