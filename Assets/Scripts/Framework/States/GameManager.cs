using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public GameStates currentGameStates;
    private AudioSource musicScource;
    private Animator animator;

    [SerializeField] GameObject _playerInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator FreezeGame()
    {
        _playerInput.SetActive(false);
        yield return new WaitForSeconds(0.5f); 
        Time.timeScale = 0f;
        musicScource.ignoreListenerPause = true;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void ActiveGame()
    {
        Time.timeScale = 1f;
        _playerInput.SetActive(true);
        musicScource.ignoreListenerPause =false;
    }
    public void ChancheGameStates(int enumValue)
    {
      currentGameStates = (GameStates)enumValue;
        switch (currentGameStates)
        {
            case GameStates.start:
                ActiveGame();
                break;

            case GameStates.pause:
                StartCoroutine(FreezeGame());     
                break;

            case GameStates.death:
                StartCoroutine(FreezeGame()); 
                break;
        }
    }
}
