using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/BurstFire")]
public class BurstFirePowerUp : PowerUp
{
    [Header("Burst Settings")]
    public int burstCount = 3;
    public float delayBetweenBullets = 0.1f;
    public float delayBetweenBursts = 0.5f; 

    public override void Apply(GameObject target)
    {
        Shooting shooting = target.GetComponent<Shooting>();
        if (shooting != null)
        {
            shooting.EnableBurstFire();
            shooting.burstCount = burstCount;
            shooting.SetBurstDelay(delayBetweenBullets);
            shooting.SetBurstCooldown(delayBetweenBursts); 
        }
    }
}
