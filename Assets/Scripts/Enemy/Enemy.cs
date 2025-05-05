using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float enemyHealth = 3;
    public int damage = 1;

    [Header("XP Reward")]
    public int minExperience = 2;
    public int maxExperience = 5;

    [Header("Targeting")]
    public float viewDistance = 10f;
    public LayerMask obstacleMask;
    public float playerCheckInterval = 0.5f;

    [Header("Knockback")]
    public float knockbackDistance = 1.5f;
    public float knockbackDuration = 0.1f;
    public LayerMask knockbackBlockMask;

    [Header("FX")]
    public GameObject soulParticlePrefab;
    public GameObject deathEffect;
    public AudioClip death;
    public AudioClip[] hurtSounds;

    [HideInInspector] public Transform castle;
    [HideInInspector] public GameObject player;

    protected Transform currentTarget;
    protected NavMeshAgent agent;
    protected ExperienceManager experienceManager;
    protected Shooting shooting;
    protected WaveSpawnerV2 waveSpawner;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalScale;

    private bool isDead = false;
    private float lastPlayerCheckTime;
    private bool hasDamagedCastle = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        castle = GameObject.FindGameObjectWithTag("Castle")?.transform;
        player = GameObject.FindGameObjectWithTag("Player");

        shooting = player?.GetComponent<Shooting>();
        experienceManager = FindObjectOfType<ExperienceManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;

        currentTarget = castle;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (!hasDamagedCastle && Time.time - lastPlayerCheckTime >= playerCheckInterval)
        {
            EvaluateTarget();
            lastPlayerCheckTime = Time.time;
        }

        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    protected virtual void EvaluateTarget()
    {
        if (player == null || castle == null)
        {
            currentTarget = castle;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float distanceToCastle = Vector2.Distance(transform.position, castle.position);

        if (distanceToPlayer <= viewDistance)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, obstacleMask | LayerMask.GetMask("Player"));

            if (hit.collider != null && hit.collider.CompareTag("Player") && distanceToPlayer < distanceToCastle)
            {
                currentTarget = player.transform;
                return;
            }
        }

        currentTarget = castle;
    }

    public void OnDamagedCastle()
    {
        hasDamagedCastle = true;
        currentTarget = castle;
    }

    public void SetCastle(Transform castleTransform) => castle = castleTransform;
    public void SetSpawner(WaveSpawnerV2 spawner) => waveSpawner = spawner;

    public void TakeDamage(int damageAmount, Vector2 hitSource)
    {
        if (isDead) return;

        if (enemyHealth > 1)
            SoundFXManager.Instance.PlayRandomSoundFXClip(hurtSounds, transform, 5f);

        CinemachineShake.Instance?.ShakeCamera(6f, 0.1f);
        enemyHealth -= damageAmount;

        StartCoroutine(FlashAndScale());
        ApplyKnockback(hitSource);

        if (enemyHealth <= 0)
            Die();
    }

    void ApplyKnockback(Vector2 source)
    {
        Vector2 knockDir = ((Vector2)transform.position - source).normalized;
        Vector2 target = (Vector2)transform.position + knockDir * knockbackDistance;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, knockDir, knockbackDistance, knockbackBlockMask);
        if (hit.collider != null)
            target = hit.point;

        StartCoroutine(KnockbackMovement((Vector2)transform.position, target, knockbackDuration));
    }

    IEnumerator KnockbackMovement(Vector2 start, Vector2 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

    IEnumerator FlashAndScale()
    {
        spriteRenderer.color = Color.white;
        transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
        transform.localScale = originalScale;
    }

    public bool IsDead() => isDead;

    void Die()
    {
        if (isDead) return;
        isDead = true;

        EnemyManager.Instance?.Unregister(transform);

        int xp = Random.Range(minExperience, maxExperience + 1);
        experienceManager?.AddExperience(xp);
        shooting?.AddEnergy(10f);

        if (soulParticlePrefab != null && player != null)
        {
            GameObject soul = Instantiate(soulParticlePrefab, transform.position, Quaternion.identity);
            soul.GetComponent<SoulFollow>()?.SetTarget(player.transform);
        }

        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        waveSpawner?.EnemyDied(gameObject);
        CinemachineShake.Instance?.ShakeCamera(13f, 0.2f);
        SoundFXManager.Instance.PlaySoundFXClip(death, transform, 1f);

        Destroy(gameObject);
    }
}
