using UnityEngine;

[CreateAssetMenu(fileName = "New SpeedBoost", menuName = "PowerUps/SpeedBoost")]
public class SpeedBoostPowerUp : PowerUp
{
    public float speedIncreaseAmount = 1.25f;  

    public override void Apply(GameObject target)
    {
        
        Player player = target.GetComponent<Player>();

        if (player != null)
        {
            
            player.moveSpeed *= speedIncreaseAmount;

            
        }
    }
}
