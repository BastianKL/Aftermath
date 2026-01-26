using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerTimer : MonoBehaviour
{
    public static PlayerTimer Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject completionUI;
    [SerializeField] private TextMeshProUGUI completionTimeText;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool hasCompleted = false;

    private void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (timerUI != null)
            timerUI.SetActive(false);
        if (completionUI != null)
            completionUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find UI references if they were lost during scene change
        if (timerText == null || timerUI == null)
        {
            FindUIReferences();
        }
    }

    private void FindUIReferences()
    {
        // Try to find timer UI in the new scene
        var foundTimerUI = GameObject.Find("TimerUI");
        if (foundTimerUI != null)
        {
            timerUI = foundTimerUI;
            timerText = foundTimerUI.GetComponentInChildren<TextMeshProUGUI>();
            if (isRunning && timerUI != null)
                timerUI.SetActive(true);
        }

        var foundCompletionUI = GameObject.Find("CompletionUI");
        if (foundCompletionUI != null)
        {
            completionUI = foundCompletionUI;
            completionTimeText = foundCompletionUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (isRunning && !hasCompleted)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        if (!isRunning && !hasCompleted)
        {
            isRunning = true;
            elapsedTime = 0f;
            if (timerUI != null)
                timerUI.SetActive(true);
            Debug.Log("Timer started!");
        }
    }

    public void StopTimer()
    {
        if (isRunning && !hasCompleted)
        {
            isRunning = false;
            hasCompleted = true;
            ShowCompletionTime();
            Debug.Log($"Timer stopped! Final time: {GetFormattedTime()}");
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime();
        }
    }

    private void ShowCompletionTime()
    {
        if (timerUI != null)
            timerUI.SetActive(false);

        if (completionUI != null)
        {
            completionUI.SetActive(true);
            if (completionTimeText != null)
            {
                completionTimeText.text = $"Final Time: {GetFormattedTime()}";
            }
        }
    }

    private string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public float GetElapsedTime() => elapsedTime;
    public bool IsRunning() => isRunning;
}