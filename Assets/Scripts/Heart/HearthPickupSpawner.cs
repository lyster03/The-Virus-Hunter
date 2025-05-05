using UnityEngine;
using System.Collections;

public class HeartPickupSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject heartPickupPrefab;
    public GameObject[] spawnPoints;
    public float minSpawnInterval = 30f;
    public float maxSpawnInterval = 45f;

    void Start()
    {
        if (heartPickupPrefab != null && spawnPoints.Length > 0)
        {
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogWarning("HeartPickupSpawner: Missing prefab or spawn points.");
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            SpawnHeart();
        }
    }

    void SpawnHeart()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Instantiate(heartPickupPrefab, spawnPoints[index].transform.position, Quaternion.identity);
    }
}
