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

    [SerializeField] private float baseShakeStrength = 10f;
    [SerializeField] private float maxShakeStrength = 40f;

    private int _currentComboCount;
    private float _elapsedTime;
    private bool _isComboActive;

    private Tween _shakeTween;

    private void Awake()
    {
        visuals.SetActive(false);
    }

    private void Update()
    {
        if (!_isComboActive) return;

        _elapsedTime += Time.deltaTime;

        float remainingTime = 1f - (_elapsedTime / comboDepleteTime);
        comboTimerBar.value = remainingTime;

        if (_elapsedTime >= comboDepleteTime && _currentComboCount > 0)
            OnLoseCombo();
    }

    public void OnSuccessfulHit()
    {
        _currentComboCount++;
        _elapsedTime = 0f;
        _isComboActive = true;

        if (_currentComboCount > 1)
            visuals.SetActive(true);

        comboTimerBar.value = 1f;
        comboText.text = $"{_currentComboCount}X";

        visuals.transform.DOKill();
        visuals.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 1);

        HandleShake();
    }

    private void HandleShake()
    {
        if (_currentComboCount <= 5)
        {
            _shakeTween?.Kill();
            return;
        }

        float strength = baseShakeStrength + (_currentComboCount - 5) * 3f;
        strength = Mathf.Clamp(strength, baseShakeStrength, maxShakeStrength);

        _shakeTween?.Kill();

        _shakeTween = visuals.transform.DOShakePosition(0.5f, strength, 20, 90, false, false)
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
        comboText.text = "0";

        _shakeTween?.Kill();

        visuals.transform.DOKill();
        _shakeTween?.Kill();

        visuals.transform.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360);
        visuals.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                visuals.transform.DOKill();
                visuals.SetActive(false);
            });
    }

    private void OnEnable()
    {
        LevelGrid.OnMatchDestroyed += OnMatch;
    }

    private void OnDisable()
    {
        LevelGrid.OnMatchDestroyed -= OnMatch;
    }

    private void OnMatch(object _)
    {
        OnSuccessfulHit();
    }
}
