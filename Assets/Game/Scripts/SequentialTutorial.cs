using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SequentialTutorial : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Tutorial Steps")]
    [SerializeField] private string lookMessage = "Move Camera using Mouse";
    [SerializeField] private string moveMessage = "Move with WASD";
    [SerializeField] private float displayDuration = 3f;

    [Header("Detection Settings")]
    [SerializeField] private float mouseSensitivityThreshold = 5f;
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool lookTutorialComplete = false;
    private bool moveTutorialComplete = false;
    private bool tutorialActive = false;
    private float totalMouseMovement = 0f;

    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (tutorialText != null)
        {
            Color c = tutorialText.color;
            c.a = 0;
            tutorialText.color = c;
        }
    }

    public void StartTutorial()
    {
        // Activate the panel FIRST so coroutines can run
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        gameObject.SetActive(true); // Make sure this GameObject is active too
        tutorialActive = true;
        StartCoroutine(ShowLookTutorial());
    }

    private void Update()
    {
        if (!tutorialActive) return;

        // Check for mouse movement
        if (!lookTutorialComplete)
        {
            if (Mouse.current != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                totalMouseMovement += mouseDelta.magnitude;

                if (totalMouseMovement >= mouseSensitivityThreshold)
                {
                    lookTutorialComplete = true;
                    StartCoroutine(TransitionToMoveTutorial());
                }
            }
        }
        // Check for WASD movement
        else if (!moveTutorialComplete)
        {
            if (Keyboard.current != null)
            {
                bool wPressed = Keyboard.current.wKey.isPressed;
                bool aPressed = Keyboard.current.aKey.isPressed;
                bool sPressed = Keyboard.current.sKey.isPressed;
                bool dPressed = Keyboard.current.dKey.isPressed;

                if (wPressed || aPressed || sPressed || dPressed)
                {
                    moveTutorialComplete = true;
                    StartCoroutine(CompleteTutorial());
                }
            }
        }
    }

    private IEnumerator ShowLookTutorial()
    {
        if (tutorialText != null)
        {
            tutorialText.text = lookMessage;
            yield return StartCoroutine(FadeText(0, 1));
        }
    }

    private IEnumerator TransitionToMoveTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeText(1, 0));

        if (tutorialText != null)
        {
            tutorialText.text = moveMessage;
            yield return StartCoroutine(FadeText(0, 1));
        }
    }

    private IEnumerator CompleteTutorial()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeText(1, 0));

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        tutorialActive = false;
        Debug.Log("Tutorial complete!");
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        if (tutorialText == null) yield break;

        float elapsed = 0f;
        Color c = tutorialText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            tutorialText.color = c;
            yield return null;
        }

        c.a = endAlpha;
        tutorialText.color = c;
    }
}