using UnityEngine;

public class PetShooter : MonoBehaviour
{
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 13f;
    public float fireCooldown = 0.7f;
    

    private float timer;
    private Player playerScript;

    void Start()
    {
        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }
    }

    void Update()
    {
        if (player == null || playerScript == null) return;

        // Follow the player
        transform.position = player.position;

        
        timer += Time.deltaTime;
        if (timer >= fireCooldown)
        {
            ShootInMouseDirection();
            timer = 0f;
        }
    }

    void ShootInMouseDirection()
    {
        Vector2 mouseDirection = (playerScript.mousePos - (Vector2)player.position).normalized;

        Vector2 cardinal = Vector2.zero;

        if (Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.y))
        {
            cardinal = mouseDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            cardinal = mouseDirection.y > 0 ? Vector2.up : Vector2.down;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(cardinal * bulletForce, ForceMode2D.Impulse);

        Destroy(bullet, 5f);
    }
}
