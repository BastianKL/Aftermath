using System.Collections;
using UnityEngine;

public class CinematicCameraZoom : MonoBehaviour
{
    [Header("Camera Positions")]
    [SerializeField] private Transform tvPosition;
    [SerializeField] private Transform playerEyesPosition;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomDuration = 3f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Player Control")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool disablePlayerDuringZoom = true;

    [Header("Tutorial")]
    [SerializeField] private SequentialTutorial sequentialTutorial;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;

        if (tvPosition != null && _camera != null)
        {
            _camera.transform.position = tvPosition.position;
            _camera.transform.rotation = tvPosition.rotation;
        }

        if (disablePlayerDuringZoom && playerMovement != null)
        {
            playerMovement.SetControlsEnabled(false);
        }
    }

    public void StartZoomToPlayer()
    {
        StartCoroutine(ZoomToPlayerEyes());
    }

    private IEnumerator ZoomToPlayerEyes()
    {
        if (_camera == null || playerEyesPosition == null) yield break;

        Vector3 startPos = _camera.transform.position;
        Quaternion startRot = _camera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            float curveT = zoomCurve.Evaluate(t);

            _camera.transform.position = Vector3.Lerp(startPos, playerEyesPosition.position, curveT);
            _camera.transform.rotation = Quaternion.Slerp(startRot, playerEyesPosition.rotation, curveT);

            yield return null;
        }

        _camera.transform.position = playerEyesPosition.position;
        _camera.transform.rotation = playerEyesPosition.rotation;

        // Enable player controls
        if (playerMovement != null)
        {
            playerMovement.SetControlsEnabled(true);
        }

        // Make camera follow player
        _camera.transform.SetParent(playerEyesPosition);

        // Start tutorial after zoom complete
        if (sequentialTutorial != null)
        {
            sequentialTutorial.StartTutorial();
        }
    }
}