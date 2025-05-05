using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    [Header("Basic Shooting")]
    public GameObject bulletPrefab1;
    public GameObject bulletPrefab0;
    public Transform firePoint;
    public float bulletForce = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public Player playerScript;
    public int bulletDamage = 1;

    [Header("Double Shot")]
    public bool isDoubleShot = false;
    public float doubleShotAngle = 10f;
    private bool useOneNext = true;

    [Header("Burst Fire")]
    public bool isBurstFire = false;
    public int burstCount = 3;
    public float burstDelay = 0.1f;
    public float burstCooldown = 0.5f;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioSource audioSource;
    public AudioClip overclockSound;

    [Header("Face Animation")]
    public Animator faceAnimator;
    public string faceAnimParameter = "isShooting";
    private float shootAnimTimer = 0f;
    public float shootAnimDuration = 2f;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyRegenRate = 10f;
    public float energyCostPerShot = 10f;
    public Slider energySlider;
    public Gradient energyGradient;
    public Image fillImage;

    [Header("Overclocking")]
    public float overclockCooldown = 2f;
    private bool isOverclocking = false;
    public Image screenFlash;

    [Header("Bullet Modifiers")]
    public float bulletSizeMultiplier = 1f;
    public float bulletForceMultiplier = 1f;
    public int bulletPierceBonus = 0;
    public bool enableFork = false;
    public GameObject forkedBulletPrefab;

    public event System.Action OnShoot;

    // Initialize energy and UI elements at the start
    void Start()
    {
        currentEnergy = maxEnergy;

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            UpdateEnergyUI();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Handle shooting, energy regeneration, and energy UI update
    void Update()
    {
        if (!PauseMenu.isPaused)
        {
            HandleShooting();
            HandleEnergyRegen();
            UpdateEnergyUI();
        }
    }

    // Handle the actual shooting of bullets
    void HandleShooting()
    {
        if (isOverclocking) return;

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextFireTime && currentEnergy >= energyCostPerShot)
        {
            if (isBurstFire)
            {
                StartCoroutine(FireBurst());
                nextFireTime = Time.time + burstCooldown;
            }
            else
            {
                FireSingleShot();
                nextFireTime = Time.time + fireRate;
            }

            currentEnergy -= energyCostPerShot;
            currentEnergy = Mathf.Max(0f, currentEnergy);

            if ((currentEnergy / maxEnergy) <= 0.01f)
                StartCoroutine(Overclock());
        }

        if (faceAnimator != null && faceAnimator.GetBool(faceAnimParameter))
        {
            shootAnimTimer -= Time.deltaTime;
            if (shootAnimTimer <= 0f)
                faceAnimator.SetBool(faceAnimParameter, false);
        }
    }

    // Fire a burst of shots in quick succession
    IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            FireSingleShot();
            yield return new WaitForSeconds(burstDelay);
        }
    }

    // Fire a single shot in the specified direction
    void FireSingleShot()
    {
        Vector2 shootDirection = (playerScript.mousePos - (Vector2)firePoint.position).normalized;
        GameObject projectileToSpawn = useOneNext ? bulletPrefab1 : bulletPrefab0;
        useOneNext = !useOneNext;
        float baseAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        if (isDoubleShot)
        {
            ShootOneBullet(baseAngle - doubleShotAngle / 2f, bulletForce, projectileToSpawn);
            ShootOneBullet(baseAngle + doubleShotAngle / 2f, bulletForce, projectileToSpawn);
        }
        else
        {
            ShootOneBullet(baseAngle, bulletForce, projectileToSpawn);
        }

        if (faceAnimator != null)
        {
            faceAnimator.SetBool(faceAnimParameter, true);
            shootAnimTimer = shootAnimDuration;
        }

        SoundFXManager.Instance.PlaySoundFXClip(shootSound, transform, 0.6f);
        OnShoot?.Invoke();
    }

    // Shoot a single bullet in a given direction
    void ShootOneBullet(float angle, float force, GameObject projectile)
    {
        if (projectile == null) return;

        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector3 firePos = firePoint.position + (Vector3)(dir * 0.2f);

        GameObject bullet = Instantiate(projectile, firePos, Quaternion.identity);
        if (bullet == null) return;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir.normalized * force * bulletForceMultiplier;

        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = bulletDamage;
            bulletScript.sizeMultiplier = bulletSizeMultiplier;
            bulletScript.pierceCount = bulletPierceBonus;

            if (enableFork && forkedBulletPrefab != null)
            {
                bulletScript.canFork = true;
                bulletScript.forkedBulletPrefab = forkedBulletPrefab;
            }
        }

        Destroy(bullet, 5f);
    }

    // Regenerate energy over time
    void HandleEnergyRegen()
    {
        if (isOverclocking) return;

        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }

    // Activate overclocking and temporarily boost energy
    IEnumerator Overclock()
    {
        isOverclocking = true;

        if (screenFlash != null)
            StartCoroutine(FlashScreenEffect());

        if (overclockSound != null)
        {
            audioSource.clip = overclockSound;
            audioSource.volume = 1.5f;
            audioSource.Play();
        }

        float targetEnergy = Mathf.Min(currentEnergy + maxEnergy * 0.2f, maxEnergy);
        float startEnergy = currentEnergy;
        float timer = 0f;

        while (timer < overclockCooldown)
        {
            timer += Time.deltaTime;
            currentEnergy = Mathf.Lerp(startEnergy, targetEnergy, timer / overclockCooldown);
            UpdateEnergyUI();
            yield return null;
        }

        isOverclocking = false;
    }

    // Flash the screen effect during overclocking
    IEnumerator FlashScreenEffect()
    {
        screenFlash.color = new Color(1f, 0f, 0f, 0.5f);
        screenFlash.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        screenFlash.color = new Color(1f, 0f, 0f, 0.25f);
        yield return new WaitForSeconds(0.2f);
        screenFlash.color = new Color(1f, 0f, 0f, 0.1f);
        yield return new WaitForSeconds(0.2f);
        screenFlash.gameObject.SetActive(false);
    }

    // Enable double shot mode
    public void EnableDoubleShot() => isDoubleShot = true;

    // Enable burst fire mode
    public void EnableBurstFire() => isBurstFire = true;

    // Set burst fire delay
    public void SetBurstDelay(float delay) => burstDelay = delay;

    // Set burst fire cooldown
    public void SetBurstCooldown(float cooldown) => burstCooldown = cooldown;

    // Add energy to the player
    public void AddEnergy(float amount)
    {
        if (isOverclocking) return;
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        UpdateEnergyUI();
    }

    // Update the energy UI slider and gradient
    void UpdateEnergyUI()
    {
        if (energySlider != null)
        {
            energySlider.value = currentEnergy;
            if (fillImage != null && energyGradient != null)
            {
                fillImage.color = energyGradient.Evaluate(currentEnergy / maxEnergy);
            }
        }
    }
}
