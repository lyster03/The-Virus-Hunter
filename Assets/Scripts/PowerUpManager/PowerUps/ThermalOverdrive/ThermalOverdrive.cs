using UnityEngine;

[CreateAssetMenu(fileName = "FireRateUp", menuName = "PowerUps/Fire Rate Up")]
public class FireRateUp : PowerUp
{
    public float fireRateMultiplier = 0.8f; 

    public override void Apply(GameObject target)
    {
        Shooting shooter = target.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.fireRate *= fireRateMultiplier;
            Debug.Log($"[PowerUp] Fire Rate upgraded. New Fire Rate: {shooter.fireRate}");
        }
    }
}
