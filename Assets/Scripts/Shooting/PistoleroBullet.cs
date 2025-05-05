using UnityEngine;

public class PistoleroBullet : MonoBehaviour
{
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, 5f); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Castle") || collision.gameObject.CompareTag("obstacle"))
        {
            Destroy(gameObject);
        }

        Debug.Log("Bullet hit: " + collision.gameObject.name);
    }

    
    public void SetDamage(int amount)
    {
        damage = amount;
    }
}
