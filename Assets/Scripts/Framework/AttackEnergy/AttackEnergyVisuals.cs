using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AttackEnergyVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AttackEnergy attackEnergy;

    [Header("Bar Tween")]
    [SerializeField] private float barTweenDuration = 0.25f;
    [SerializeField] private Ease barTweenEase = Ease.OutQuad;

    [Header("Popup Text")]
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private float popupFadeDelay = 0.2f;
    [SerializeField] private float popupFadeDuration = 0.4f;
    [SerializeField] private float popupMoveDistance = 30f;

    private readonly Dictionary<Slider, Tween> barTweens = new();
    private Sequence popupSequence;
    private Vector3 popupStartPos;

    private void Awake()
    {
        if (popupRect != null)
        {
            popupStartPos = popupRect.position;
        }

        if (popupText != null)
        {
            popupText.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnEnergyChanged += HandleEnergyChanged;
            attackEnergy.OnMatchGained += HandleMatchGained;
        }
    }

    private void OnDisable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnEnergyChanged -= HandleEnergyChanged;
            attackEnergy.OnMatchGained -= HandleMatchGained;
        }
    }

    private void HandleEnergyChanged(EnergyType energy, float previous, float current)
    {
        if (energy.EnergyBar == null) return;

        energy.EnergyBar.maxValue = energy.MaxEnergy;

        if (barTweens.TryGetValue(energy.EnergyBar, out var tween) && tween.IsActive())
        {
            tween.Kill();
        }

        barTweens[energy.EnergyBar] = energy.EnergyBar
            .DOValue(current, barTweenDuration)
            .SetEase(barTweenEase);
    }

    private void HandleMatchGained(EnergyType energy, float gained)
    {
        if (popupText == null || popupRect == null) return;

        string label = string.IsNullOrWhiteSpace(energy.DisplayName) ? energy.AttackType.name : energy.DisplayName;
        popupText.text = $"+{gained:0} {label} Energy";
        popupText.color = energy.Color;

        popupSequence?.Kill();
        popupRect.position = popupStartPos;
        popupText.alpha = 0f;

        popupSequence = DOTween.Sequence();
        popupSequence.Append(popupText.DOFade(1f, 0.15f));
        popupSequence.Join(popupRect.DOMoveY(popupStartPos.y + popupMoveDistance, popupFadeDelay + popupFadeDuration));
        popupSequence.AppendInterval(popupFadeDelay);
        popupSequence.Append(popupText.DOFade(0f, popupFadeDuration));
    }
}