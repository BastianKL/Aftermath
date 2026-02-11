using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueEntry
{
    [TextArea(2, 5)]
    public string dialogueText;
    public AudioClip dialogueAudio;
    public float displayDuration = 3f;
}

public class DialogueTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showOnlyOnce = true;

    [Header("Dialogue Sequence")]
    [SerializeField] private DialogueEntry[] dialogueEntries;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueTextField;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Assign in Inspector

    private bool hasShown = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueTextField != null)
            dialogueTextField.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && (!hasShown || !showOnlyOnce))
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(PlayDialogueSequence());
        }
    }

    private IEnumerator PlayDialogueSequence()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        hasShown = true;

        foreach (var entry in dialogueEntries)
        {
            if (dialogueTextField != null)
                dialogueTextField.text = entry.dialogueText;

            if (audioSource != null && entry.dialogueAudio != null)
            {
                audioSource.clip = entry.dialogueAudio;
                audioSource.Play();
            }

            yield return new WaitForSeconds(entry.displayDuration);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueTextField != null)
            dialogueTextField.text = "";
    }
}
