using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("UI References")]
    public GameObject canvas;
    public List<PowerUpSlotUI> slots;

    [Header("PowerUps")]
    public List<PowerUp> allPowerUps;

    private GameObject player;
    private int queuedPowerUpChoices = 0;
    private RectTransform canvasRect;
    private Coroutine currentAnim;

    [Header("Inventory UI")]
    public Transform powerUpInventoryPanel;
    public GameObject powerUpIconPrefab;
    private int powerUpIconCount = 0;
    public float iconSpacing = 75f;
    public GameObject borderImage;

    public AudioClip powerUpSound;


    private void Awake()
    {
        Instance = this;
        canvasRect = canvas.GetComponent<RectTransform>();
        canvas.SetActive(false);
    }

    // Called at the beginning to find the player GameObject
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
            Debug.LogError("PowerUpManager: No GameObject tagged 'Player' found in scene.");
    }

    // Queues a number of power-up selections
    public void QueuePowerUpSelections(int levelsGained)
    {
        queuedPowerUpChoices += levelsGained;

        if (!canvas.activeInHierarchy)
        {
            ShowNextPowerUpSelection();
        }
    }

    // Displays the power-up selection UI with animation
    private void ShowNextPowerUpSelection()
    {
        if (queuedPowerUpChoices <= 0 || allPowerUps.Count == 0)
        {
            CloseUI();
            return;
        }

        Time.timeScale = 0f;
        canvas.SetActive(true);
        canvasRect.localScale = Vector3.one * 0.1f;

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateScale(canvasRect, Vector3.one * 0.1f, Vector3.one * 1.1f, Vector3.one, 0.1f));

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

    // Called when a power-up is selected
    private void OnPowerUpSelected(PowerUp selected)
    {
        selected.Apply(player);
        allPowerUps.Remove(selected);
        queuedPowerUpChoices--;

        AddPowerUpToInventoryUI(selected);

        if (queuedPowerUpChoices > 0)
        {
            ShowNextPowerUpSelection();
        }
        else
        {
            if (currentAnim != null) StopCoroutine(currentAnim);
            currentAnim = StartCoroutine(CloseWithAnimation());
        }

        SoundFXManager.Instance.PlaySoundFXClip(powerUpSound, transform, 1f);

    }

    // Adds the selected power-up to the inventory UI
    private void AddPowerUpToInventoryUI(PowerUp powerUp)
    {
        
        GameObject borderGO = Instantiate(borderImage, powerUpInventoryPanel);
        RectTransform borderRT = borderGO.GetComponent<RectTransform>();
        borderRT.anchorMin = new Vector2(0, 1);
        borderRT.anchorMax = new Vector2(0, 1);
        borderRT.pivot = new Vector2(0, 1);
        borderRT.anchoredPosition = new Vector2(powerUpIconCount * iconSpacing, 0f);

        
        GameObject iconGO = Instantiate(powerUpIconPrefab, borderGO.transform);
        RectTransform iconRT = iconGO.GetComponent<RectTransform>();

        
        iconRT.anchorMin = new Vector2(0.5f, 0.5f);
        iconRT.anchorMax = new Vector2(0.5f, 0.5f);
        iconRT.pivot = new Vector2(0.5f, 0.5f);
        iconRT.anchoredPosition = Vector2.zero;

        
        Image iconImage = iconGO.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = powerUp.icon;
        }

        

        powerUpIconCount++;
    }


    // Coroutine that closes the UI with a scale animation
    private IEnumerator CloseWithAnimation()
    {
        yield return AnimateScale(canvasRect, Vector3.one, Vector3.one * 1.1f, Vector3.one * 0.1f, 0.1f);
        CloseUI();
    }

    // Deactivates the power-up UI and resumes time
    private void CloseUI()
    {
        canvas.SetActive(false);
        Time.timeScale = 1f;
    }

    // Animates the scale of a RectTransform from start to middle to end
    private IEnumerator AnimateScale(RectTransform target, Vector3 start, Vector3 middle, Vector3 end, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            target.localScale = Vector3.Lerp(start, middle, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            target.localScale = Vector3.Lerp(middle, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        target.localScale = end;
    }
}