using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] resolutions;
    private int currentResolutionID;

    private void Awake()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> _resolutionLabels = new List<string>();
        for (var i = 0; i < resolutions.Length; i++)
        {
            _resolutionLabels.Add(resolutions[i].width + "x" + resolutions[i].height);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) 
                currentResolutionID = i;
        }
        resolutionDropDown.AddOptions(_resolutionLabels);
        resolutionDropDown.value = currentResolutionID;
        fullscreenToggle.isOn = Screen.fullScreen;
        audioMixer.GetFloat("Master", out float _volume);
        volumeSlider.value = Mathf.InverseLerp(-80, 5f, _volume);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        resolutionDropDown.onValueChanged.AddListener(UpdateResolution);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
    }

    private void UpdateVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Lerp(-80f, 5f, value));
        Debug.Log("Volume: " + value);
    }

    private void UpdateResolution(int _value)
    {
        currentResolutionID = _value;
        fullscreenToggle.onValueChanged.RemoveListener(ToggleFullscreen);
        fullscreenToggle.isOn = false;
        Screen.fullScreen = false;
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
        Screen.SetResolution(resolutions[currentResolutionID].width, resolutions[currentResolutionID].height, false);
        print("Resolution : " + resolutions[currentResolutionID]);
    }

    private void ToggleFullscreen(bool value)
    {
        Screen.fullScreen = value;
        Debug.Log("Fullscreen: " + Screen.fullScreen);
    }
}