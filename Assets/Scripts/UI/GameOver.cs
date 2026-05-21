using System;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    [SerializeField] OnGameOverChannel gameOverChannel;
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
        AudioManager.Instance.PlaySound("Die");
        gameOverChannel.RaiseEvent();
        _gameOverScreen.SetActive(true);
    }

    public void EnemyDead()
    {
        AudioManager.Instance.PlaySound("Win");
        gameOverChannel.RaiseEvent();
        _victoryScreen?.SetActive(true);
    }

    public void OnDead() => putOff?.Invoke();
}
