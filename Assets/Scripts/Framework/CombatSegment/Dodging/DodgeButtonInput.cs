using UnityEngine;

public class DodgeButtonInput : MonoBehaviour
{
    public void DodgeLeft()
    {
        if (CharacterInput.Instance == null) return;
        CharacterInput.Instance.OnDodgeInputDetected(SideType.Left);
    }

    public void DodgeRight()
    {
        if (CharacterInput.Instance == null) return;
        CharacterInput.Instance.OnDodgeInputDetected(SideType.Right);
    }

    public void DodgeDown()
    {
        if (CharacterInput.Instance == null) return;
        CharacterInput.Instance.OnDodgeInputDetected(SideType.Down);
    }
}
