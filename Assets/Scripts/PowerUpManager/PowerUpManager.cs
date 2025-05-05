using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("UI References")]
    public GameObject canvas;
    public Animator canvasAnimator;
    public List<PowerUpSlotUI> slots;

    [Header("PowerUps")]
    public List<PowerUp> allPowerUps;

    private GameObject player;
    private int queuedPowerUpChoices = 0;

    private void Awake()
    {
        Instance = this;

        if (canvasAnimator == null && canvas != null)
        {
            canvasAnimator = canvas.GetComponent<Animator>();
            if (canvasAnimator == null)
                Debug.LogWarning("PowerUpManager: No Animator found on Canvas!");
        }

        if (canvasAnimator != null && canvasAnimator.runtimeAnimatorController != null)
        {
            foreach (var clip in canvasAnimator.runtimeAnimatorController.animationClips)
            {
                Debug.Log($"[PowerUpAnimator] Clip: {clip.name}");
            }
        }
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
            Debug.LogError("PowerUpManager: No GameObject tagged 'Player' found in scene.");
    }

    public void QueuePowerUpSelections(int levelsGained)
    {
        queuedPowerUpChoices += levelsGained;

        if (!canvas.activeInHierarchy)
        {
            ShowNextPowerUpSelection();
        }
    }

    private void ShowNextPowerUpSelection()
    {
        if (queuedPowerUpChoices <= 0 || allPowerUps.Count == 0)
        {
            CloseUI();
            return;
        }

        Time.timeScale = 0f;
        canvas.SetActive(true);

        // Ensure the border animation starts playing
        if (canvasAnimator != null)
        {
            canvasAnimator.Rebind();                // Reset animator
            canvasAnimator.Update(0f);              // Ensure frame 0 is applied
            canvasAnimator.Play("BorderLoop", 0, 0f); // Play your looping animation (change "BorderLoop" to match your state name)
        }

        StartCoroutine(PlayPopUpNextFrame());

        var available = allPowerUps.ToList();
        int count = Mathf.Min(3, available.Count);
        var picked = available.OrderBy(_ => Random.value).Take(count).ToList();

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < picked.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Setup(picked[i], OnPowerUpSelected);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator PlayPopUpNextFrame()
    {
        yield return null;

        if (canvasAnimator != null)
        {
            canvasAnimator.Play("PowerUpPopUp", 0, 0f);
            canvasAnimator.Update(0f);
        }
    }

    private void OnPowerUpSelected(PowerUp selected)
    {
        selected.Apply(player);
        allPowerUps.Remove(selected);
        queuedPowerUpChoices--;

        if (queuedPowerUpChoices > 0)
        {
            ShowNextPowerUpSelection();
        }
        else
        {
            CloseUI();
        }
    }

    private void CloseUI()
    {
        canvas.SetActive(false);
        Time.timeScale = 1f;
    }
}
