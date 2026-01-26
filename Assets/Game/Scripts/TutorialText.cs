using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float delayBeforeShow = 2f; // Wait 2 seconds in trigger
    [SerializeField] private float displayDuration = 5f; // Show for 5 seconds
    [SerializeField] private bool showOnlyOnce = true;

    [Header("Text")]
    [TextArea(3, 10)]
    [SerializeField] private string tutorialMessage = "Press W A S D to move";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialTextField; // Assign in Inspector
    [SerializeField] private GameObject tutorialPanel; // Optional panel background

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool hasShown = false;
    private bool playerInTrigger = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (tutorialTextField != null)
        {
            Color c = tutorialTextField.color;
            c.a = 0;
            tutorialTextField.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && (!hasShown || !showOnlyOnce))
        {
            playerInTrigger = true;
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowTutorialAfterDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = false;
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(HideTutorial());
        }
    }

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        if (!playerInTrigger) yield break; // Player left before delay finished

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        if (tutorialTextField != null)
        {
            tutorialTextField.text = tutorialMessage;
            yield return StartCoroutine(FadeText(0, 1));
        }

        hasShown = true;

        // Auto-hide after duration
        yield return new WaitForSeconds(displayDuration);

        if (playerInTrigger) // Only auto-hide if still in trigger
        {
            yield return StartCoroutine(HideTutorial());
        }
    }

    private IEnumerator HideTutorial()
    {
        if (tutorialTextField != null)
        {
            yield return StartCoroutine(FadeText(1, 0));
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        if (tutorialTextField == null) yield break;

        float elapsed = 0f;
        Color c = tutorialTextField.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            tutorialTextField.color = c;
            yield return null;
        }

        c.a = endAlpha;
        tutorialTextField.color = c;
    }
}