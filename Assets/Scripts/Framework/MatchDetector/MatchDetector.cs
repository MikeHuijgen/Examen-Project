using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchDetector
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.right,
        Vector2Int.up
    };
    public Match3BlockProfile GetRandomValidMatch3Profile(Match3BlockProfile[] profiles, GridObject[,] gridArray, int x, int y)
    {
        var possibleProfiles = new List<Match3BlockProfile>();

        for (int i = 0; i < profiles.Length; i++)
        {
            possibleProfiles.Add(profiles[i]);
        }

        if (x >= 2)
        {
            var left = gridArray[x - 1, y].GetMatch3BlockProfile;
            var left2 = gridArray[x - 2, y].GetMatch3BlockProfile;

            if (left == left2)
            {
                possibleProfiles.Remove(left);
            }
        }

        if (y >= 2)
        {
            var down = gridArray[x, y - 1].GetMatch3BlockProfile;
            var down2 = gridArray[x, y - 2].GetMatch3BlockProfile;

            if (down == down2)
            {
                possibleProfiles.Remove(down);
            }
        }

        return possibleProfiles[UnityEngine.Random.Range(0, possibleProfiles.Count)];
    }

    public HashSet<Match> CheckForAllMatches(GridObject[,] grid, int gridWidth, int gridHeight)
    {
        var matches = new HashSet<Match>();

        for (int y = 0; y < gridHeight; y++)
        {
            var matchLength = 1;
            for (var x = 0; x < gridWidth; x++)
            {
                if (grid[x, y].GetMatch3BlockProfile == null || !grid[x, y].GetMatch3BlockProfile.HasAction<MatchActionSO>()) continue;
                if (x == gridWidth - 1 || grid[x, y].GetMatch3BlockProfile != grid[x + 1, y].GetMatch3BlockProfile)
                {
                    if (matchLength >= 3)
                    {
                        var matchGridObjectGroup = new GridObject[matchLength];
                        for (int k = 0; k < matchLength; k++)
                        {
                            matchGridObjectGroup[k] = grid[x - k, y];
                        }
                        matches.Add(new Match(matchGridObjectGroup));
                    }
                    matchLength = 1;
                }
                else
                {
                    matchLength++;
                }
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            int matchLength = 1;
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].GetMatch3BlockProfile == null || !grid[x, y].GetMatch3BlockProfile.HasAction<MatchActionSO>()) continue;
                if (y == gridHeight - 1 || grid[x, y].GetMatch3BlockProfile != grid[x, y + 1].GetMatch3BlockProfile)
                {
                    if (matchLength >= 3)
                    {
                        var matchGridObjectGroup = new GridObject[matchLength];
                        for (int k = 0; k < matchLength; k++)
                        {
                            matchGridObjectGroup[k] = grid[x, y - k];
                        }
                        matches.Add(new Match(matchGridObjectGroup));
                    }
                    matchLength = 1;
                }
                else
                {
                    matchLength++;
                }
            }
        }

        return matches;
    }

    public bool HasMatchAt(GridObject[,] grid, int x, int y)
    {
        var match3BlockProfile = grid[x, y].GetMatch3BlockProfile;
        if (match3BlockProfile == null) return false;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int count = 1;

        for (var i = x - 1; i >= 0; i--)
        {
            var leftBlock = grid[i, y].GetMatch3BlockProfile;
            if (leftBlock != null && leftBlock == match3BlockProfile) count++;
            else break;
        }

        for (var i = x + 1; i < width; i++)
        {
            var rightBlock = grid[i, y].GetMatch3BlockProfile;
            if (rightBlock != null && rightBlock == match3BlockProfile) count++;
            else break;
        }

        if (count >= 3) return true;

        count = 1;

        for (var i = y - 1; i >= 0; i--)
        {
            var downBlock = grid[x, i].GetMatch3BlockProfile;
            if (downBlock != null && downBlock == match3BlockProfile) count++;
            else break;
        }

        for (int i = y + 1; i < height; i++)
        {
            var upBlock = grid[x, i].GetMatch3BlockProfile;
            if (upBlock != null && upBlock == match3BlockProfile) count++;
            else break;
        }

        if (count >= 3) return true;

        return false;
    }

    public bool PlayerHasPossibleMoves(GridObject[,] grid, Action<GridObject, GridObject> swapGridData)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                foreach (var dir in Directions)
                {
                    var nx = x + dir.x;
                    var ny = y + dir.y;

                    if (nx >= width || ny >= height) continue;

                    swapGridData(grid[x, y], grid[nx, ny]);

                    if (HasMatchAt(grid, x, y) || HasMatchAt(grid, nx, ny))
                    {
                        swapGridData(grid[x, y], grid[nx, ny]);
                        return true;
                    }

                    swapGridData(grid[x, y], grid[nx, ny]);
                }
            }
        }
        return false;
    }
}
