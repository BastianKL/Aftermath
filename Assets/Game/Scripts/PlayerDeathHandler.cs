using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    public void OnPlayerDeath()
    {
        Debug.Log("Player died! Reloading scene...");
        Invoke(nameof(ReloadScene), 2f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}