using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public Button level1Button;
    public Button level2Button;
    public GameObject lockedLevel2Icon;

    [Header("Scene Names")]
    public string level1Scene = "GamePlayScene";
    public string level2Scene = "Level2Scene";

    [Header("Transition Settings")]
    public GameObject transitionImage;
    public Animator animator;
    public string transitionTrigger = "Play";
    public float transitionDuration = 1f;

    private bool isLevel2Unlocked;

    private void Start()
    {
        LoadUnlockState();
        UpdateLevelButtons();

        if (transitionImage != null)
            transitionImage.SetActive(false);
    }

    private void LoadUnlockState()
    {
       
        isLevel2Unlocked = PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;
    }

    private void UpdateLevelButtons()
    {
        level2Button.interactable = isLevel2Unlocked;

        if (lockedLevel2Icon != null)
            lockedLevel2Icon.SetActive(!isLevel2Unlocked);
    }

    public void LoadLevel1()
    {
        StartCoroutine(PlayGameWithTransition(level1Scene));
    }

    public void LoadLevel2()
    {
        if (isLevel2Unlocked)
        {
            StartCoroutine(PlayGameWithTransition(level2Scene));
        }
    }

    IEnumerator PlayGameWithTransition(string sceneName)
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

    
    public static void UnlockLevel2()
    {
        PlayerPrefs.SetInt("Level2Unlocked", 1);
        PlayerPrefs.Save();
    }
}