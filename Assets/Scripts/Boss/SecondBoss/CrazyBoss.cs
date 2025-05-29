using System.Collections;
using UnityEngine;

public class CircularBoss : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 1.5f;
    [SerializeField] private float moveTransitionSpeed = 2f;
    private float currentAngle;
    private Vector2 targetPosition;

    [Header("Combat Settings")]
    [SerializeField] private float timeBetweenAttacks = 3f;
    [SerializeField] private float patternSwitchInterval = 10f;
    private float attackCooldownTimer;
    private Transform playerTarget;

    [Header("Bullet Patterns")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject homingBulletPrefab;
    [SerializeField] private float bulletForce = 12f;

    [Header("Pattern Settings")]
    [SerializeField] private int spiralArms = 5;
    [SerializeField] private float spiralAngleIncrement = 12f;
    [SerializeField] private int waveBullets = 7;
    [SerializeField] private float waveAmplitude = 1.5f;
    [SerializeField] private int homingBurstCount = 3;
    [SerializeField] private float homingSpawnDelay = 0.4f;

    private bool isAttacking = false;
    private bool isActive = false;

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        if (orbitCenter == null) orbitCenter = transform;
        currentAngle = Random.Range(0f, 360f);
        StartCoroutine(ActivateBoss(2f));
    }

    private IEnumerator ActivateBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActive = true;
        StartCoroutine(PatternCycleRoutine());
    }

    private void Update()
    {
        if (!isActive) return;

        UpdateCircularMovement();
        HandleAttackCycle();
    }

    private void UpdateCircularMovement()
    {
        currentAngle += orbitSpeed * Time.deltaTime;
        targetPosition = orbitCenter.position + new Vector3(
            Mathf.Cos(currentAngle) * orbitRadius,
            Mathf.Sin(currentAngle) * orbitRadius,
            0
        );
        transform.position = Vector2.Lerp(transform.position, targetPosition, moveTransitionSpeed * Time.deltaTime);
    }

    private void HandleAttackCycle()
    {
        if (!isAttacking && attackCooldownTimer <= 0)
        {
            StartCoroutine(ExecuteAttackPattern());
            attackCooldownTimer = timeBetweenAttacks;
        }
        else
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    private IEnumerator PatternCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(patternSwitchInterval);
            orbitSpeed *= -1;
            orbitRadius = Random.Range(2f, 4f);
        }
    }

    private IEnumerator ExecuteAttackPattern()
    {
        isAttacking = true;

        int pattern = Random.Range(0, 5);
        switch (pattern)
        {
            case 0:
                yield return StartCoroutine(OrbitalPulse());
                break;
            case 1:
                yield return StartCoroutine(SpiralFan());
                break;
            case 2:
                yield return StartCoroutine(HomingSwarm());
                break;
            case 3:
                yield return StartCoroutine(RadialBurst());
                break;
            case 4:
                yield return StartCoroutine(StaggeredWave());
                break;
        }

        isAttacking = false;
    }

    // Normal bullet patterns (from orbit center)
    private IEnumerator OrbitalPulse()
    {
        int pulses = 3;
        for (int i = 0; i < pulses; i++)
        {
            FireRing(8 + i * 2, 45f * i);
            yield return new WaitForSeconds(0.7f);
        }
    }

    private IEnumerator SpiralFan()
    {
        float currentAngle = 0f;
        for (int i = 0; i < 8; i++)
        {
            FireSpread(spiralArms, currentAngle);
            currentAngle += spiralAngleIncrement;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator RadialBurst()
    {
        FireRing(12, 0f);
        yield return new WaitForSeconds(0.5f);
        FireRing(12, 15f);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StaggeredWave()
    {
        for (int i = 0; i < 3; i++)
        {
            FireWave(waveBullets, waveAmplitude * (i + 1));
            yield return new WaitForSeconds(0.6f);
        }
    }

    // Homing attack pattern (from boss position)
    private IEnumerator HomingSwarm()
    {
        for (int i = 0; i < homingBurstCount; i++)
        {
            FireHomingMissiles(3);
            yield return new WaitForSeconds(homingSpawnDelay);
        }
    }

    private void FireRing(int bullets, float startAngle)
    {
        float angleStep = 360f / bullets;
        for (int i = 0; i < bullets; i++)
        {
            float angle = startAngle + angleStep * i;
            FireBullet(orbitCenter.position, angle);
        }
    }

    private void FireSpread(int arms, float startAngle)
    {
        float angleStep = 360f / arms;
        for (int i = 0; i < arms; i++)
        {
            float angle = startAngle + angleStep * i;
            FireBullet(orbitCenter.position, angle);
        }
    }

    private void FireWave(int bullets, float amplitude)
    {
        float baseAngle = Random.Range(0f, 360f);
        for (int i = 0; i < bullets; i++)
        {
            float angle = baseAngle + (360f / bullets) * i;
            float waveOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude;
            FireBullet(orbitCenter.position, angle + waveOffset);
        }
    }

    private void FireHomingMissiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = transform.position;
            spawnPos += Random.insideUnitCircle * 0.5f;
            FireHomingBullet(spawnPos);
        }
    }

    private void FireBullet(Vector2 position, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.AddForce(bullet.transform.up * bulletForce, ForceMode2D.Impulse);
        Destroy(bullet, 4f);
    }

    private void FireHomingBullet(Vector2 position)
    {
        GameObject homing = Instantiate(homingBulletPrefab, position, Quaternion.identity);
        HomingMissile hm = homing.GetComponent<HomingMissile>();
        if (hm) hm.Initialize(playerTarget, 180f, bulletForce);
        Destroy(homing, 6f);
    }
}