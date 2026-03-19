using UnityEngine;

[RequireComponent(typeof(CharacterData))]
public abstract class CharacterComponent : MonoBehaviour
{
    protected CharacterData character_data { get; private set; }

    protected virtual void Awake()
    {
        character_data = GetComponent<CharacterData>();
    }
}