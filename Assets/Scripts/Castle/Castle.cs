using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public int castleHealth = 1000;
    public int maxCastleHealth;
    public float damageCooldown = 1f;

    private Dictionary<Enemy, float> enemyCooldowns = new Dictionary<Enemy, float>();

    public GameObject deathScreen;
    public AudioSource deathSound;

    public HealthBarUI castleHealthBar;

    private Vector3 originalScale;

    void Start()
    {
        maxCastleHealth = castleHealth;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Update health bar UI
        if (castleHealthBar != null)
        {
            castleHealthBar.UpdateHealthBar(this);
        }

        // Trigger death sequence if health reaches zero
        if (castleHealth <= 0)
        {
            YouDied();
            deathSound.Play();
            deathScreen.SetActive(true);
            gameObject.SetActive(false);
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

            // Apply damage if cooldown has passed
            if (Time.time - enemyCooldowns[enemy] >= damageCooldown)
            {
                int damage = enemy.damage;
                castleHealth -= damage;
                enemyCooldowns[enemy] = Time.time;

                StartCoroutine(ScaleBounce());
            }
        }
    }

    // bounce effect when hit
    private IEnumerator ScaleBounce()
    {
        transform.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // Slow down time on death
    void YouDied()
    {
        Time.timeScale = 0.2f;
    }
}
