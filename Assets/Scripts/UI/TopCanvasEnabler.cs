using UnityEngine;

public class TopCanvasEnabler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    private void OnEnable() => TutorialManager.OnTutorialFinished += () => container.SetActive(true);
    private void OnDisable() => TutorialManager.OnTutorialFinished -=  () => container.SetActive(true);
}
