using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 0.2f;
    
    void Start()
    {
        // Initialize health
        health = maxHealth;

        if (healthSlider != null && easeHealthSlider != null)
        {
            healthSlider.value = health;
            easeHealthSlider.value = health;
        }
    }

    void Update()
    {
        if (healthSlider != null && easeHealthSlider != null)
        {
            if (healthSlider.value != easeHealthSlider.value)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
            }
        }
    }

    public void updateHealthBar(float currHealth, float maxHealth)
    {
        health = currHealth;

        if (healthSlider != null)
        {
            healthSlider.value = currHealth;
        }
    }
}
