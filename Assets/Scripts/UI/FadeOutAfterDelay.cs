using UnityEngine;
using System.Collections;

public class FadeOutAfterDelay : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float visibleDuration = 3f;
    public float fadeDuration = 2f;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // Stay fully visible for a while
        yield return new WaitForSecondsRealtime(visibleDuration);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
