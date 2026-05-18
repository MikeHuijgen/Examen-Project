using UnityEngine;

public class TopCanvasEnabler : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameOver gameOver;
    private void OnEnable()
    {
        tutorialManager.OnTutorialFinished += () => container.SetActive(true);
        gameOver.putOff += () => container.SetActive(false);
    }
    private void OnDisable()
    {
        tutorialManager.OnTutorialFinished -=  () => container.SetActive(true);
        gameOver.putOff -= () => container.SetActive(false);
    }
}
