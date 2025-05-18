using UnityEngine;

public class StartTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public GameObject transitionImage;
    public Animator animator;
    public string transitionTrigger = "Play";
    public float transitionDuration = 0.5f;

    void Start()
    {
        Debug.Log("Transition started");

        if (transitionImage != null)
            transitionImage.SetActive(true);

        if (animator != null)
            animator.SetTrigger(transitionTrigger);

        if (transitionImage != null)
            StartCoroutine(DisableAfterDelay());
    }


    private System.Collections.IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(transitionDuration);
        transitionImage.SetActive(false);
    }
}
