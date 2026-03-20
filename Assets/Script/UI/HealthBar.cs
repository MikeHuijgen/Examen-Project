using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : HealthSystem
{
    [SerializeField] private Slider _hPSlider;
    [SerializeField] private TextMeshProUGUI _SliderTextHP;
    [SerializeField] private int _dammage;
   
    [SerializeField] private bool _attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hPSlider.maxValue = _maxHP;
        _hPSlider.value = _currentHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (_attack)
        {
            UpdateHealth();
        }
    }

    void UpdateHealth()
    {
        _currentHP = _currentHP - _dammage;
        _SliderTextHP.text = (_currentHP).ToString();
        _hPSlider.value = _currentHP;
        _attack = false;
    }
}
