using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    [Header("Transition Animation")]
    public GameObject transitionObject; // Reference to object already in the scene
    public float transitionDuration = 2f;

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;

        if (transitionObject != null)
            transitionObject.SetActive(false); // Hide it at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu(string mainMenuSceneName)
    {
        StartCoroutine(PlayTransitionAndLoad(mainMenuSceneName));
    }

    private IEnumerator PlayTransitionAndLoad(string sceneName)
    {
        Time.timeScale = 1f;

        if (transitionObject != null)
        {
            transitionObject.SetActive(true);

            Animator animator = transitionObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Play");
            }
        }

        yield return new WaitForSecondsRealtime(transitionDuration);
        SceneManager.LoadScene(sceneName);
    }
}
