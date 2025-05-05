using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(NavMeshAgent))]
public class PistoleroAI : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float shootForce = 8f;
    public float shootCooldown = 1.2f;
    public LayerMask visionMask;
    public float shootRange = 8f;
    public float rotationSpeed = 720f; // degrees per second

    private Enemy enemyBase;
    private NavMeshAgent agent;
    private bool canShoot = true;
    private bool lockedOnCastle = false;
    private Transform currentTarget;

    private void Start()
    {
        enemyBase = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (enemyBase == null || enemyBase.IsDead()) return;

        UpdateTarget();

        if (currentTarget != null && CanSee(currentTarget))
        {
            FaceTarget(currentTarget.position);
            agent.isStopped = true;
            TryShoot();
        }
        else
        {
            agent.isStopped = false;
            MoveToCastle();
        }
    }

    private void UpdateTarget()
    {
        if (!lockedOnCastle && enemyBase.player != null && CanSee(enemyBase.player.transform))
        {
            currentTarget = enemyBase.player.transform;
        }
        else if (CanSee(enemyBase.castle))
        {
            lockedOnCastle = true;
            currentTarget = enemyBase.castle;
        }
        else
        {
            currentTarget = enemyBase.castle;
        }
    }

    private void MoveToCastle()
    {
        if (enemyBase.castle != null)
        {
            agent.SetDestination(enemyBase.castle.position);
        }
    }

    private void FaceTarget(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion lookRotation = Quaternion.Euler(0, 0, angle);

        // Faster and smoother rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private bool CanSee(Transform target)
    {
        if (target == null) return false;

        Vector2 dir = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > shootRange)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, visionMask);
        if (hit.collider != null && hit.collider.CompareTag(target.tag))
        {
            return true;
        }
        return false;
    }

    private void TryShoot()
    {
        if (canShoot && bulletPrefab != null && shootPoint != null)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        canShoot = false;

        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootPoint.up * shootForce;
        }
        Destroy(bullet, 5f);

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
