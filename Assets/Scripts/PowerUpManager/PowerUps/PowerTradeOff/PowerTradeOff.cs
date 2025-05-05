using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Glass Cannon")]
public class GlassCannon : PowerUp
{
    public float damageMultiplier = 2f;
    public float fireRateMultiplier = 2f;

    public override void Apply(GameObject target)
    {
        Shooting shooter = target.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.bulletDamage = (int)(shooter.bulletDamage * damageMultiplier);
            shooter.fireRate *= fireRateMultiplier;
        }
    }
}
