using UnityEngine;

public class SpawnOnEnemyDeath : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int numberToSpawn = 2;
    public float spawnOffset = 0.5f;

    private Vector3 lastPosition;
    private bool isDead = false;

    private WaveSpawnerV2 waveSpawner;

    void Start()
    {
        // Find WaveSpawnerV2 in the scene
        waveSpawner = FindObjectOfType<WaveSpawnerV2>();
    }

    void Update()
    {
        if (!isDead)
        {
            lastPosition = transform.position;
        }
    }

    public void NotifyDeath()
    {
        isDead = true;

        if (enemyPrefab != null && SpawnManager.Instance != null)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                Vector3 offset = Random.insideUnitCircle.normalized * spawnOffset;
                Vector3 spawnPosition = lastPosition + offset;

                GameObject spawned = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Register with the wave spawner
                if (waveSpawner != null)
                {
                    waveSpawner.EnemySpawned(spawned);
                }

                // Optionally set spawner on the new enemy
                Enemy enemyComponent = spawned.GetComponent<Enemy>();
                if (enemyComponent != null && waveSpawner != null)
                {
                    enemyComponent.SetSpawner(waveSpawner);
                }
            }
        }
    }
}
