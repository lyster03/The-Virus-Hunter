using UnityEngine;
using System.Collections;

public class PoisonZone : MonoBehaviour
{
    public float duration = 6f;
    public int damagePerTick = 1;
    public float damageInterval = 1f;
    public LayerMask enemyLayer;

    private void Start()
    {
        Invoke(nameof(DestroySelf), duration);
        StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        while (true)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, enemyLayer);
            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent<Enemy>(out var e))
                {
                    e.TakeDamage(damagePerTick, transform.position);
                }
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
