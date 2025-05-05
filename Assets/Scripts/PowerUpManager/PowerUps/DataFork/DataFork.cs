using UnityEngine;

[CreateAssetMenu(fileName = "DataFork", menuName = "PowerUps/Data Fork")]
public class DataFork : PowerUp
{
    public GameObject forkedBulletPrefab;

    public override void Apply(GameObject target)
    {
        Shooting shooter = target.GetComponent<Shooting>();
        if (shooter != null)
        {
            shooter.enableFork = true;
            shooter.forkedBulletPrefab = forkedBulletPrefab;
            
        }
    }
}
