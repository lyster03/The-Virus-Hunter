using UnityEngine;

public class ReverseShot : MonoBehaviour
{
    private Shooting shooting;

    private void Awake()
    {
        shooting = GetComponent<Shooting>();
    }

    private void OnEnable()
    {
        if (shooting == null) shooting = GetComponent<Shooting>();
        if (shooting != null)
        {
            shooting.OnShoot += FireReverse;
        }
    }

    private void OnDisable()
    {
        if (shooting != null)
        {
            shooting.OnShoot -= FireReverse;
        }
    }

    private void FireReverse()
    {
        if (shooting.bulletPrefab1 == null || shooting.firePoint == null) return;

        Vector2 shootDir = (shooting.playerScript.mousePos - (Vector2)shooting.firePoint.position).normalized;
        Vector2 reverseDir = -shootDir;
        float baseAngle = Mathf.Atan2(reverseDir.y, reverseDir.x) * Mathf.Rad2Deg;

        GameObject projectileToSpawn = shooting.bulletPrefab1;

        if (shooting.isDoubleShot)
        {
            FireReverseBullet(baseAngle - shooting.doubleShotAngle / 2f, reverseDir, projectileToSpawn);
            FireReverseBullet(baseAngle + shooting.doubleShotAngle / 2f, reverseDir, projectileToSpawn);
        }
        else
        {
            FireReverseBullet(baseAngle, reverseDir, projectileToSpawn);
        }
    }
    // logic for shooting a bullet in the opposite direction
    private void FireReverseBullet(float angle, Vector2 direction, GameObject projectile)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector3 firePos = shooting.firePoint.position + (Vector3)(dir * 0.2f);

        GameObject bullet = Instantiate(projectile, firePos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = dir.normalized * shooting.bulletForce;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = shooting.bulletDamage;
        }

        Destroy(bullet, 5f);
    }
}
