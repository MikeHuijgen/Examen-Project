using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    [SerializeField] private Slider _hPSlider;
    [SerializeField] private Slider _delaySlider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private TextMeshProUGUI _SliderTextHP;
    [SerializeField] private Image fill;

    public void SetSliderMaxValue(int maxhealth)
    {
        _hPSlider.maxValue = maxhealth;
        fill.color = _gradient.Evaluate(1f);
    }

     public void UpdateHealthUI(int currentHealth)
    {
        _SliderTextHP.text = (currentHealth).ToString();
        _hPSlider.value = currentHealth;
        _delaySlider.value = math.lerp(_delaySlider.value, _hPSlider.value, Time.deltaTime);
        fill.color = _gradient.Evaluate(_hPSlider.normalizedValue);
    }
}
