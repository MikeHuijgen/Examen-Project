using System.Collections;
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

    [Header("Bar Punch Scale")]
    [SerializeField] private float barPunchDuration = 0.15f;
    [SerializeField] private float barPunchScale = 0.08f;
    [SerializeField] private int barPunchVibrato = 6;

    [Header("Popup Text")]
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private float popupFadeDelay = 0.2f;
    [SerializeField] private float popupFadeDuration = 0.4f;
    [SerializeField] private float popupMoveDistance = 30f;

    private readonly Dictionary<Slider, Tween> _barTweens = new Dictionary<Slider, Tween>();
    private readonly Dictionary<Slider, bool> _pendingFullReset = new Dictionary<Slider, bool>();
    private Sequence _popupSequence;
    private Vector2 _popupStartAnchoredPos;

    private void Awake()
    {
        if (popupText != null) popupText.alpha = 0f;
    }

    private void OnEnable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnEnergyChanged += HandleEnergyChanged;
            attackEnergy.OnMatchGained += HandleMatchGained;
        }

        StartCoroutine(CachePopupPositionAfterLayout());
    }

    private void OnDisable()
    {
        if (attackEnergy != null)
        {
            attackEnergy.OnEnergyChanged -= HandleEnergyChanged;
            attackEnergy.OnMatchGained -= HandleMatchGained;
        }
    }

    private IEnumerator CachePopupPositionAfterLayout()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        if (popupRect != null) _popupStartAnchoredPos = popupRect.anchoredPosition;
    }

    private void HandleEnergyChanged(EnergyType energy, float previous, float current)
    {
        if (energy.EnergyBar == null) return;

        energy.EnergyBar.maxValue = energy.MaxEnergy;

        if (_barTweens.TryGetValue(energy.EnergyBar, out Tween tween) && tween.IsActive()) tween.Kill();

        var isFull = current >= energy.MaxEnergy - 0.001f;
        var isResetToZero = current <= 0.001f;

        if (isFull)
        {
            _pendingFullReset[energy.EnergyBar] = true;

            _barTweens[energy.EnergyBar] = energy.EnergyBar.DOValue(energy.MaxEnergy, barTweenDuration).SetEase(barTweenEase);

            PunchBar(energy.EnergyBar);
            return;
        }

        if (isResetToZero && _pendingFullReset.TryGetValue(energy.EnergyBar, out bool pending) && pending)
        {
            _pendingFullReset[energy.EnergyBar] = false;

            _barTweens[energy.EnergyBar] = energy.EnergyBar.DOValue(0f, barTweenDuration).SetEase(barTweenEase);

            return;
        }

        _barTweens[energy.EnergyBar] = energy.EnergyBar.DOValue(current, barTweenDuration).SetEase(barTweenEase);
    }

    private void PunchBar(Slider slider)
    {
        var target = slider.transform;
        target.DOKill();
        target.DOPunchScale(Vector3.one * barPunchScale, barPunchDuration, barPunchVibrato, 0.5f);
    }

    private void HandleMatchGained(EnergyType energy, float gained)
    {
        if (popupText == null || popupRect == null) return;

        var label = string.IsNullOrWhiteSpace(energy.DisplayName) ? energy.AttackType.name : energy.DisplayName;
        popupText.text = $"+{gained:0} {label} Energy";

        var fill = energy.EnergyBar.fillRect != null ? energy.EnergyBar.fillRect.GetComponent<Image>() : null;

        if (fill != null) popupText.color = fill.color;

        _popupSequence?.Kill();
        popupRect.anchoredPosition = _popupStartAnchoredPos;
        popupText.alpha = 0f;

        _popupSequence = DOTween.Sequence();
        _popupSequence.Append(popupText.DOFade(1f, 0.15f));
        _popupSequence.Join(
            popupRect.DOAnchorPosY(_popupStartAnchoredPos.y + popupMoveDistance, popupFadeDelay + popupFadeDuration)
        );
        _popupSequence.AppendInterval(popupFadeDelay);
        _popupSequence.Append(popupText.DOFade(0f, popupFadeDuration));
    }
}