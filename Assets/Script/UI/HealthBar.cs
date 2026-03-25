using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    [SerializeField] private Slider hPSlider;
    [SerializeField] private Slider delaySlider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private TextMeshProUGUI sliderTextHP;
    [SerializeField] private Image fill;
    
    [SerializeField] private float delaySpeed;
    
    [SerializeField] private float delayTimer;

    private float _currentDelayTime;

    private void Start()
    {
        _currentDelayTime = delayTimer;
    }

    private void Update()
    {
        if (hPSlider.value == delaySlider.value)return;
        _currentDelayTime -= Time.deltaTime;
        if (_currentDelayTime > 0) return;
        if (delaySlider.value > hPSlider.value)
        {
            delaySlider.value -= Time.deltaTime * delaySpeed;
        }
        else
        {
            delaySlider.value = hPSlider.value;
            _currentDelayTime = delayTimer;
        }
    }
    public void SetSliderMaxValue(int maxhealth)
    {
        hPSlider.maxValue = maxhealth;
        delaySlider.maxValue = maxhealth;
        delaySlider.value = hPSlider.value;
        fill.color = gradient.Evaluate(1f);
    }

     public void UpdateHealthUI(int currentHealth)
    {
        sliderTextHP.text = (currentHealth).ToString();
        hPSlider.value = currentHealth;
        _currentDelayTime = delayTimer; 
        fill.color = gradient.Evaluate(hPSlider.normalizedValue);
    }
}
