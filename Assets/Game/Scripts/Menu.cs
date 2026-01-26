using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Scene")]
    public GameObject loadingscreen;
    public string sceneName;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public GameObject VideoRenderer;

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject pauseMenu;

    [Header("Intro Sequence")]
    [SerializeField] private float silenceDuration = 3f;
    [SerializeField] private Image fadeInImage;
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float imageHoldDuration = 2f;

    public GameObject creditsPanel;
    public GameObject previousMenu;

    void Start()
    {
        // Completely disable video at start
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }

        if (VideoRenderer != null)
        {
            VideoRenderer.SetActive(false);
        }

        // Disable loading screen
        if (loadingscreen != null)
        {
            loadingscreen.SetActive(false);
        }

        // Setup cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Setup fade image
        if (fadeInImage != null)
        {
            Color c = fadeInImage.color;
            c.a = 0;
            fadeInImage.color = c;
            fadeInImage.gameObject.SetActive(false);
        }

        // Setup black overlay
        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = 0;
            blackOverlay.color = c;
            blackOverlay.gameObject.SetActive(false);
        }
    }

    public void playGame()
    {
        StartCoroutine(PlayCinematicIntro());
    }

    private IEnumerator PlayCinematicIntro()
    {
        // 1. Fade to black (0 to 100)
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                Color c = blackOverlay.color;
                c.a = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
                blackOverlay.color = c;
                yield return null;
            }
            Color finalColor = blackOverlay.color;
            finalColor.a = 1;
            blackOverlay.color = finalColor;
        }

        // 2. Black screen silence
        yield return new WaitForSeconds(silenceDuration);

        // 3. Fade in video image ON TOP of black (black stays at 100)
        if (fadeInImage != null)
        {
            fadeInImage.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                Color c = fadeInImage.color;
                c.a = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
                fadeInImage.color = c;
                yield return null;
            }
            Color finalColor = fadeInImage.color;
            finalColor.a = 1;
            fadeInImage.color = finalColor;
        }

        // 4. Hold image
        yield return new WaitForSeconds(imageHoldDuration);

        // 5. Prepare video while image is still visible
        yield return StartCoroutine(PlayPrologueAndLoad());
    }

    private IEnumerator PlayPrologueAndLoad()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is not assigned!");
            yield break;
        }

        // Activate video renderer behind everything
        if (VideoRenderer != null)
        {
            VideoRenderer.SetActive(true);
        }

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.playOnAwake = false;
        videoPlayer.SetDirectAudioVolume(0, 1);

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Start playing video (behind image and black)
        videoPlayer.Play();

        // Small delay to ensure first frame is rendered
        yield return new WaitForSeconds(0.1f);

        // Instantly hide image and black overlay to reveal video (no fade)
        if (fadeInImage != null)
        {
            fadeInImage.gameObject.SetActive(false);
        }

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(false);
        }

        // Wait for video to finish
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        videoPlayer.gameObject.SetActive(false);

        if (VideoRenderer != null)
        {
            VideoRenderer.SetActive(false);
        }

        if (loadingscreen != null)
        {
            loadingscreen.SetActive(true);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        pauseMenu.SetActive(false); // or main menu panel
    }
    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        pauseMenu.SetActive(true); // or main menu panel
    }
}