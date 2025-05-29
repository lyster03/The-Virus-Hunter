using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveDeathText : MonoBehaviour
{
    [SerializeField] private Text waveText;
    [SerializeField] private string bossName;

    void OnEnable()
    {
        if (waveText == null)
            waveText = GetComponent<Text>();

        int lastWave = WaveSpawnerV2.LastWaveIndex;
        if (lastWave >= 10)
        {
            waveText.text = $"{bossName} was the end of you.";

        }
        else
        {
            waveText.text = $"Wave {lastWave + 1} was the end of you.";
        }
            
    }
}
