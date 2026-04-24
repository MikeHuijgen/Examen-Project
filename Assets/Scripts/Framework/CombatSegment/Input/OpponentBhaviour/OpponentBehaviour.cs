using System.Collections.Generic;
using UnityEngine;

public class OpponentBehaviour : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;

    [SerializeField] private List<OpponentAttack> opponentAttacks;
    [SerializeField] private List<GameObject> attackDirectionWarnings;

    [SerializeField] private float minAttackDelayTime;
    [SerializeField] private float maxAttackDelayTime;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameOver gameOver;

    private CountdownTimer _idleTimer;
    private TimerManager _timer;

    private float _currentDelay;
    private bool _finishedTutorial;
    private bool _gameOver;

    private void Start()
    {
        attackDirectionWarnings.ForEach(warningObject => warningObject.SetActive(false));
        _timer = new TimerManager();
        SetNewDelay();
    }

    private void OnEnable()
    {
        tutorialManager.OnTutorialFinished += () => _finishedTutorial = true;
        gameOver.OnGameOver += () => _gameOver = true;
    }
    private void OnDisable()
    {
        tutorialManager.OnTutorialFinished -= () => _finishedTutorial = true;
        gameOver.OnGameOver -= () => _gameOver = true;
    }

    private void Update()
    {
        if (!_finishedTutorial || _gameOver) return;

        if (!attackSystem.IsIdle)
        {
            int direction = attackSystem.CurrentAttackDirection();

            if (direction >= 0 && direction < attackDirectionWarnings.Count)
            {
                attackDirectionWarnings[direction].SetActive(true);
            }
        }
        else
        {
            attackDirectionWarnings.ForEach(warning => warning.SetActive(false));
            HandleAttackDelay();
        }
    }

    private void HandleAttackDelay()
    {
        if (!_timer.RunTimer(ref _idleTimer, _currentDelay))
            return;

        var attack = GetAttack();
        attackSystem.TriggerAttack(attack);

        SetNewDelay();
    }

    private void SetNewDelay()
    {
        _currentDelay = Random.Range(minAttackDelayTime, maxAttackDelayTime);
    }

    private OpponentAttack GetAttack()
    {
        return opponentAttacks[Random.Range(0, opponentAttacks.Count)];
    }
}