using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class GridActionProcessor : MonoBehaviour
{
    public async Task TryProcessSwapAction(SwapActionParameters parameters)
    {
        if (parameters.from == null || 
        parameters.to == null || 
        parameters.dataSwapCallback == null || 
        parameters.visualSwapCallback == null || 
        parameters.GetWorldPositionCallback == null||
        parameters.tweenSwapSpeed == 0)
        {
            Debug.LogWarning("One of the parameters are null");
            return;
        }

        await new SwapAction().Execute(parameters);
    }
}
