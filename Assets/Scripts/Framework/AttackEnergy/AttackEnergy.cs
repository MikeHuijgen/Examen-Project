using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnergyType
{
    public BaseAttack AttackType;
    public string DisplayName;
    public Slider EnergyBar;
    public float CurrentEnergy;
    public float MaxEnergy = 100f;
}

public class AttackEnergy : MonoBehaviour
{
    [SerializeField] private List<EnergyType> energyTypes = new List<EnergyType>();
    [SerializeField] private float decayRate = 2f;
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private ComboCounter comboCounter;

    [Header("Combo Gain Scaling")]
    [SerializeField] private float minIncrease = 8f;
    [SerializeField] private float maxIncrease = 16f;
    [SerializeField] private int maxComboForScaling = 20;

    private readonly Dictionary<BaseAttack, EnergyType> _energyByAttack = new Dictionary<BaseAttack, EnergyType>();

    public event Action<EnergyType, float, float> OnEnergyChanged;
    public event Action<EnergyType, float> OnMatchGained;

    private void Awake()
    {
        _energyByAttack.Clear();

        foreach (EnergyType energy in energyTypes)
        {
            if (energy?.AttackType == null) continue;
            _energyByAttack[energy.AttackType] = energy;
        }
    }

    private void Start()
    {
        foreach (EnergyType energy in energyTypes)
        {
            var previous = energy.CurrentEnergy;
            energy.CurrentEnergy = Mathf.Clamp(energy.CurrentEnergy, 0f, energy.MaxEnergy);
            UpdateBarMax(energy);
            OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        }
    }

    private void Update()
    {
        var decay = decayRate * Time.deltaTime;

        foreach (EnergyType energy in energyTypes)
        {
            if (energy.CurrentEnergy <= 0f) continue;

            var previous = energy.CurrentEnergy;
            energy.CurrentEnergy = Mathf.Max(0f, energy.CurrentEnergy - decay);
            UpdateBarMax(energy);
            OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        }
    }

    public void OnMatch(BaseAttack attackType)
    {
        if (!_energyByAttack.TryGetValue(attackType, out EnergyType energy) || energy == null)
        {
            Debug.LogWarning($"No EnergyType found for attack: {attackType?.name}");
            return;
        }

        var previous = energy.CurrentEnergy;
        var gained = GetScaledGain();

        energy.CurrentEnergy = Mathf.Min(energy.MaxEnergy, energy.CurrentEnergy + gained);

        UpdateBarMax(energy);
        OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        OnMatchGained?.Invoke(energy, gained);

        if (energy.CurrentEnergy >= energy.MaxEnergy)
        {
            OnFullEnergy(energy.AttackType);
            previous = energy.CurrentEnergy;
            energy.CurrentEnergy = 0f;
            UpdateBarMax(energy);
            OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        }
    }

    private float GetScaledGain()
    {
        var combo = comboCounter != null ? comboCounter.CurrentComboCount : 0;
        var t = Mathf.InverseLerp(0f, maxComboForScaling, combo);
        return Mathf.Lerp(minIncrease, maxIncrease, t);
    }

    private void OnFullEnergy(BaseAttack attack) => attackSystem.TriggerAttack(attack);

    private void UpdateBarMax(EnergyType energy)
    {
        if (energy.EnergyBar == null) return;
        energy.EnergyBar.maxValue = energy.MaxEnergy;
    }
}