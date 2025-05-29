using System.Collections;
using UnityEngine;

public class BossTriggerManager : MonoBehaviour
{
    [Header("Boss Logic")]
    public GameObject boss;
    public GameObject bossHealthBar;

    [Header("Objects to Disable")]
    public GameObject[] objectsToDisable;

    [Header("Objects to Enable (Arena)")]
    public GameObject[] objectsToEnable;

    [Header("Wave Spawner Reference")]
    public WaveSpawnerV2 waveSpawner;

    [Header("Animator Settings")]
    public GameObject bossIntroUI;
    public Animator bossIntroAnimator;
    public string bossIntroTrigger = "StartBossIntro";

    [Header("Timing")]
    public float waitBeforeTransition = 5f;
    public float transitionDuration = 2f;

    [Header("Player Reference")]
    public Transform playerTransform;
    public Vector3 playerBossPosition = new Vector3(0f, 20f, 0f);

    private bool bossTriggered = false;

    void Update()
    {
        // Wait until all waves are complete, then trigger boss sequence
        if (!bossTriggered && waveSpawner != null && waveSpawner.HasCompletedAllWaves())
        {
            StartCoroutine(TriggerBossSequence());
        }
    }

    private IEnumerator TriggerBossSequence()
    {
        bossTriggered = true;

        // Camera shake for dramatic effect
        CinemachineShake.Instance?.ShakeCamera(30f, waitBeforeTransition + transitionDuration + 1f);

        yield return new WaitForSeconds(waitBeforeTransition);

        // Play boss intro animation
        if (bossIntroUI != null)
            bossIntroUI.SetActive(true);

        if (bossIntroAnimator != null)
        {
            bossIntroAnimator.SetTrigger(bossIntroTrigger);
        }

        yield return new WaitForSeconds(1f); 

        TriggerBossPhase();
    }

    // Activates boss, arena objects, UI, and repositions player
    private void TriggerBossPhase()
    {
        foreach (GameObject obj in objectsToDisable)
            if (obj != null)
                obj.SetActive(false);

        foreach (GameObject obj in objectsToEnable)
            if (obj != null)
                obj.SetActive(true);

        if (boss != null)
            boss.SetActive(true);

        if (bossHealthBar != null)
            bossHealthBar.SetActive(true);

        if (playerTransform != null)
            playerTransform.position = playerBossPosition;
        bossIntroUI.SetActive(false);

    }
}
