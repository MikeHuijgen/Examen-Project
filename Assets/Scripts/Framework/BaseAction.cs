using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public virtual async Task Execute(IActionParameters actionParameters){}
}
