using System;
using UnityEngine;
public class GameOver : MonoBehaviour
{
    [SerializeField] OnGameOverChannel gameOverChannel;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject victoryScreen;

    public Action PutOff;

    private void Start()
    {
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
    }

    public void PlayerDead()
    {
        AudioManager.Instance.PlaySound("Die");
        gameOverChannel.RaiseEvent();
        gameOverScreen.SetActive(true);
    }

    public void EnemyDead()
    {
        AudioManager.Instance.PlaySound("Win");
        gameOverChannel.RaiseEvent();
        victoryScreen?.SetActive(true);
    }

    public void OnDead() => PutOff?.Invoke();
}
