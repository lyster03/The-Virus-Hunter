using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject transitionImage; 
    public Animator animator;
    public string transitionTrigger = "Play"; 
    public float transitionDuration = 1f;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("Level2Unlocked"))
        {
            PlayerPrefs.SetInt("Level2Unlocked", 0);
            PlayerPrefs.Save();
        }
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game closed!");
    }

    public void PlayButton()
    {
        StartCoroutine(PlayGameWithTransition());
    }

    IEnumerator PlayGameWithTransition()
    {
        // Activate transition image
        if (transitionImage != null)
            transitionImage.SetActive(true);

       
        if (animator != null)
            animator.SetTrigger(transitionTrigger);
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene("GamePlayScene");

    }
}
