using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public GameStates CurrentGameStates;
    private AudioSource _musicScource;

    [SerializeField] private GameObject playerInput;

    private void Awake() => ActiveGame();

    private IEnumerator FreezeGame()
    {
        playerInput.SetActive(false);
        yield return new WaitForSeconds(0.5f); 
        Time.timeScale = 0f;
        if (_musicScource != null) _musicScource.ignoreListenerPause = true;
    }

    private void ActiveGame()
    {
        Time.timeScale = 1f;
        playerInput.SetActive(true);
        if (_musicScource != null) _musicScource.ignoreListenerPause = false;
    }
    public void ChancheGameStates(int enumValue)
    {
        CurrentGameStates = (GameStates)enumValue;
        switch (CurrentGameStates)
        {
            case GameStates.start:
                ActiveGame();
                break;

            case GameStates.pause:
                StartCoroutine(FreezeGame());     
                break;
        }
    }
}
