using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Reverse Shot")]
public class ReverseShotPowerUp : PowerUp
{
    public override void Apply(GameObject target)
    {
        if (target.TryGetComponent(out ReverseShot reverseShot))
        {
            reverseShot.enabled = true;
        }
        else
        {
            target.AddComponent<ReverseShot>();
        }
    }
}
