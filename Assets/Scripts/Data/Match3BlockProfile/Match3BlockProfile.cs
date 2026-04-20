using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfile", menuName = "Scriptable Objects/Match3/Blocks/Profile")]
public class Match3BlockProfile : ScriptableObject
{
    public Match3BaseAction[] Actions;

    private Dictionary<Type, Match3BaseAction> _actionTypes;

    public void Init()
    {
        _actionTypes = new Dictionary<Type, Match3BaseAction>();
        foreach (var action in Actions)
        {
            if (_actionTypes.ContainsKey(action.GetType())) continue;
            _actionTypes.Add(action.GetType(), action);
        }        
    }

    public bool HasAction<T>(out Match3BaseAction result)
    {
        result = null;
        if (!_actionTypes.TryGetValue(typeof(T), out var action)) return false;

        result = action;
        return true;
    }

}
