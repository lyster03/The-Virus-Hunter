using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience Progression")]
    [SerializeField] int baseExperience = 100;            
    [SerializeField] float growthFactor = 1.24f;         

    public int currentLevel = 1;                         
    [SerializeField] int totalExperience;                
    private int previousLevelsExperience;             
    private int nextLevelsExperience;              

    [Header("XP UI")]
    [SerializeField] Image[] xpSegments;               
    [SerializeField] GameObject[] xpCompletePrefabs;      

    [Header("Power-Up System")]
    [SerializeField] PowerUpManager powerUpManager;
    [SerializeField] AudioClip LvlPowerUpAudio;

    void Start()
    {
        CalculateLevelBoundaries();
        UpdateInterface(); 
    }

   
    public void AddExperience(int amount)
    {
        totalExperience += amount;

        int levelsGained = 0;

        
        while (totalExperience >= nextLevelsExperience)
        {
            SoundFXManager.Instance.PlaySoundFXClip(LvlPowerUpAudio, transform, 0.6f);

            currentLevel++;
            levelsGained++;
            CalculateLevelBoundaries();
        }

        UpdateInterface();  

        // If the player has leveled up, queue power-up rewards
        if (levelsGained > 0 && powerUpManager != null)
        {
            powerUpManager.QueuePowerUpSelections(levelsGained);
        }
    }

    
    void CalculateLevelBoundaries()
    {
        previousLevelsExperience = GetCumulativeExperienceForLevel(currentLevel);
        nextLevelsExperience = GetCumulativeExperienceForLevel(currentLevel + 1);
    }

    
    void UpdateInterface()
    {
        int gainedXP = totalExperience - previousLevelsExperience;
        int requiredXP = nextLevelsExperience - previousLevelsExperience;
        float fillPerBar = requiredXP / (float)xpSegments.Length;

        
        for (int i = 0; i < xpSegments.Length; i++)
        {
            float xpInBar = Mathf.Clamp(gainedXP - (i * fillPerBar), 0, fillPerBar);
            float fillAmount = xpInBar / fillPerBar;
            xpSegments[i].fillAmount = fillAmount;

            
            if (xpCompletePrefabs != null && i < xpCompletePrefabs.Length && xpCompletePrefabs[i] != null)
            {
                bool shouldBeActive = fillAmount >= 1f;
                if (xpCompletePrefabs[i].activeSelf != shouldBeActive)
                {
                    xpCompletePrefabs[i].SetActive(shouldBeActive);
                }
            }
        }
    }

   
    int GetCumulativeExperienceForLevel(int level)
    {
        float totalXP = 0f;
        for (int i = 1; i < level; i++)
        {
            totalXP += baseExperience * Mathf.Pow(growthFactor, i - 1); 
        }
        return Mathf.FloorToInt(totalXP);
    }
}
