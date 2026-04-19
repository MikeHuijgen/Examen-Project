using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfileContainer", menuName = "Scriptable Objects/Match3/Blocks/Container")]
public class Match3BlockProfileContainer : ScriptableObject
{
    public Match3BlockProfile[] match3BlockProfiles;

    public Match3BlockProfile GetRandomProfile() => match3BlockProfiles[Random.Range(0, match3BlockProfiles.Length)];
}
