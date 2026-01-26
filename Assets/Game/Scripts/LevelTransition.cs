using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private string playerTag = "Player";

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Video Settings (Optional)")]
    [SerializeField] private bool playVideo = false;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoRenderer;

    [Header("Loading Screen (Optional)")]
    [SerializeField] private GameObject loadingScreen;

    [Header("Player Control")]
    [SerializeField] private bool freezePlayer = true;

    private bool hasTriggered = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }

        if (videoRenderer != null)
        {
            videoRenderer.SetActive(false);
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            hasTriggered = true;

            // Freeze player movement
            if (freezePlayer)
            {
                var playerMovement = other.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.SetControlsEnabled(false);
                }
            }

            StartCoroutine(TransitionToNextLevel());
        }
    }

    private IEnumerator TransitionToNextLevel()
    {
        // Show cursor for video watching
        if (playVideo)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Play video if enabled
        if (playVideo && videoPlayer != null)
        {
            yield return StartCoroutine(PlayTransitionVideo());
        }

        // Show loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1;
        fadeImage.color = c;
    }

    private IEnumerator PlayTransitionVideo()
    {
        if (videoRenderer != null)
        {
            videoRenderer.SetActive(true);
        }

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();

        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        videoPlayer.gameObject.SetActive(false);

        if (videoRenderer != null)
        {
            videoRenderer.SetActive(false);
        }
    }
}