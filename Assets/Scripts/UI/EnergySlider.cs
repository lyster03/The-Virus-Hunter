using UnityEngine;
using UnityEngine.UI;

public class EnergySliderController : MonoBehaviour
{
    public Shooting shootingScript;
    public Slider energySlider;
    public Image fillImage;
    public float smoothSpeed = 5f;

    void Update()
    {
        if (shootingScript == null || energySlider == null || fillImage == null) return;

        float currentPercent = shootingScript.currentEnergy / shootingScript.maxEnergy;
        energySlider.value = Mathf.Lerp(energySlider.value, currentPercent, Time.deltaTime * smoothSpeed);

        // color gradient
        fillImage.color = Color.Lerp(Color.red, Color.green, currentPercent);
    }
}
