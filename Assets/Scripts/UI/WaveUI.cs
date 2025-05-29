using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveUI : MonoBehaviour
{
    [Header("Wave Text Settings")]
    [SerializeField] private Text waveText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector2 moveOffset = new Vector2(0, 50f);

    [Header("Permanent UI")]
    [SerializeField] private Text permanentWaveText;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float audioStartTime = 13f;
    [SerializeField] private float audioFadeDuration = 1f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float fadeOutDuration = 2f;

    private int currentWaveNumber = 0;

    private WaveSpawnerV2 waveSpawner;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (permanentWaveText != null)
            permanentWaveText.text = "Wave 0";
    }

    private void Start()
    {
        waveSpawner = FindObjectOfType<WaveSpawnerV2>();
        if (waveSpawner == null)
        {
            Debug.LogWarning("WaveSpawnerV2 not found in the scene.");
        }
    }

    private void Update()
    {
        if (waveSpawner != null && waveSpawner.HasCompletedAllWaves())
        {
            if (permanentWaveText != null && permanentWaveText.text != "BOSS")
            {
                permanentWaveText.text = "BOSS";
                Debug.Log("All waves completed. Showing BOSS in permanent wave text.");
            }
        }
    }

    public void ShowWave(int waveNumber)
    {
        currentWaveNumber = waveNumber;
        waveText.text = $"Wave {waveNumber}";
        StartCoroutine(PlayWaveAnimationWithAudio());
    }

    private IEnumerator PlayWaveAnimationWithAudio()
    {
        float totalAnimDuration = fadeInDuration + holdDuration + fadeOutDuration;

        rectTransform.anchoredPosition = originalPosition;
        canvasGroup.alpha = 0f;

        if (audioSource != null && audioSource.clip != null)
        {
            if (audioSource.clip.length >= audioStartTime + totalAnimDuration)
            {
                audioSource.Stop();
                audioSource.time = audioStartTime;
                audioSource.volume = 0f;
                audioSource.Play();

                StartCoroutine(FadeInAudio(audioFadeDuration));
                StartCoroutine(FadeOutAudio(audioFadeDuration, totalAnimDuration - audioFadeDuration));
                StartCoroutine(StopAudioAfterDelay(totalAnimDuration));
            }
            else
            {
                Debug.LogWarning("Audio clip is too short to play for full animation.");
            }
        }

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            float t = timer / fadeInDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + moveOffset, t);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition + moveOffset;

        yield return new WaitForSeconds(holdDuration);

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            float t = timer / fadeOutDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition;

        if (permanentWaveText != null)
        {
            permanentWaveText.text = waveSpawner != null && waveSpawner.HasCompletedAllWaves()
                ? "  BOSS"
                : $"Wave {currentWaveNumber}";
        }
    }

    private IEnumerator FadeInAudio(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    private IEnumerator FadeOutAudio(float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        while (timer < duration)
        {
            audioSource.volume = Mathf.Lerp(1f, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f;
    }

    private IEnumerator StopAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
