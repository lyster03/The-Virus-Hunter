using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PowerUpSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;

    private PowerUp powerUp;

    public void Setup(PowerUp data, Action<PowerUp> onSelected)
    {
        
        if (icon == null || titleText == null || descriptionText == null || selectButton == null)
        {
            Debug.LogError("PowerUpSlotUI: UI reference missing!", this);
            return;
        }

        powerUp = data;
        icon.sprite = data.icon;
        titleText.text = data.title;
        descriptionText.text = data.description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke(powerUp));
    }
}
