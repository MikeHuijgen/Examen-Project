using System;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    public static event Action OnDeadStarted;

    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _victoryScreen;
    [SerializeField] GameObject _playerInput;

    void Start()
    {
        _gameOverScreen.SetActive(false);
        _victoryScreen.SetActive(false);
    }

    public void PlayerDead()
    {
        _gameOverScreen.SetActive(true);
        _playerInput.SetActive(true);
        OnDeadStarted?.Invoke();
    }

    public void EnemyDead()
    {
        _victoryScreen?.SetActive(true);
        _playerInput.SetActive(true);
        OnDeadStarted?.Invoke();
    }
}
