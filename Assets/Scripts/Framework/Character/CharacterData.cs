using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;

    [HideInInspector] public bool IsDodging;
    [HideInInspector] public SideType CurrentDodgeSide;
}