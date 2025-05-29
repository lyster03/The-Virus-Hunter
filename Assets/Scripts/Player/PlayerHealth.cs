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
    public AudioClip deathSound;
    public AudioClip hurtSound;

    public UnityEvent OnHealthChanged;

    public int MaxHearts => maxHearts;
    public int CurrentHP => currentHP;

    private bool isDying = false;

    private ShieldController shieldController;

    [Header("Death Animation")]
    [SerializeField] private GameObject deathAnimationObject;
    [SerializeField] private string deathAnimationTrigger = "Play";

    void Awake()
    {
        maxHP = maxHearts * 2;
        currentHP = maxHP;
        shieldController = GetComponent<ShieldController>();
    }

    void Update()
    {
        if (currentHP <= 0 && !isDying)
        {
            YouDied();
        }
    }

    public void AddHealth(int halfHearts = 1)
    {
        currentHP = Mathf.Min(currentHP + halfHearts, maxHP);
        OnHealthChanged.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastDamageTime >= damageCooldown)
        {
            TakeDamage(damageTaken);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastDamageTime >= damageCooldown)
        {
            TakeDamage(damageTaken);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDying) return;

        if (shieldController != null && shieldController.TryNegateDamage(gameObject))
        {
            lastDamageTime = Time.time;
            return;
        }

        currentHP = Mathf.Max(currentHP - amount, 0);
        lastDamageTime = Time.time;
        OnHealthChanged.Invoke();

        SoundFXManager.Instance?.PlaySoundFXClip(hurtSound, transform, 5f);
        CinemachineShake.Instance?.ShakeCamera(15f, 0.2f);
    }

    void YouDied()
    {
        isDying = true;
        

        
        if (deathSound != null)
            SoundFXManager.Instance.PlaySoundFXClip(deathSound, transform, 1f);

       
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
        GetComponent<Shooting>().enabled = false;

        StartCoroutine(DelayedSceneChange());
       
        this.enabled = false;
    }

    public IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;
        deathScreen.SetActive(true); 
    }

    public void IncreaseMaxHearts(int hearts = 1, bool healToFull = false)
    {
        maxHearts += hearts;
        maxHP = maxHearts * 2;

        if (healToFull)
            currentHP = maxHP;

        OnHealthChanged.Invoke();
    }
}
