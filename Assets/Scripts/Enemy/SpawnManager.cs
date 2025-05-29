using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnEnemies(GameObject prefab, Vector3 position, int count, float offset)
    {
        StartCoroutine(SpawnEnemiesCoroutine(prefab, position, count, offset));
    }

    private IEnumerator SpawnEnemiesCoroutine(GameObject prefab, Vector3 position, int count, float offset)
    {
        yield return null; // Wait one frame to bypass Unity's prefab rotation overwrite

        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset2D = Random.insideUnitCircle * offset;
            Vector3 spawnPos = position + new Vector3(randomOffset2D.x, randomOffset2D.y, 0f);
            spawnPos.z = 0f; // Enforce 2D plane

            GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawned.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Force proper rotation
        }
    }
}
