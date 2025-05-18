using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject transitionImage; // The UI image with the animation
    public Animator animator; // Animator component on that image
    public string transitionTrigger = "Play"; // Trigger name in Animator
    public float transitionDuration = 1f; // How long the animation lasts

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
