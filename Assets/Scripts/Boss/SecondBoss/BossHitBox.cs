using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossHitbox2D : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 10f;

    private BossHealth bossHealth;

    private void Start()
    {
        // Find BossHealth script in parent object
        bossHealth = GetComponentInParent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogError("BossHealth script not found on parent object.");
        }

        // Ensure this collider is a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Reset rotation on start
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        // Continuously lock rotation
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it was hit by a player bullet
        if (other.CompareTag("Bullet"))
        {
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damageAmount);
            }

            // Optional: destroy the bullet
            Destroy(other.gameObject);
        }
    }
}
