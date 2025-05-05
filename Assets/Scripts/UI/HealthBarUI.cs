using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{

    [SerializeField]
    private UnityEngine.UI.Image _healthBarForeGroundImageCastle;  

    
    public void UpdateHealthBar(Castle castle)
    {
        _healthBarForeGroundImageCastle.fillAmount = (float)castle.castleHealth / (float)castle.maxCastleHealth;
    }
    [SerializeField]
    private UnityEngine.UI.Image _healthBarForeGroundImage;

   
}
