using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite heartEmpty;
    public Sprite heartHalf;
    public Sprite heartFull;

    [Header("References")]
    public GameObject heartPrefab;
    public PlayerHealth playerHealth;

    private List<Image> heartImages = new List<Image>();

    IEnumerator Start()
    {
        
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        yield return null;

        
        playerHealth.OnHealthChanged.AddListener(UpdateHearts);
        CreateHearts();
        UpdateHearts();
    }

    void CreateHearts()
    {
        // Clears old hearts and generates new hearts
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        heartImages.Clear();

        for (int i = 0; i < playerHealth.MaxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image heartImage = heart.GetComponent<Image>();
            heartImages.Add(heartImage);
        }
    }

    void UpdateHearts()
    {
        // If the heart images count doesn't match max hearts, recreate them
        if (heartImages.Count != playerHealth.MaxHearts)
        {
            CreateHearts();
        }

        int hp = playerHealth.CurrentHP;

        // Update heart sprites based on current health
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (hp >= 2)
            {
                heartImages[i].sprite = heartFull;
                hp -= 2;
            }
            else if (hp == 1)
            {
                heartImages[i].sprite = heartHalf;
                hp -= 1;
            }
            else
            {
                heartImages[i].sprite = heartEmpty;
            }
        }
    }
}
