using System.Collections.Generic;
using UnityEngine;


public class MatchDetector
{
    public FakeAttack GetRandomValidAttackData(List<FakeAttack> attackDataList , GridObject[,] gridArray, int x, int y)
    {
        List<FakeAttack> possibleAttackData = new List<FakeAttack>();

        // stel: je hebt 5 soorten blocks (0 t/m 4)
        for (int i = 0; i < attackDataList.Count; i++)
        {
            possibleAttackData.Add(attackDataList[i]);
        }

        // 🔍 Check links (horizontaal)
        if (x >= 2)
        {
            var left = gridArray[x - 1, y].GetAttackData;
            var left2 = gridArray[x - 2, y].GetAttackData;

            if (left == left2)
            {
                possibleAttackData.Remove(left);
            }
        }

        // 🔍 Check onder (verticaal)
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
}
