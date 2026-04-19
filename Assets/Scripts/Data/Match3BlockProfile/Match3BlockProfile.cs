using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfile", menuName = "Scriptable Objects/Match3/Blocks/Profile")]
public class Match3BlockProfile : ScriptableObject
{
    public Match3BaseAction[] Actions;

    private HashSet<Type> _actionTypes;

    public void Init()
    {
        _actionTypes = new HashSet<Type>();
        foreach (var action in Actions)
        {
            _actionTypes.Add(action.GetType());
        }        
    }

    public bool HasAction<T>() => _actionTypes.Contains(typeof(T));

}
