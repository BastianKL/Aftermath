using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinematicCameraZoom cameraZoom;
    [SerializeField] private SequentialTutorial sequentialTutorial; // Add this reference

    [Header("Timing")]
    [SerializeField] private float zoomDuration = 2.0f; // Set this to your actual zoom time

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (cameraZoom != null)
        {
            Debug.Log("Starting camera zoom!");
            cameraZoom.StartZoomToPlayer();

            // Wait for the zoom to finish
            yield return new WaitForSeconds(zoomDuration);

            // Start the tutorial
            if (sequentialTutorial != null)
            {
                sequentialTutorial.StartTutorial();
            }
            else
            {
                Debug.LogError("SequentialTutorial reference is missing!");
            }
        }
        else
        {
            Debug.LogError("CinematicCameraZoom reference is missing!");
        }
    }
}
