using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour
{
    [SerializeField] private RuleToAction[] ruleToActions;

    private Dictionary<RuleFlag, BaseAction> _ruleToActionDictionary;

    void Awake()
    {
        FillDictionary();
    }

    private void FillDictionary()
    {
        foreach (var ruleToAction in ruleToActions)
        {
            if (_ruleToActionDictionary.ContainsKey(ruleToAction.ruleFlag)) continue;
            _ruleToActionDictionary.Add(ruleToAction.ruleFlag, ruleToAction.action);
        }
    }

    public async Task TryProcessSwapAction(SwapAction action)
    {
        
    }
}

[Serializable]
public struct RuleToAction
{
    public RuleFlag ruleFlag;
    public BaseAction action;
}
