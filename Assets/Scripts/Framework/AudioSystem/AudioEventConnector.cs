using UnityEngine;

public class AudioEventConnector : MonoBehaviour
{
    public void PlaySound(string soundIdentifier)
    {
        AudioManager.Instance.PlaySound(soundIdentifier);
    }
}
