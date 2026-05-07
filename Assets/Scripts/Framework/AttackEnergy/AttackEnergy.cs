using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnergyType
{
    public string Name;
    public Slider EnergyBar;
    public float CurrentEnergy;
    public float MaxEnergy = 100f;
    public float GainPerMatch = 10f;
}

public class AttackEnergy : MonoBehaviour
{
    [SerializeField] private List<EnergyType> energyTypes = new List<EnergyType>();
    [SerializeField] private float decayRate = 2f;

    private void Start()
    {
        foreach (var energy in energyTypes)
        {
            energy.CurrentEnergy = Mathf.Clamp(energy.CurrentEnergy, 0f, energy.MaxEnergy);
            UpdateBar(energy);
        }
    }

    private void Update()
    {
        float decay = decayRate * Time.deltaTime;

        foreach (var energy in energyTypes)
        {
            if (energy.CurrentEnergy > 0f)
            {
                energy.CurrentEnergy = Mathf.Max(0f, energy.CurrentEnergy - decay);
                UpdateBar(energy);
            }
        }
    }

    public void OnMatch(int index)
    {
        if (!IsValidIndex(index)) return;

        var energy = energyTypes[index];
        energy.CurrentEnergy = Mathf.Min(energy.MaxEnergy, energy.CurrentEnergy + energy.GainPerMatch);
        UpdateBar(energy);

        if (energy.CurrentEnergy >= energy.MaxEnergy)
        {
            TriggerAttack(index);
            energy.CurrentEnergy = 0f;
            UpdateBar(energy);
        }
    }

    private void TriggerAttack(int index)
    {
        Debug.Log($"Attack triggered for bar {index} ({energyTypes[index].Name})");
    }

    private void UpdateBar(EnergyType energy)
    {
        if (energy.EnergyBar == null) return;
        energy.EnergyBar.maxValue = energy.MaxEnergy;
        energy.EnergyBar.value = energy.CurrentEnergy;
    }

    private bool IsValidIndex(int index) => index >= 0 && index < energyTypes.Count;
}