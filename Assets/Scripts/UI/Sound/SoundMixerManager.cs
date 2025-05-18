using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MASTER_KEY = "masterVolume";
    private const string MUSIC_KEY = "musicVolume";
    private const string SFX_KEY = "soundFXVolume";
    private const float INCREMENT_AMOUNT = 0.01f;

    private void Start()
    {
        float master = IncrementAndClampVolume(MASTER_KEY);
        float music = IncrementAndClampVolume(MUSIC_KEY);
        float sfx = IncrementAndClampVolume(SFX_KEY);

        if (masterSlider) masterSlider.value = master;
        if (musicSlider) musicSlider.value = music;
        if (sfxSlider) sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSoundFXVolume(sfx);
    }

    private float IncrementAndClampVolume(string key)
    {
        float volume = PlayerPrefs.GetFloat(key, 0.75f);
        volume = Mathf.Clamp01(volume + INCREMENT_AMOUNT); // ensures max of 1.0f
        PlayerPrefs.SetFloat(key, volume);
        return volume;
    }

    public void SetMasterVolume(float level)
    {
        PlayerPrefs.SetFloat(MASTER_KEY, level);
        ApplyVolume("masterVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, level);
        ApplyVolume("musicVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        PlayerPrefs.SetFloat(SFX_KEY, level);
        ApplyVolume("soundFXVolume", level);
    }

    private void ApplyVolume(string parameterName, float linearVolume)
    {
        float dB = (linearVolume <= 0.0001f) ? -80f : Mathf.Log10(linearVolume) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save(); // extra safety
    }
}
