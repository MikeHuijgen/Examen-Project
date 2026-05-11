using System;
using System.Collections;
using UnityEngine;

public class PlayerDodgeSystem : MonoBehaviour
{
    [SerializeField] private float dodgeDuration;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private int startDodgeAmount;
    [SerializeField] private int maxDodgeAmount;
    [SerializeField] private Animator _characterAnimator;

    private bool _isDodging;
    private SideType _currentDodgeSide;
    private float _currentDodgeAmount;
    private bool _canDodge;

    private void Start()
    {
        _currentDodgeAmount = startDodgeAmount;
        _canDodge = true;
    }

    private void OnEnable() => CharacterInput.Instance.OnDodgeInput += DoDodge;
    
    private void OnDisable() => CharacterInput.Instance.OnDodgeInput -= DoDodge;

    private void DoDodge(SideType dodgeSide)
    {
        if (!_canDodge) return;

        AudioManager.Instance.PlaySound("Dodge");
        StartCoroutine(DodgeCoroutine(dodgeSide));
    }
    
    private IEnumerator DodgeCoroutine(SideType dodgeSide)
    {
        _currentDodgeAmount--;
        _canDodge = false; 
        _isDodging = true;
        _currentDodgeSide = dodgeSide;
        SetDodgeAnimation(_isDodging, _currentDodgeSide);

        yield return new WaitForSeconds(dodgeDuration);
        
        _isDodging = false;
        _currentDodgeSide = SideType.None;
        SetDodgeAnimation(_isDodging, _currentDodgeSide);

        yield return new WaitForSeconds(cooldownDuration);

        if (_currentDodgeAmount > 0) _canDodge = true;
    }

    public void SetDodgeAnimation(bool isDodging, SideType dodgeSide)
    {
        _characterAnimator.SetBool("IsDodging", isDodging);
        _characterAnimator.SetInteger("SideTypeInt", (int)dodgeSide);
    }
    
    public (bool isDodging, SideType dodgeSide) GetCurrentDodgeInfo() => (_isDodging, _currentDodgeSide);

    public void AddDodge(float dodgeAmount) => _currentDodgeAmount += dodgeAmount;
}
