using UnityEngine;
using System.Collections;

public class PoisonCloudSpawner : MonoBehaviour
{
    private Coroutine spawnRoutine;

    public void StartSpawning(GameObject prefab, float interval, float totalDuration)
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnPoison(prefab, interval, totalDuration));
    }

    private IEnumerator SpawnPoison(GameObject prefab, float interval, float totalDuration)
    {
        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }
}
