using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    [SerializeField] private Slider _hPSlider;
    [SerializeField] private TextMeshProUGUI _SliderTextHP;

    public void SetSliderMaxValue(int maxhealth)
    {
        _hPSlider.maxValue = maxhealth;
    }

     public void UpdateHealthUI(int currentHealth)
    {
        _SliderTextHP.text = (currentHealth).ToString();
        _hPSlider.value = currentHealth;
    }
}
