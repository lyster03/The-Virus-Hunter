using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public List<Transform> enemies = new List<Transform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(Transform enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void Unregister(Transform enemy)
    {
        enemies.Remove(enemy);
    }

    public Transform GetClosestEnemy(Vector2 fromPosition, float radius)
    {
        Transform closest = null;
        float closestDistance = radius;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(fromPosition, enemy.position);
            if (distance < closestDistance)
            {
                closest = enemy;
                closestDistance = distance;
            }
        }

        return closest;
    }
}
