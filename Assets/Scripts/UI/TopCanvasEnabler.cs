using UnityEngine;

public class TopCanvasEnabler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    private void OnEnable() => TutorialManager.OnTutorialFinished += EnableCanvas;
    private void OnDisable() => TutorialManager.OnTutorialFinished -= EnableCanvas;

    private void EnableCanvas() => container.SetActive(true);
}
