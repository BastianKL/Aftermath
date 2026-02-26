using UnityEngine;
using TMPro;

public class CompletionSceneDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI completionTimeText;

    private void Awake()
    {
        if (completionTimeText == null)
        {
            completionTimeText = GameObject.Find("CompletionTimeText")?.GetComponent<TextMeshProUGUI>();
        }

        if (PlayerTimer.Instance != null && completionTimeText != null)
        {
            completionTimeText.text = $"Final Time: {PlayerTimer.Instance.GetFormattedTime()}";
        }
    }
}
