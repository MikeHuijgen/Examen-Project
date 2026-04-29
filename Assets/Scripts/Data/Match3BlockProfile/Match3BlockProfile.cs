using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Match3BlockProfile", menuName = "Scriptable Objects/Match3/Profile")]
public class Match3BlockProfile : ScriptableObject
{
    public BaseMatchEffect matchEffect;
    public RuleFlag[] ruleFlags;

    private Dictionary<string, RuleFlag> _actionTypes;

    public void Init()
    {
        _actionTypes = new Dictionary<string, RuleFlag>();
        foreach (var rule in ruleFlags)
        {
            if (_actionTypes.ContainsKey(rule.RuleId)) continue;
            _actionTypes.Add(rule.RuleId, rule);
        }        
    }

    public bool HasRule(string targetId) => _actionTypes.ContainsKey(targetId);
}
