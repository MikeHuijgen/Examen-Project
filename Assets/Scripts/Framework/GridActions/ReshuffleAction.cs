using System;
using UnityEngine;

public class ReshuffleAction : BaseAction<ReshuffleActionParameters>
{
    private GridObject[,] _grid;
    private float _spawnOffset = 5f;

    public ReshuffleAction(ReshuffleActionParameters parameters) : base(parameters){}

    public override void Execute(Action<BaseAction> OnActionComplete)
    {
        on_action_complete = OnActionComplete;
        action_context = parameters.actionContext;
        _grid = action_context.GridSystem.GetGridObjectArray;
        Debug.Log("Reshuffle");
        ReshuffleGrid();
    }

    private void ReshuffleGrid()
    {
        foreach (var gridObject in _grid)
        {
            action_context.BlockVisualManager.TryDisableVisualOnGridObject(gridObject);
            gridObject.SetMatch3BlockProfile(null);
        }

        for (var x = 0; x < action_context.LevelGridData.GridWidth; x++)
        {
            for (int y = 0; y < action_context.LevelGridData.GridHeight; y++)
            {
                action_context.GridActionProcessor.ProcessAction(new CreateTileAction(new CreateTileActionParameters
                {
                    actionContext = action_context,
                    spawnYOffset = _spawnOffset,
                    targetGridObject = _grid[x,y],
                    targetGridPosition = _grid[x,y].GetGridPosition
                }));
            }
        }

        CheckForPossibleMoves();
    }

    private void CheckForPossibleMoves()
    {
        if (action_context.MatchDetector.PlayerHasPossibleMoves(_grid, action_context.GridSystem.SwapGridObjectsData)) 
        {
            CompleteAction();
            return;
        }

        ReshuffleGrid();
    }
}
