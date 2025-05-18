using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullScreenToggle;

    Resolution[] AllResolutions;
    bool IsFullScreen = true;
    int SelectedResolution = 0;
    List<Resolution> SelectedResolutionList = new List<Resolution>();

    private const string RESOLUTION_PREF_KEY = "resolutionIndex";
    private const string FULLSCREEN_PREF_KEY = "isFullScreen";

    private void Start()
    {
        StartCoroutine(InitializeResolution());
    }

    IEnumerator InitializeResolution()
    {
        yield return null; // Wait one frame for UI to initialize properly

        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        Resolution currentResolution = Screen.currentResolution;
        int currentResolutionIndex = 0;

        for (int i = 0; i < AllResolutions.Length; i++)
        {
            Resolution res = AllResolutions[i];
            newRes = res.width + " x " + res.height;

            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);

                if (res.width == currentResolution.width && res.height == currentResolution.height)
                {
                    currentResolutionIndex = SelectedResolutionList.Count - 1;
                }
            }
        }

        ResDropDown.ClearOptions();
        ResDropDown.AddOptions(resolutionStringList);

        // Load saved resolution and fullscreen, if any
        SelectedResolution = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, currentResolutionIndex);
        IsFullScreen = PlayerPrefs.GetInt(FULLSCREEN_PREF_KEY, 1) == 1;

        ResDropDown.value = SelectedResolution;
        ResDropDown.RefreshShownValue();

        FullScreenToggle.isOn = IsFullScreen;

        ApplyResolution();
    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, SelectedResolution);
        ApplyResolution();
    }

    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        PlayerPrefs.SetInt(FULLSCREEN_PREF_KEY, IsFullScreen ? 1 : 0);
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        Resolution res = SelectedResolutionList[SelectedResolution];
        Screen.SetResolution(res.width, res.height, IsFullScreen);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, SelectedResolution);
        PlayerPrefs.SetInt(FULLSCREEN_PREF_KEY, IsFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

}
