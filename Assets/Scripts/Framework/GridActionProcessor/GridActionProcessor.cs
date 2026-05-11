using System.Threading.Tasks;
using UnityEngine;

public class GridActionProcessor : MonoBehaviour 
{
    public async Task TryProcessAction<Tparameters>(BaseAction<Tparameters> action)
    {
        await action.Execute();
    }
}
