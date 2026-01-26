using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinematicCameraZoom cameraZoom;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // Small delay to ensure everything is loaded
        yield return new WaitForSeconds(0.5f);

        // Start camera zoom from TV to player
        if (cameraZoom != null)
        {
            Debug.Log("Starting camera zoom!");
            cameraZoom.StartZoomToPlayer();
        }
        else
        {
            Debug.LogError("CinematicCameraZoom reference is missing!");
        }
    }
}