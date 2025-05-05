using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Double Shot")]
public class DoubleShotPowerUp : PowerUp
{
    public override void Apply(GameObject player)
    {
        Shooting shooter = player.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.EnableDoubleShot();
        }
    }
}
