using UnityEngine;
using UnityEngine.UI;

public class XPCell : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;                     
    public GameObject completedEffectPrefab;    

    private bool isFull = false;

    public void SetFill(float amount)
    {
        fillImage.fillAmount = amount;

        if (!isFull && amount >= 1f)
        {
            isFull = true;
            if (completedEffectPrefab != null)
                completedEffectPrefab.SetActive(true);
        }
        else if (amount < 1f)
        {
            isFull = false;
            if (completedEffectPrefab != null)
                completedEffectPrefab.SetActive(false);
        }
    }
}
