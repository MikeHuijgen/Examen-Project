using System;
using UnityEngine;
using UnityEngine.Events;
public class GameOver : MonoBehaviour
{
    public UnityEvent OnGameOverActivated = new UnityEvent();
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
        OnGameOverActivated?.Invoke();
    }

    public void EnemyDead()
    {
        gameOverChanel.RaceEvent();
        _victoryScreen?.SetActive(true);
        OnGameOverActivated?.Invoke();
    }

    public void OnDead() => putOff?.Invoke();
}
