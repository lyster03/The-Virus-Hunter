using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 5f;
    private Transform targetPoint;
    private bool isMoving = false;

    [Header("Shooting")]
    [SerializeField] private Transform[] cannons;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletForce = 10f;
    [SerializeField] private float shootingCooldown = 2f;

    [Header("Attack Patterns")]
    [SerializeField] private float rotationPerShot = 15f;
    [SerializeField] private float rotationBetweenBursts = 45f;
    [SerializeField] private float rotationSpeed = 200f;

    private bool isAttacking = false;
    private bool bossActive = false;

    private void Start()
    {
        StartCoroutine(StartAfterDelay(3f)); // Delays boss activation
    }

    private IEnumerator StartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossActive = true;
        PickNewPatrolPoint();
    }

    private void Update()
    {
        if (!bossActive) return;

        // If boss isn't moving or attacking, initiate movement to next point
        if (!isMoving && !isAttacking)
        {
            StartCoroutine(MoveToTarget());
        }
    }

    private void PickNewPatrolPoint()
    {
        targetPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        yield return StartCoroutine(PerformAttackPattern());
    }

    private IEnumerator PerformAttackPattern()
    {
        isAttacking = true;

        int pattern = Random.Range(0, 2);

        if (pattern == 0)
        {
            yield return StartCoroutine(AttackPattern_ShootAndRotate());
        }
        else
        {
            yield return StartCoroutine(AttackPattern_BurstFire());
        }

        isAttacking = false;
        PickNewPatrolPoint();
    }

    // Fires shots while rotating after each one
    private IEnumerator AttackPattern_ShootAndRotate()
    {
        int shots = 5;

        for (int i = 0; i < shots; i++)
        {
            FireAllCannons();
            yield return new WaitForSeconds(shootingCooldown);
            yield return StartCoroutine(SmoothRotate(rotationPerShot));
        }
    }

    // Fires multiple bursts, with rotation between each burst
    private IEnumerator AttackPattern_BurstFire()
    {
        int bursts = 3;
        int shotsPerBurst = 3;

        for (int b = 0; b < bursts; b++)
        {
            for (int s = 0; s < shotsPerBurst; s++)
            {
                FireAllCannons();
                yield return new WaitForSeconds(shootingCooldown * 0.5f);
            }

            yield return StartCoroutine(SmoothRotate(rotationBetweenBursts));
            yield return new WaitForSeconds(shootingCooldown);
        }
    }

    // Spawns bullets from all cannon points
    private void FireAllCannons()
    {
        foreach (Transform cannon in cannons)
        {
            GameObject bullet = Instantiate(bulletPrefab, cannon.position, cannon.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(cannon.up * bulletForce, ForceMode2D.Impulse);
            }
        }
    }

    // Smoothly rotates the boss by a given angle
    private IEnumerator SmoothRotate(float angle)
    {
        float elapsed = 0f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.z + angle);
        float duration = Mathf.Abs(angle) / rotationSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
    }
}
