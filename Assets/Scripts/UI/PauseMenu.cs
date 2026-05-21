using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public UnityEvent OnPauseStart = new UnityEvent();
    public UnityEvent OnPauseFinished = new UnityEvent();

    bool isPaused;

    private void Start()
    {
        pauseMenu.SetActive(false);
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
        pauseMenu.SetActive(true);
        OnPauseStart?.Invoke();
        isPaused = true;
    }

    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
        OnPauseFinished?.Invoke();
        isPaused = false;
    }
}