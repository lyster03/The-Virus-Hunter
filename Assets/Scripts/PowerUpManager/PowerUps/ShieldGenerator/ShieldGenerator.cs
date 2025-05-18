using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Shield Generator")]
public class ShieldGeneratorPowerUp : PowerUp
{
    public GameObject shieldPrefab;

    public override void Apply(GameObject target)
    {
        ShieldController shieldController = target.GetComponent<ShieldController>();
        if (shieldController == null)
        {
            shieldController = target.AddComponent<ShieldController>();
        }

        if (shieldController != null)
        {
            shieldController.ActivateShield();
        }
    }
}
