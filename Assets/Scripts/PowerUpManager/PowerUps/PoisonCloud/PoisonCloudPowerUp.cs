using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Poison Cloud")]
public class PoisonCloudPowerUp : PowerUp
{
    public GameObject poisonZonePrefab;
    public float spawnInterval = 5f;
    public float duration = 15f;

    public override void Apply(GameObject target)
    {
        PoisonCloudSpawner spawner = target.GetComponent<PoisonCloudSpawner>();
        if (spawner == null)
        {
            spawner = target.AddComponent<PoisonCloudSpawner>();
        }

        spawner.StartSpawning(poisonZonePrefab, spawnInterval, duration);
    }
}
