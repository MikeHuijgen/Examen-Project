using UnityEngine;

[CreateAssetMenu(fileName = "PlaySoundSubAction", menuName = "Scriptable Objects/Match3/Actions/SubAction/PlaySound")]
public class PlaySoundSubAction : Match3BaseSubAction
{
    public string AudioString;
    public override void Execute()
    {
        AudioManager.Instance.PlaySound(AudioString);
    }
}
