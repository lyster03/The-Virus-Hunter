using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldVisual;
    public float cooldown = 15f;

    [Header("Audio Settings")]
    public AudioClip shieldActivate;

    private bool isOnCooldown = false;
    private bool shieldActive = false;

    void Start()
    {
        shieldActive = false;
    }

    public void ActivateShield()
    {
        if (!isOnCooldown)
        {
            shieldActive = true;

            if (shieldVisual != null)
                shieldVisual.SetActive(true);

        }
    }

    private void DeactivateShield()
    {
        shieldActive = false;
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
        SoundFXManager.Instance.PlaySoundFXClip(shieldActivate, transform, 0.6f);
    }

    public bool TryNegateDamage(GameObject player)
    {
        if (shieldActive)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.AddHealth(1);
            }

            DeactivateShield();

            if (!isOnCooldown)
                StartCoroutine(ShieldCooldown());

            return true;
        }

        return false;
    }

    private IEnumerator ShieldCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
        ActivateShield();
    }

    public bool IsShieldActive()
    {
        return shieldActive;
    }
}
