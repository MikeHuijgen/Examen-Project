using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class GridActionProcessor : MonoBehaviour 
{
    public async Task TryProcessAction<Tparameters>(BaseAction<Tparameters> action)
    {
        await action.Execute();
    }
}
