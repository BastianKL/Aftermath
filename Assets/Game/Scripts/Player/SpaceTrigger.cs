using UnityEngine;

public class SpaceTrigger : MonoBehaviour
{
    [SerializeField] private bool enableSpaceMode = true;

    private void OnTriggerEnter(Collider other)
    {
        var playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetSpaceMode(enableSpaceMode);
        }
    }
}