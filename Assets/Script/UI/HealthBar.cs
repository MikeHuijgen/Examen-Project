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
    
    [SerializeField] private float _delaySpeed;
    
    [SerializeField] private float _delayTimer;

    private float _currentDelayTime;

    private void Start()
    {
        _currentDelayTime = _delayTimer;
    }

    private void Update()
    {
        if (_hPSlider.value == _delaySlider.value)return;
        _currentDelayTime -= Time.deltaTime;
        if (_currentDelayTime > 0) return;
        if (_delaySlider.value > _hPSlider.value)
        {
            _delaySlider.value -= Time.deltaTime * _delaySpeed;
        }
        else
        {
            _delaySlider.value = _hPSlider.value;
            _currentDelayTime = _delayTimer;
        }
    }
    public void SetSliderMaxValue(int maxhealth)
    {
        _hPSlider.maxValue = maxhealth;
        _delaySlider.maxValue = maxhealth;
        _delaySlider.value = _hPSlider.value;
        fill.color = _gradient.Evaluate(1f);
    }

     public void UpdateHealthUI(int currentHealth)
    {
        _SliderTextHP.text = (currentHealth).ToString();
        _hPSlider.value = currentHealth;
        _currentDelayTime = _delayTimer; 
        fill.color = _gradient.Evaluate(_hPSlider.normalizedValue);
    }
}
