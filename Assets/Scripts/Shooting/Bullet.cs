using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;

    [Header("Forking")]
    public bool canFork = false;
    public GameObject forkedBulletPrefab;
    public float sizeMultiplier = 1f;
    public int pierceCount = 0;

    void Start()
    {
        transform.localScale *= sizeMultiplier;
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                bool wasAliveBefore = !enemy.IsDead();

                enemy.TakeDamage(damage, transform.position);

                bool killed = wasAliveBefore && enemy.IsDead();

                // Fork new bullets if enabled
                if (canFork && forkedBulletPrefab != null)
                    Fork();

                // Continue if piercing is available and enemy was killed
                if (pierceCount > 0 && killed)
                {
                    pierceCount--;
                    return;
                }
            }

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Castle") ||
            collision.gameObject.CompareTag("obstacle"))
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    // Spawns two angled bullets from current bullet's position
    void Fork()
    {
        float angleOffset = 30f;

        for (int i = -1; i <= 1; i += 2)
        {
            float angle = transform.eulerAngles.z + i * angleOffset;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 spawnPos = transform.position + (Vector3)(dir * 0.2f);

            GameObject fork = Instantiate(forkedBulletPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
            Rigidbody2D rb = fork.GetComponent<Rigidbody2D>();
            rb.velocity = dir * 7f;

            Bullet b = fork.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = Mathf.Max(1, damage / 2);
                b.sizeMultiplier = 0.75f;
                b.pierceCount = 0;
                b.canFork = false;
            }
        }
    }
}
