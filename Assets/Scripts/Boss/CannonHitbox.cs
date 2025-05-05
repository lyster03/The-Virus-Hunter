using UnityEngine;

public class CannonHitbox : MonoBehaviour
{
    [SerializeField] private BossHealth bossHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Bullet bullet = collision.collider.GetComponent<Bullet>();
            if (bullet != null)
            {
                bossHealth.TakeDamage(bullet.damage);
            }

            Destroy(collision.collider.gameObject);
        }
    }
}
