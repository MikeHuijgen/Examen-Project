using System;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    public static event Action OnGameOver;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _victoryScreen;

    void Start()
    {
        _gameOverScreen.SetActive(false);
        _victoryScreen.SetActive(false);
    }

    public void PlayerDead()
    {
        _gameOverScreen.SetActive(true);
        OnGameOver?.Invoke();
    }

    public void EnemyDead()
    {
        _victoryScreen?.SetActive(true);
        OnGameOver?.Invoke();
    }
}
