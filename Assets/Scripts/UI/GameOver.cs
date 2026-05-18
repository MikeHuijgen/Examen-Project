using System;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    [SerializeField] OnGameOverChanel gameOverChanel;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _victoryScreen;

    public Action putOff;

    void Start()
    {
        _gameOverScreen.SetActive(false);
        _victoryScreen.SetActive(false);
    }

    public void PlayerDead()
    {
        gameOverChanel.RaceEvent();
        _gameOverScreen.SetActive(true);
    }

    public void EnemyDead()
    {
        gameOverChanel.RaceEvent();
        _victoryScreen?.SetActive(true);
    }

    public void OnDead() => putOff?.Invoke();
}
