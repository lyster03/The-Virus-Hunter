using UnityEngine;

public class HeartPickup : MonoBehaviour
{

    public AudioClip pickupSound;
    public GameObject pickupEffect;
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
            SoundFXManager.Instance.PlaySoundFXClip(pickupSound, transform, 1f);
            Destroy(gameObject); 
        }
    }
}
