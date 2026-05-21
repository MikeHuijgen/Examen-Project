using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackEnergy : MonoBehaviour
{
    [SerializeField] private List<EnergyType> energyTypes = new List<EnergyType>();
    [SerializeField] private float decayRate = 2f;
    [SerializeField] private ComboCounter comboCounter;

    [Header("Combo Gain Scaling")]
    [SerializeField] private float maxIncrease = 16f;
    [SerializeField] private int maxComboForScaling = 20;

    [Header("Effect Channels")]
    [SerializeField] private MatchAttackEffectChannel matchAttackEffectChannel;
    [SerializeField] private MatchDoubleDamageEffectChannel matchDoubleDamageEffectChannel;

    private readonly Dictionary<BaseMatchEffect, EnergyType> _energyByEffect = new Dictionary<BaseMatchEffect, EnergyType>();

    public event Action<EnergyType, float, float> OnEnergyChanged;
    public event Action<EnergyType, float> OnMatchGained;

    public event Action<BaseAttack> OnAttackTriggered;
    public event Action<float> OnDoubleDamageTriggered;

    private void Awake()
    {
        _energyByEffect.Clear();

        foreach (EnergyType energy in energyTypes)
        {
            if (energy?.MatchEffect == null) continue;
            _energyByEffect[energy.MatchEffect] = energy;
        }
    }

    private void OnEnable()
    {
        matchAttackEffectChannel.OnEventRaised += HandleMatchAttack;
        matchDoubleDamageEffectChannel.OnEventRaised += HandleDoubleDamage;
    }

    private void OnDisable()
    {
        matchAttackEffectChannel.OnEventRaised -= HandleMatchAttack;
        matchDoubleDamageEffectChannel.OnEventRaised -= HandleDoubleDamage;
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

    private void HandleMatchAttack(BaseAttack attack)
    {
        if (attack == null) return;

        foreach (var pair in _energyByEffect)
        {
            if (pair.Key is MatchAttackEffect attackEffect && attackEffect.Attack == attack)
            {
                OnMatch(pair.Key);
                break;
            }
        }
    }

    private void HandleDoubleDamage(float multiplier)
    {
        foreach (var pair in _energyByEffect)
        {
            if (pair.Key is MatchDoubleDamageEffect)
            {
                OnMatch(pair.Key);
                break;
            }
        }
    }

    public void OnMatch(BaseMatchEffect effect)
    {
        if (!_energyByEffect.TryGetValue(effect, out EnergyType energy) || energy == null)
        {
            return;
        }

        var previous = energy.CurrentEnergy;
        var gained = GetScaledGain(energy);
        energy.CurrentEnergy = Mathf.Min(energy.MaxEnergy, energy.CurrentEnergy + gained);

        comboCounter.OnSuccessfulHit();
        UpdateBarMax(energy);
        OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        OnMatchGained?.Invoke(energy, gained);

        if (energy.CurrentEnergy >= energy.MaxEnergy)
        {
            OnFullEnergy(effect);
            previous = energy.CurrentEnergy;
            energy.CurrentEnergy = 0f;
            UpdateBarMax(energy);
            OnEnergyChanged?.Invoke(energy, previous, energy.CurrentEnergy);
        }
    }

    private float GetScaledGain(EnergyType energy)
    {
        var combo = comboCounter != null ? comboCounter.CurrentComboCount : 0;
        var t = Mathf.InverseLerp(0f, maxComboForScaling, combo);
        return Mathf.Lerp(energy.BaseGain, energy.BaseGain + maxIncrease, t);
    }

    private void OnFullEnergy(BaseMatchEffect matchEffect)
    {
        if (matchEffect is MatchAttackEffect attackEffect) OnAttackTriggered?.Invoke(attackEffect.Attack);
        else if (matchEffect is MatchDoubleDamageEffect doubleDamageEffect) OnDoubleDamageTriggered?.Invoke(doubleDamageEffect.Multiplier);
    }

    private void UpdateBarMax(EnergyType energy)
    {
        if (energy.EnergyBar == null) return;
        energy.EnergyBar.maxValue = energy.MaxEnergy;
    }
}