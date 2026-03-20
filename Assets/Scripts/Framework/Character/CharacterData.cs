using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;

    public bool IsDodging;
    public SideType CurrentDodgeSide;
}