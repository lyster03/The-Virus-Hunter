using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 3;
    public int currentHP;
    private int maxHP;

    public float damageCooldown = 1f;
    private float lastDamageTime = 0f;
    public int damageTaken = 1;

    public GameObject deathScreen;
    public AudioSource deathSound;
    public AudioClip hurtSound;

    public UnityEvent OnHealthChanged;

    public int MaxHearts => maxHearts;
    public int CurrentHP => currentHP;

    private bool isDying = false;

    // Initializes maxHP and currentHP
    void Awake()
    {
        maxHP = maxHearts * 2;
        currentHP = maxHP;
    }

    // Checks if the player has died and triggers death process
    void Update()
    {
        if (currentHP <= 0 && !isDying)
        {
            YouDied();
        }
    }

    // Adds health to the player, ensuring it doesn't exceed maxHP
    public void AddHealth(int halfHearts = 1)
    {
        currentHP += halfHearts;

        if (currentHP > maxHP)
            currentHP = maxHP;

        OnHealthChanged.Invoke();
    }

    // Handles collision with enemies and applies damage if cooldown has passed
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastDamageTime >= damageCooldown)
        {
            TakeDamage(damageTaken);
        }
    }

    // Handles staying in collision with enemies and applies damage if cooldown has passed
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastDamageTime >= damageCooldown)
        {
            TakeDamage(damageTaken);
        }
    }

    // Reduces the player's health by the given amount
    public void TakeDamage(int amount)
    {
        if (isDying) return;
        currentHP = Mathf.Max(currentHP - amount, 0);
        lastDamageTime = Time.time;
        OnHealthChanged.Invoke();

        SoundFXManager.Instance.PlaySoundFXClip(hurtSound, transform, 5f);
        CinemachineShake.Instance?.ShakeCamera(15f, 0.2f);
    }

    // Handles the death of the player
    void YouDied()
    {
        isDying = true;

        Time.timeScale = 0.2f;

        if (deathSound != null)
            deathSound.Play();

        if (deathScreen != null)
            deathScreen.SetActive(true);

        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        StartCoroutine(DelayedSceneChange());

        this.enabled = false;
    }

    // Delays the scene change to the main menu after a short period
    public IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Increases the player's max hearts and optionally heals to full
    public void IncreaseMaxHearts(int hearts = 1, bool healToFull = false)
    {
        maxHearts += hearts;
        maxHP = maxHearts * 2;

        if (healToFull)
            currentHP = maxHP;

        OnHealthChanged.Invoke();
    }
}
