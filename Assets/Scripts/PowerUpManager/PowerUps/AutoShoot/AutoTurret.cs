using System.Collections;
using UnityEngine;

public class AutoTurret : MonoBehaviour
{
    public GameObject bulletPrefab;
    private float fireInterval = 2;
    private float bulletSpeed= 35;
    private int bulletDamage = 2;
    private Coroutine firingRoutine;

    public void Initialize(GameObject bulletPrefab, float fireInterval, float bulletSpeed, int bulletDamage)
    {
        this.bulletPrefab = bulletPrefab;
        this.fireInterval = fireInterval;
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;

        if (firingRoutine != null)
            StopCoroutine(firingRoutine);

        firingRoutine = StartCoroutine(FireRoutine());
    }


    private IEnumerator FireRoutine()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void Fire()
    {
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.right,
            Vector2.down,
            Vector2.left
        };

        foreach (Vector2 dir in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = dir * bulletSpeed;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = bulletDamage;
                bulletScript.pierceCount = 0;
                bulletScript.canFork = false;
                bulletScript.sizeMultiplier = 1f;

                
            }
        }
    }
}
