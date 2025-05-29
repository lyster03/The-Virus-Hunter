using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleLevelLoader : MonoBehaviour
{
    [Header("Scene Names")]
    public string level1Scene = "GamePlayScene";
    public string level2Scene = "Level2Scene";

    [Header("Transition Settings")]
    public GameObject transitionImage;
    public Animator animator;
    public string transitionTrigger = "Play";
    public float transitionDuration = 1f;

    private void Start()
    {
        if (transitionImage != null)
            transitionImage.SetActive(false);
    }

    public void LoadLevel1()
    {
        StartCoroutine(LoadWithTransition(level1Scene));
    }

    public void LoadLevel2()
    {
        StartCoroutine(LoadWithTransition(level2Scene));
    }

    private IEnumerator LoadWithTransition(string sceneName)
    {
        if (transitionImage != null)
        {
            transitionImage.SetActive(true);
            if (animator != null)
                animator.SetTrigger(transitionTrigger);
        }

        yield return new WaitForSecondsRealtime(transitionDuration);
        SceneManager.LoadScene(sceneName);
    }
}
