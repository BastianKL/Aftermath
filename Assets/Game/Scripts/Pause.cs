using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public string MenuSceneName = "Menu";
    public GameObject settingsPanel;
    public GameObject creditsPanel; // Assign in Inspector
    public PlayerMovement playerMovement;
    public InputActionReference pauseAction; // Assign in Inspector

    private bool isPaused = false;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPausePressed;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePressed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == MenuSceneName)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (playerMovement != null)
            playerMovement.SetControlsEnabled(true);
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (playerMovement != null)
            playerMovement.SetControlsEnabled(false);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public void QuitToDesktop()
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
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }
}
