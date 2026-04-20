using System.Collections.Generic;
using UnityEngine;

public class MatchDetector
{
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

        return possibleProfiles[Random.Range(0, possibleProfiles.Count)];
    }

    public HashSet<Match> CheckForAllMatches(GridObject[,] grid, int gridWidth, int gridHeight)
    {
        var matches = new HashSet<Match>();

        for (int y = 0; y < gridHeight; y++)
        {
            int matchLength = 1;
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y].GetMatch3BlockProfile == null || grid[x, y].GetMatch3BlockProfile.HasAction<MatchAction>(out var matchAction)) continue;
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
                if (grid[x, y].GetMatch3BlockProfile == null) continue;
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

        foreach (var match in matches)
        {
            for (int i = 0; i < match.MatchedObjectGroup.Length; i++)
            {
                //Debug.Log(match.MatchedObjectGroup[i].GetMatch3BlockProfile);
            }
        }

        return matches;
    }

    public bool HasMatchAt(GridObject[,] grid, int x, int y)
    {
        var block = grid[x, y].GetGridMatch3Block;
        if (block == null) return false;

        var blockType = block.GetAttackData;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int count = 1;

        for (var i = x - 1; i >= 0; i--)
        {
            var leftBlock = grid[i, y].GetGridMatch3Block;
            if (leftBlock != null && leftBlock.GetAttackData == blockType) count++;
            else break;
        }

        for (var i = x + 1; i < width; i++)
        {
            var rightBlock = grid[i, y].GetGridMatch3Block;
            if (rightBlock != null && rightBlock.GetAttackData == blockType) count++;
            else break;
        }

        if (count >= 3) return true;

        count = 1;

        for (var i = y - 1; i >= 0; i--)
        {
            var downBlock = grid[x, i].GetGridMatch3Block;
            if (downBlock != null && downBlock.GetAttackData == blockType) count++;
            else break;
        }

        for (int i = y + 1; i < height; i++)
        {
            var upBlock = grid[x, i].GetGridMatch3Block;
            if (upBlock != null && upBlock.GetAttackData == blockType) count++;
            else break;
        }

        if (count >= 3) return true;

        return false;
    }
}
