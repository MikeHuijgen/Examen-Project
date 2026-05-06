using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauzeMenu;

    public UnityEvent OnPauseStart = new UnityEvent();
    public UnityEvent OnPauseFinished = new UnityEvent();

    bool isPaused;

    private void Start()
    {
        pauzeMenu.SetActive(false);
    }

    public void IsPaused()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }

    }

    private void PauseGame()
    {
        pauzeMenu.SetActive(true);
        OnPauseStart?.Invoke();
        isPaused = true;
    }

    private void ResumeGame()
    {
        pauzeMenu.SetActive(false);
        OnPauseFinished?.Invoke();
        isPaused = false;
    }
}