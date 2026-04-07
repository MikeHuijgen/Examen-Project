using System.Collections.Generic;
using UnityEngine;

public class MatchDetector
{
    public FakeAttack GetRandomValidAttackData(List<FakeAttack> attackDataList , GridObject[,] gridArray, int x, int y)
    {
        List<FakeAttack> possibleAttackData = new List<FakeAttack>();

        for (int i = 0; i < attackDataList.Count; i++)
        {
            possibleAttackData.Add(attackDataList[i]);
        }

        if (x >= 2)
        {
            var left = gridArray[x - 1, y].GetAttackData;
            var left2 = gridArray[x - 2, y].GetAttackData;

            if (left == left2)
            {
                possibleAttackData.Remove(left);
            }
        }

        if (y >= 2)
        {
            var down = gridArray[x, y - 1].GetAttackData;
            var down2 = gridArray[x, y - 2].GetAttackData;

            if (down == down2)
            {
                possibleAttackData.Remove(down);
            }
        }

        return possibleAttackData[Random.Range(0, possibleAttackData.Count)];
    }

    public HashSet<gridObject> CheckForAllMatches(GridObject[,] grid, int gridWidth, int gridHeight)
    {
        var matches = new HashSet<gridObject>();

        for (int y = 0; y < gridHeight; y++)
        {
            int matchLength = 1;
            for (int x = 0; x < gridWidth; x++)
            {
                if (x == gridWidth - 1 || grid[x, y].GetAttackData != grid[x + 1, y].GetAttackData)
                {
                    if (matchLength >= 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            matches.Add(new gridObject(x - k, y));
                        }
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
                if (y == gridHeight - 1 || grid[x, y].GetAttackData != grid[x, y + 1].GetAttackData)
                {
                    if (matchLength >= 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            matches.Add(new gridObject(x, y - k));
                        }
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
}
