using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnergyType
{
    public BaseMatchEffect MatchEffect;
    public string DisplayName;
    public Slider EnergyBar;
    public Color DisplayColor = Color.white;
    public float CurrentEnergy;
    public float MaxEnergy = 100f;
    public float BaseGain = 8f;
}