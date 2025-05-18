using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Castle : MonoBehaviour
{
    [Header("Castle Health Settings")]
    public int castleHealth = 1000;
    public int maxCastleHealth;
    public float damageCooldown = 1f;

    private Dictionary<Enemy, float> enemyCooldowns = new Dictionary<Enemy, float>();

    [Header("UI & Feedback")]
    public GameObject deathScreen;
    public AudioClip deathSound;
    public HealthBarUI castleHealthBar;

    [Header("Death Animation (Optional)")]
    [SerializeField] private GameObject deathAnimationObject;
    [SerializeField] private string deathAnimationTrigger = "Play";

    private Vector3 originalScale;
    private bool isDying = false;

    void Start()
    {
        maxCastleHealth = castleHealth;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (castleHealth <= 0 && !isDying)
        {
            YouDied();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (!enemyCooldowns.ContainsKey(enemy))
            {
                enemyCooldowns[enemy] = 0f;
            }

            if (Time.time - enemyCooldowns[enemy] >= damageCooldown)
            {
                int damage = enemy.damage;
                castleHealth -= damage;
                enemyCooldowns[enemy] = Time.time;

                if (castleHealthBar != null)
                    castleHealthBar.UpdateHealthBar(this);

                StartCoroutine(ScaleBounce());
            }
        }
    }

    private IEnumerator ScaleBounce()
    {
        transform.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    void YouDied()
    {
        isDying = true;
        

        if (deathSound != null)
            SoundFXManager.Instance?.PlaySoundFXClip(deathSound, transform, 1f);

        if (deathAnimationObject != null)
        {
            deathAnimationObject.SetActive(true);
            Animator animator = deathAnimationObject.GetComponent<Animator>();
            if (animator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
            {
                animator.SetTrigger(deathAnimationTrigger);
            }
        }

        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        this.enabled = false;

        StartCoroutine(LoadMainMenuAfterDelay(1f));
    }

    private IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
