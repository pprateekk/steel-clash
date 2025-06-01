using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void UpdateHealthBar(float currentHealth, float maxHealthVal)
    {
        slider.value = currentHealth / maxHealthVal;
    }
}
