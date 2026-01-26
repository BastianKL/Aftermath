using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public PlayerMovement playerMovement; // Assign in Inspector

    public void SaveGame()
    {
        if (playerMovement != null)
        {
            Vector3 pos = playerMovement.transform.position;
            PlayerPrefs.SetFloat("PlayerX", pos.x);
            PlayerPrefs.SetFloat("PlayerY", pos.y);
            PlayerPrefs.SetFloat("PlayerZ", pos.z);
            PlayerPrefs.Save();
        }
    }

    public void LoadGame()
    {
        if (playerMovement != null && PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            playerMovement.transform.position = new Vector3(x, y, z);
        }
    }
}
