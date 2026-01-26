using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject previousMenu;
    public AudioMixer audioMixer;
    public TMP_Dropdown qualityDropdown;
    public Slider volumeSlider;
    public GameObject pauseMenu;

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (previousMenu != null)
            previousMenu.SetActive(true);
    }

    void Start()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        float volume;
        audioMixer.GetFloat("MasterVolume", out volume);
        volumeSlider.value = Mathf.Pow(10, volume / 20);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MasterVolume", dB);
    }
}
//dette var noget ekstra vi legede lidt med, det er mest lavet med hjælp fra GitHub Copilot