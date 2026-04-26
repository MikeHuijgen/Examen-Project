using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public struct SwapAction
{
    public async Task Execute(SwapActionParameters parameters)
    {
        var from = parameters.from;
        var to = parameters.to;
        var GetWorldPositionCallback = parameters.GetWorldPositionCallback;

        var tweenSwapSpeed = parameters.tweenSwapSpeed;

        parameters.dataSwapCallback(from, to);
        await parameters.visualSwapCallback(from, to, GetWorldPositionCallback, tweenSwapSpeed, Ease.InOutQuad);
    }
}
