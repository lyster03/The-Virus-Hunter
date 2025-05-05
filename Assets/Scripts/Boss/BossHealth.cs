using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider redSlider;
    public Slider yellowSlider;
    public GameObject healthBarUI;

    [Header("Yellow Bar Settings")]
    public float yellowDelay = 0.5f;
    public float yellowSpeed = 50f;

    private float yellowTimer = 0f;
    private bool tookDamage = false;
    private bool isDead = false;

    [Header("Death Sound")]
    public AudioClip deathClip;

    [Header("Scene Transition")]
    public Animator endAnimator;
    public GameObject endAnimationObject;
    public string endAnimationTrigger = "PlayEnd";
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        currentHealth = maxHealth;

        if (redSlider != null)
        {
            redSlider.maxValue = maxHealth;
            redSlider.value = maxHealth;
        }

        if (yellowSlider != null)
        {
            yellowSlider.maxValue = maxHealth;
            yellowSlider.value = maxHealth;
        }
    }

    private void Update()
    {
        // Smooth transition of yellow slider after taking damage
        if (tookDamage)
        {
            yellowTimer -= Time.unscaledDeltaTime;

            if (yellowTimer <= 0f && yellowSlider.value > redSlider.value)
            {
                yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, redSlider.value, yellowSpeed * Time.unscaledDeltaTime);
            }
            else if (yellowSlider.value <= redSlider.value)
            {
                tookDamage = false;
            }
        }

        // Trigger death if health reaches zero
        if (!isDead && currentHealth <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0f, currentHealth);

        if (redSlider != null)
            redSlider.value = currentHealth;

        yellowTimer = yellowDelay;
        tookDamage = true;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (redSlider != null)
            redSlider.value = currentHealth;

        if (yellowSlider != null && yellowSlider.value < redSlider.value)
            yellowSlider.value = currentHealth;
    }

    private void Die()
    {
        // Play death sound
        SoundFXManager.Instance.PlaySoundFXClip(deathClip, transform, 1f);

        // Hide health bar UI
        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        // Move boss out of view
        transform.position = new Vector3(9999, 9999, 9999);

        // Disable visuals and collisions
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Begin scene transition sequence
        StartCoroutine(HandlePostDeathSequence());
    }

    private IEnumerator HandlePostDeathSequence()
    {
        yield return new WaitForSeconds(2f);

        if (endAnimationObject != null)
            endAnimationObject.SetActive(true);

        if (endAnimator != null)
            endAnimator.SetTrigger(endAnimationTrigger);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
