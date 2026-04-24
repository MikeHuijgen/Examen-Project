using UnityEngine;

public class TopCanvasEnabler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TutorialManager tutorialManager;
    private void OnEnable()
    {
        tutorialManager.OnTutorialFinished += () => container.SetActive(true);
        GameOver.OnGameOver += () => container.SetActive(false);
    }
    private void OnDisable()
    {
        tutorialManager.OnTutorialFinished -=  () => container.SetActive(true);
        GameOver.OnGameOver -= () => container.SetActive(false);
    }
}
