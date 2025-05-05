using UnityEngine;

[CreateAssetMenu(fileName = "BulletSizeUp", menuName = "PowerUps/Bullet Size Up")]
public class BulletSizeUp : PowerUp
{
    public float sizeMultiplier = 1.5f; 

    public override void Apply(GameObject target)
    {
        Shooting shooter = target.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.bulletSizeMultiplier *= sizeMultiplier;
            
        }
    }
}
