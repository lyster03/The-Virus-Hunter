using UnityEngine;

public class ObstacleParticleTrigger : MonoBehaviour
{
    [Header("Particle Settings")]
    public GameObject particleEffectPrefab; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
                Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
    
    }
}
