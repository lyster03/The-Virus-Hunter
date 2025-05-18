using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawnerV2 : MonoBehaviour
{
    [Header("Wave Settings")]
    public WaveV2[] waves;
    private WaveV2 currentWave;
    private int currentWaveIndex = 0;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private Transform castle;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    [Header("Wave UI")]
    [SerializeField] private WaveUI waveUI;
    [SerializeField] private float waveAnimationDuration = 2f;

    private bool stopSpawning = false;
    private bool waveInProgress = false;
    private bool isSpawningEnemies = false;
    private bool boundsSwitched = false;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    [Header("Events")]
    public UnityEvent OnWaveCompleted;

    
    private int walkerExtraHP = 0;
    private float walkerExtraSpeed = 0f;
    private int pistoleroExtraHP = 0;
    private float pistoleroExtraSpeed = 0f;
    private int tankExtraHP = 0;
    private float tankExtraSpeed = 0f;

    [Header("Enemy Prefabs (Order Matters)")]
    [SerializeField] private GameObject walkerPrefab;
    [SerializeField] private GameObject pistoleroPrefab;
    [SerializeField] private GameObject tankPrefab;

    [Header("Testing / Debugging")]
    public int debugStartWaveIndex = -1;
    public bool killAllEnemies = false;

    [Header("Camera Confiner")]
    public CameraConfinerSwitcher cameraConfinerSwitcher;

    [Header("Crack System")]
    [SerializeField] private List<GameObject> crackObjects;
    [SerializeField] private int crackStartWave = 3;
    [SerializeField] private int crackFrequency = 3;

    [Header("Screen Flash")]
    [SerializeField] private CanvasGroup flashImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private GameObject objectToToggleDuringBlackScreen;

    public AudioClip crackSound;

    // Initialize important references and settings for waves
    private void Awake()
    {
        if (waves.Length == 0)
        {
            Debug.LogWarning("No waves assigned to WaveSpawnerV2.");
            stopSpawning = true;
            return;
        }

        currentWave = waves[currentWaveIndex];

        if (castle == null)
        {
            GameObject castleObj = GameObject.FindGameObjectWithTag("Castle");
            if (castleObj != null)
                castle = castleObj.transform;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Start the wave sequence based on the debug setting or default wave order
    private void Start()
    {
        if (debugStartWaveIndex >= 0 && debugStartWaveIndex < waves.Length)
        {
            currentWaveIndex = debugStartWaveIndex;
            currentWave = waves[currentWaveIndex];
            Debug.Log($"Debug override: starting from wave {currentWaveIndex + 1}");
        }

        StartCoroutine(BeginNextWaveWithDelay(currentWave.TimeBeforeThisWave));
    }

    // Update each frame to manage enemy spawning and wave completion
    private void Update()
    {
        if (killAllEnemies)
        {
            KillAllEnemies();
            killAllEnemies = false;
        }

        if (stopSpawning || !waveInProgress || isSpawningEnemies) return;

        // Check player health and stop spawning if player is dead
        if (player != null)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null && health.currentHP <= 0)
            {
                Debug.Log("Player is dead. Spawning stopped.");
                stopSpawning = true;
                return;
            }
        }

        aliveEnemies.RemoveAll(e => e == null);

        // If all enemies are dead, complete the wave and move to the next one
        if (aliveEnemies.Count == 0)
        {
            waveInProgress = false;
            OnWaveCompleted?.Invoke();

            if ((currentWaveIndex + 1) >= crackStartWave &&
                ((currentWaveIndex + 1 - crackStartWave) % crackFrequency == 0))
            {
                StartCoroutine(ShakeAndActivateCracks());
            }

            if (currentWaveIndex + 1 < waves.Length)
            {
                currentWaveIndex++;
                currentWave = waves[currentWaveIndex];

                
                StartCoroutine(BeginNextWaveWithDelay(currentWave.TimeBeforeThisWave));
            }
            else
            {
                Debug.Log("All waves completed.");
                stopSpawning = true;

                if (cameraConfinerSwitcher != null && !boundsSwitched)
                {
                    cameraConfinerSwitcher.SwitchToBossBounds();
                    boundsSwitched = true;
                    Debug.Log("Switched to boss camera bounds.");
                }
            }
        }
    }

   

    // Start the next wave after a delay
    private IEnumerator BeginNextWaveWithDelay(float delay)
    {
        Debug.Log($"Wave {currentWaveIndex + 1} starting in {delay} seconds...");
        yield return new WaitForSeconds(delay);

        if (waveUI != null)
        {
            waveUI.ShowWave(currentWaveIndex + 1);
        }

        yield return new WaitForSeconds(waveAnimationDuration);

        Debug.Log("Spawning wave: " + (currentWaveIndex + 1));
        waveInProgress = true;
        SpawnWave();
    }

    // Spawn the enemies for the current wave
    private void SpawnWave()
    {
        isSpawningEnemies = true;
        int totalTypes = currentWave.EnemyTypes.Length;
        int completedTypes = 0;

        List<Transform> shuffledSpawnPoints = GetShuffledSpawnPoints();

        for (int j = 0; j < totalTypes; j++)
        {
            StartCoroutine(SpawnEnemyTypeWithDelay(j, shuffledSpawnPoints, () =>
            {
                completedTypes++;
                if (completedTypes >= totalTypes)
                {
                    isSpawningEnemies = false;
                }
            }));
        }
    }

    // Spawn a specific type of enemy with a delay between each spawn
    private IEnumerator SpawnEnemyTypeWithDelay(int index, List<Transform> spawnPoints, System.Action onComplete)
    {
        int numToSpawn = Mathf.RoundToInt(currentWave.NumberTypesToSpawn[index]);

        for (int i = 0; i < numToSpawn; i++)
        {
            int spawnIndex = i % spawnPoints.Count;
            SpawnEnemy(index, spawnPoints[spawnIndex]);
            yield return new WaitForSeconds(0.5f);
        }

        onComplete?.Invoke();
    }

    // Instantiate an enemy at a spawn point
    private void SpawnEnemy(int index, Transform spawnPoint)
    {
        if (currentWave.EnemyTypes[index] != null)
        {
            GameObject enemy = Instantiate(
                currentWave.EnemyTypes[index].prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            aliveEnemies.Add(enemy);

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.SetCastle(castle);
                enemyComponent.SetSpawner(this);
              
            }
        }
        else
        {
            Debug.LogWarning($"Enemy type at index {index} is null!");
        }
    }

   
   

    // Shuffle the spawn points for randomness
    private List<Transform> GetShuffledSpawnPoints()
    {
        List<Transform> shuffled = new List<Transform>(spawnpoints);
        for (int i = 0; i < shuffled.Count; i++)
        {
            Transform temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }
        return shuffled;
    }

    // Called when an enemy dies to remove it from the alive list
    public void EnemyDied(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    // Kill all currently spawned enemies
    public void KillAllEnemies()
    {
        foreach (GameObject enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        aliveEnemies.Clear();
        Debug.Log("All enemies killed.");
    }

    // Check if all waves are completed
    public bool HasCompletedAllWaves()
    {
        return stopSpawning && currentWaveIndex >= waves.Length - 1;
    }

    // Shake the camera and activate crack effects
    private IEnumerator ShakeAndActivateCracks()
    {
        SoundFXManager.Instance.PlaySoundFXClip(crackSound, transform, 1f);
        CinemachineShake.Instance?.ShakeCamera(40f, 1f);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FlashScreen());
        ActivateRandomCracks();
        yield return new WaitForSeconds(0.3f);
        CinemachineShake.Instance?.ShakeCamera(40f, 1f);
    }

    // Flash the screen to indicate a crack event
    private IEnumerator FlashScreen()
    {
        if (flashImage == null) yield break;

        if (objectToToggleDuringBlackScreen != null)
            objectToToggleDuringBlackScreen.SetActive(false);

        flashImage.alpha = 1f;
        flashImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        flashImage.alpha = 0f;
        flashImage.gameObject.SetActive(false);

        if (objectToToggleDuringBlackScreen != null)
            objectToToggleDuringBlackScreen.SetActive(true);
    }

    // Activate random cracks in the scene for dramatic effect
    private void ActivateRandomCracks()
    {
        int cracksToActivate = Random.Range(3, 6);
        List<GameObject> inactiveCracks = crackObjects.FindAll(obj => !obj.activeSelf);
        if (inactiveCracks.Count <= 0) return;

        for (int i = 0; i < cracksToActivate && inactiveCracks.Count > 0; i++)
        {
            int index = Random.Range(0, inactiveCracks.Count);
            inactiveCracks[index].SetActive(true);
            inactiveCracks.RemoveAt(index);
        }
        Debug.Log($"Activated {cracksToActivate} crack(s).");
    }
}
