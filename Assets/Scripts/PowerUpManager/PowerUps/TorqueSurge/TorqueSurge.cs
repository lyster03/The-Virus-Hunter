using UnityEngine;

[CreateAssetMenu(fileName = "BulletForceUp", menuName = "PowerUps/Bullet Force Up")]
public class BulletForceUp : PowerUp
{
    public float forceMultiplier = 1.3f; 

    public override void Apply(GameObject target)
    {
        Shooting shooter = target.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.bulletForce *= forceMultiplier;
            Debug.Log($"[PowerUp] Bullet Force upgraded. New Bullet Force: {shooter.bulletForce}");
        }
    }
}
