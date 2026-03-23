using System;
using System.Collections;
using UnityEngine;

public class PlayerDodgeSystem : MonoBehaviour
{
    [SerializeField] private float dodgeDuration;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private int startDodgeAmount;
    [SerializeField] private int maxDodgeAmount;

    private bool _isDodging;
    private SideType _currentDodgeSide;
    private float _currentDodgeAmount;
    private bool _canDodge;

    private void Start()
    {
        _currentDodgeAmount = startDodgeAmount;
        _canDodge = true;
    }

    private void OnEnable()
    {
        CharacterInput.Instance.OnDodgeInput += DoDodge;
    }
    
    private void OnDisable()
    {
        CharacterInput.Instance.OnDodgeInput -= DoDodge;
    }

    private void DoDodge(SideType dodgeSide)
    {
        if (!_canDodge) return;

        StartCoroutine(DodgeCoroutine(dodgeSide));
    }
    
    private IEnumerator DodgeCoroutine(SideType dodgeSide)
    {
        _currentDodgeAmount--;
        _canDodge = false; 
        _isDodging = true;
        _currentDodgeSide = dodgeSide;

        yield return new WaitForSeconds(dodgeDuration);
        
        _isDodging = false;
        _currentDodgeSide = SideType.None;

        yield return new WaitForSeconds(cooldownDuration);

        if (_currentDodgeAmount > 0)
            _canDodge = true;
    }
}
