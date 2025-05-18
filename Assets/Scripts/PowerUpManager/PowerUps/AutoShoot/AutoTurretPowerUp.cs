using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Auto Turret")]
public class AutoTurretPowerUp : PowerUp
{
    public GameObject bulletPrefab;
    public float fireInterval = 3f;
    public float bulletSpeed = 25f;
    public int bulletDamage = 2;

    public override void Apply(GameObject target)
    {
        AutoTurret turret = target.GetComponent<AutoTurret>();
        if (turret == null)
        {
            turret = target.AddComponent<AutoTurret>();
        }

        turret.Initialize(bulletPrefab, fireInterval, bulletSpeed, bulletDamage);
    }
}
