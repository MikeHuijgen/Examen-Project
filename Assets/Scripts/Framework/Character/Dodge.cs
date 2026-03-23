using System;
using System.Collections;
using UnityEngine;

public class Dodge : CharacterComponent
{
    [SerializeField] private float dodgeDuration;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private int startDodgeAmount;
    [SerializeField] private int maxDodgeAmount;

    private float _currentDodges;
    private bool _canDodge;

    private void Start()
    {
        _currentDodges = startDodgeAmount;
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
        
        Debug.Log("Character Dodging");

        StartCoroutine(DodgeCoroutine(dodgeSide));
    }
    
    private IEnumerator DodgeCoroutine(SideType dodgeSide)
    {
        _currentDodges--;
        _canDodge = false;
        character_data.IsDodging = true;
        character_data.CurrentDodgeSide = dodgeSide;

        yield return new WaitForSeconds(dodgeDuration);
        
        character_data.IsDodging = false;
        character_data.CurrentDodgeSide = SideType.none;

        yield return new WaitForSeconds(cooldownDuration);

        if (_currentDodges > 0)
            _canDodge = true;
    }
}
