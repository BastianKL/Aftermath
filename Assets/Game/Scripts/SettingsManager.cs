using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject previousMenu;
    public AudioMixer audioMixer;
    public GameObject pauseMenu;

    // Player and UI references
    public PlayerMovement playerMovement;
    public PlayerStamina playerStamina;
    public PlayerHealth playerHealth;
    public GameObject subtitlesUI;
    public GameObject[] tutorialUI;
    public GameObject musicToastUI;

    // Volume sliders
    public Slider masterVolumeSlider;
    public Slider ambientVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    void Start()
    {
        float volume;

        if (audioMixer.GetFloat("MasterVolume", out volume) && masterVolumeSlider != null)
            masterVolumeSlider.value = Mathf.Pow(10, volume / 20);
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        if (audioMixer.GetFloat("AmbientVolume", out volume) && ambientVolumeSlider != null)
            ambientVolumeSlider.value = Mathf.Pow(10, volume / 20);
        if (ambientVolumeSlider != null)
            ambientVolumeSlider.onValueChanged.AddListener(SetAmbientVolume);

        if (audioMixer.GetFloat("SFXVolume", out volume) && sfxVolumeSlider != null)
            sfxVolumeSlider.value = Mathf.Pow(10, volume / 20);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        if (audioMixer.GetFloat("MusicVolume", out volume) && musicVolumeSlider != null)
            musicVolumeSlider.value = Mathf.Pow(10, volume / 20);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (previousMenu != null)
            previousMenu.SetActive(true);
    }

    // General
    public void SetViewBobbing(bool enabled)
    {
        if (playerMovement != null)
            playerMovement.SetViewBobbing(enabled);
    }

    public void SetStaminaEnabled(bool enabled)
    {
        if (playerStamina != null)
            playerStamina.SetEnabled(enabled);
    }

    public void SetHealthEnabled(bool enabled)
    {
        if (playerHealth != null)
            playerHealth.SetEnabled(enabled);
    }

    // UI
    public void SetSubtitlesEnabled(bool enabled)
    {
        if (subtitlesUI != null)
            subtitlesUI.SetActive(enabled);
    }

    public void SetTutorialsEnabled(bool enabled)
    {
        if (tutorialUI != null)
        {
            foreach (var panel in tutorialUI)
            {
                if (panel != null)
                    panel.SetActive(enabled);
            }
        }
    }

    public void SetMusicToastEnabled(bool enabled)
    {
        if (musicToastUI != null)
            musicToastUI.SetActive(enabled);
    }

    // Quality
    public void SetQualityLow() => QualitySettings.SetQualityLevel(0);
    public void SetQualityMedium() => QualitySettings.SetQualityLevel(1);
    public void SetQualityHigh() => QualitySettings.SetQualityLevel(2);

    // Audio
    public void SetMasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MasterVolume", dB);
    }
    public void SetAmbientVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("AmbientVolume", dB);
    }
    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("SFXVolume", dB);
    }
    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MusicVolume", dB);
    }
}
