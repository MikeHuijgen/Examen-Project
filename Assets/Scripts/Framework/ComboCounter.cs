using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    [SerializeField] private Slider comboTimerBar;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private float comboDepleteTime = 2f;
    [SerializeField] private GameObject visuals;

    [SerializeField] private float shakeTweenDuration = 0.5f;
    [SerializeField] private float baseShakeStrength = 10f;
    [SerializeField] private float maxShakeStrength = 40f;

    [SerializeField] private float addComboTweenDuratin = 0.2f;
    [SerializeField] private float addComboScaleAmount = 0.3f;
    [SerializeField] private float resetTweenDuration = 0.5f;

    private int _currentComboCount;
    private float _elapsedTime;
    private bool _isComboActive;
    private Transform _startTransform;
    private bool _isResetting;

    private Tween _shakeTween;

    public int CurrentComboCount => _currentComboCount;

    private void Awake()
    {
        visuals.SetActive(false);
    }

    private void Start()
    {
        _startTransform = visuals.transform;
    }

    private void Update()
    {
        if (!_isComboActive) return;

        _elapsedTime += Time.deltaTime;

        var remainingTime = 1f - (_elapsedTime / comboDepleteTime);
        comboTimerBar.value = remainingTime;

        if (_elapsedTime >= comboDepleteTime && _currentComboCount > 0) OnLoseCombo();
    }

    public void OnSuccessfulHit()
    {
        if(_isResetting) return;
        
        _currentComboCount++;
        _elapsedTime = 0f;
        _isComboActive = true;

        if (_currentComboCount > 1) visuals.SetActive(true);

        comboTimerBar.value = 1f;
        comboText.text = $"{_currentComboCount}X";

        visuals.transform.DOKill();
        
        visuals.transform.DOPunchScale(Vector3.one * addComboScaleAmount, addComboTweenDuratin, 10, 1);

        HandleShake();
    }

    private void HandleShake()
    {
        if (_currentComboCount <= 5)
        {
            _shakeTween?.Kill();
            return;
        }

        var strength = baseShakeStrength + (_currentComboCount - 5) * 3f;
        strength = Mathf.Clamp(strength, baseShakeStrength, maxShakeStrength);

        _shakeTween?.Kill();

        var vibrato = 20;
        var randomness = 90;

        _shakeTween = visuals.transform.DOShakePosition(shakeTweenDuration, strength, vibrato, randomness, false, false)
            .SetLoops(-1, LoopType.Restart);
    }

    public void OnLoseCombo()
    {
        ResetCombo();
    }

    private void ResetCombo()
    {
        _currentComboCount = 0;
        _elapsedTime = 0f;
        _isComboActive = false;

        comboTimerBar.value = 0f;

        _shakeTween?.Kill();

        visuals.transform.DOKill();
        _shakeTween?.Kill();

        _isResetting = true;
        
        visuals.transform.DORotate(new Vector3(0, 0, 360f), resetTweenDuration, RotateMode.FastBeyond360).SetEase(Ease.InQuad);
        visuals.transform.DOScale(Vector3.zero, resetTweenDuration).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                visuals.transform.DOKill();
                visuals.SetActive(false);
                comboText.text = "0";
                visuals.transform.localScale = Vector3.one;
                visuals.transform.position = _startTransform.position;
                _isResetting = false;
            });
    }
}