using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [Header("Visual & Audio")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    [HideInInspector] public HeartPickupSpawner spawner;
    [HideInInspector] public int spawnIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        int before = health.CurrentHP;

        health.AddHealth(2);

        if (health.CurrentHP > before)
        {
            if (pickupEffect != null)
            {
                GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (pickupSound != null)
            {
                SoundFXManager.Instance.PlaySoundFXClip(pickupSound, transform, 1f);
            }

           
            if (spawner != null)
            {
                spawner.FreeSpawnPoint(spawnIndex);
            }

            Destroy(gameObject);
        }
    }
}
