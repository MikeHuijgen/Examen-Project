using UnityEngine;

public class MatchDetector
{
    // private int GetValidBlockType(List<Match3Block> gridObjectVisualPrefabs ,int x, int y)
    // {
    //     List<FakeAttack> possibleTypes = new List<FakeAttack>();

    //     // stel: je hebt 5 soorten blocks (0 t/m 4)
    //     for (int i = 0; i < gridObjectVisualPrefabs.Count; i++)
    //     {
    //         possibleTypes.Add(gridObjectVisualPrefabs[i].GetFakeAttack);
    //     }

    //     // 🔍 Check links (horizontaal)
    //     if (x >= 2)
    //     {
    //         var left = _gridObjectArray[x - 1, y].GetGridMatch3Block.GetFakeAttack;
    //         var left2 = _gridObjectArray[x - 2, y].GetGridMatch3Block.GetFakeAttack;

    //         if (left == left2)
    //         {
    //             possibleTypes.Remove(left);
    //         }
    //     }

    //     // 🔍 Check onder (verticaal)
    //     if (y >= 2)
    //     {
    //         var down = _gridObjectArray[x, y - 1].GetGridMatch3Block.GetFakeAttack;
    //         var down2 = _gridObjectArray[x, y - 2].GetGridMatch3Block.GetFakeAttack;

    //         if (down == down2)
    //         {
    //             possibleTypes.Remove(down);
    //         }
    //     }

    //     return possibleTypes.Count;
    // }
}
