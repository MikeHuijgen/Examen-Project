using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider delaySlider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private TextMeshProUGUI sliderTextHealth;
    [SerializeField] private Image fill;
    
    [SerializeField] private float delaySpeed;
    
    [SerializeField] private float delayTimer;

    private float _currentDelayTime;

    private void Start()
    {
        _currentDelayTime = delayTimer;

        healthSlider.interactable = false;
        delaySlider.interactable = false;
    }

    private void Update()
    {
        if (healthSlider.value == delaySlider.value)return;
        _currentDelayTime -= Time.deltaTime;
        if (_currentDelayTime > 0) return;
        if (delaySlider.value > healthSlider.value)
        {
            delaySlider.value -= Time.deltaTime * delaySpeed;
        }
        else
        {
            delaySlider.value = healthSlider.value;
            _currentDelayTime = delayTimer;
        }
    }
    public void SetSliderMaxValue(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        delaySlider.maxValue = maxHealth;
        delaySlider.value = healthSlider.value;
        fill.color = gradient.Evaluate(1f);
    }

     public void UpdateHealthUI(float currentHealth)
    {
        sliderTextHealth.text = (currentHealth).ToString();
        healthSlider.value = currentHealth;
        _currentDelayTime = delayTimer; 
        fill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }
}
