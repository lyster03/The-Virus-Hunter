using UnityEngine;

public class SceneIntroAnimation : MonoBehaviour
{
    public GameObject animatedObject;       // GameObject that holds the Animator
    public Animator animator;               // Animator component
    public string animationTrigger = "Play";

    void Start()
    {
        if (animatedObject != null)
            animatedObject.SetActive(true); // Activate the GameObject

        if (animator != null)
            animator.SetTrigger(animationTrigger); // Play the animation
    }
}
