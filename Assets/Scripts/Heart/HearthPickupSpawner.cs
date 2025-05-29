using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeartPickupSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject heartPickupPrefab;
    public GameObject[] spawnPoints;
    public float minSpawnInterval = 30f;
    public float maxSpawnInterval = 45f;

    private bool[] isOccupied;

    void Start()
    {
        if (heartPickupPrefab != null && spawnPoints.Length > 0)
        {
            isOccupied = new bool[spawnPoints.Length];
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

            // Wait until at least one spawn point is free
            while (!HasFreeSpawnPoint())
            {
                yield return new WaitForSeconds(1f); // check every second
            }

            SpawnHeart();
        }
    }

    void SpawnHeart()
    {
        List<int> freeIndices = new List<int>();

        // Find all free spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!isOccupied[i])
                freeIndices.Add(i);
        }

        if (freeIndices.Count > 0)
        {
            int selectedIndex = freeIndices[Random.Range(0, freeIndices.Count)];
            GameObject heart = Instantiate(heartPickupPrefab, spawnPoints[selectedIndex].transform.position, Quaternion.identity);
            isOccupied[selectedIndex] = true;

            // Register to clear the occupied flag when the heart is destroyed
            HeartPickup heartScript = heart.GetComponent<HeartPickup>();
            if (heartScript != null)
            {
                heartScript.spawner = this;
                heartScript.spawnIndex = selectedIndex;
            }
            else
            {
                Debug.LogWarning("Heart prefab is missing HeartPickup script. Spawner won't be notified on collection.");
            }
        }
    }

    public void FreeSpawnPoint(int index)
    {
        if (index >= 0 && index < isOccupied.Length)
        {
            isOccupied[index] = false;
        }
    }

    bool HasFreeSpawnPoint()
    {
        foreach (bool occupied in isOccupied)
        {
            if (!occupied)
                return true;
        }
        return false;
    }
}
