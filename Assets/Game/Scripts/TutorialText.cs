using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showOnlyOnce = true;

    [Header("Tutorial")]
    [TextArea(2, 5)]
    [SerializeField] private string tutorialMessage = "Press W A S D to move";
    [SerializeField] private float displayDuration = 3f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialTextField;
    [SerializeField] private GameObject tutorialPanel;

    private bool hasShown = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (tutorialTextField != null)
            tutorialTextField.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && (!hasShown || !showOnlyOnce))
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowTutorial());
        }
    }

    private IEnumerator ShowTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        hasShown = true;

        if (tutorialTextField != null)
            tutorialTextField.text = tutorialMessage;

        yield return new WaitForSeconds(displayDuration);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (tutorialTextField != null)
            tutorialTextField.text = "";
    }
}
