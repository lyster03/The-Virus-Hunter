using UnityEngine;

public class OrbitingOrb : MonoBehaviour
{
    public Transform player;
    public float orbitSpeed = 180f;
    public float radius = 1.5f;
    public int damage = 1;

    private float angle;

    private void Update()
    {
        if (player == null) return;

        angle += orbitSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;
        transform.position = player.position + new Vector3(x, y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 hitSource = transform.position;
                enemy.TakeDamage(damage, hitSource);
            }
        }
    }
}
