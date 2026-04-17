using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    private GameStates currentGameStates;
    private AudioSource musicScource;
    private Animator animator;

    public UnityEvent won = new UnityEvent();
    public UnityEvent lose = new UnityEvent();
    [SerializeField] GameObject _playerInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;
        _playerInput.SetActive(false);
        musicScource.ignoreListenerPause = true;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
    public void ChancheGameStates(GameStates gameState)
    {
      currentGameStates = gameState;
        switch (currentGameStates)
        {
            case GameStates.start:
                Time.timeScale = 1f;
                break;

            case GameStates.pause:
                FreezeGame();             
                break;

            case GameStates.victory:
                FreezeGame();
                won?.Invoke();
                break;

            case GameStates.death:
                FreezeGame();
                lose?.Invoke();
                break;
        }
    }
}
