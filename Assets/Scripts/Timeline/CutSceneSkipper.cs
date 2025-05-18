using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class CutsceneSkipper : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public string mainMenuSceneName = "MainMenu";

    public GameObject skipWarningImage;             // Image that appears on first key press
    public GameObject transitionGameObject;         // Object that holds the Animator
    public Animator transitionAnimator;             // Animator with the "Play" trigger
    public float transitionDuration = 1.5f;         // Animation duration

    private bool hasPressedOnce = false;
    private bool isSkipping = false;

    void Update()
    {
        if (isSkipping) return;

        if (Input.anyKeyDown)
        {
            if (!hasPressedOnce)
            {
                hasPressedOnce = true;
                if (skipWarningImage != null)
                    skipWarningImage.SetActive(true);
            }
            else
            {
                StartCoroutine(SkipWithAnimation());
            }
        }
    }

    IEnumerator SkipWithAnimation()
    {
        isSkipping = true;

        // Stop timeline
        if (timelineDirector != null)
            timelineDirector.Stop();

        // Activate transition object and trigger animation
        if (transitionGameObject != null)
            transitionGameObject.SetActive(true);

        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("Play");

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
