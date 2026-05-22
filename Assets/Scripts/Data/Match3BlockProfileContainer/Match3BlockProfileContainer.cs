using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Match3BlockProfileContainer", menuName = "Scriptable Objects/Match3/Container")]
public class Match3BlockProfileContainer : ScriptableObject
{
    public Match3BlockProfile[] Match3BlockProfiles;

    public Match3BlockProfile GetRandomProfile() => Match3BlockProfiles[Random.Range(0, Match3BlockProfiles.Length)];
}
