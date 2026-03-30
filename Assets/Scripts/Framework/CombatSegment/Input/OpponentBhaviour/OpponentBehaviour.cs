using UnityEngine;

public class OpponentBehaviour : MonoBehaviour
{
    [SerializeField] private float minAttackDelayTime;
    [SerializeField] private float maxAttackDelayTime;

    private CountdownTimer _attackDelay;

    private void Start()
    {
        _attackDelay = new CountdownTimer(Random.Range(minAttackDelayTime, maxAttackDelayTime));
        _attackDelay.StartTimer();
    }

    void Update()
    {
        _attackDelay.Tick(Time.deltaTime);
        if (!_attackDelay.IsTimerDone) return;
        _attackDelay.StopTimer();
        Debug.Log("Attack!");
        _attackDelay.StartTimer();
    }
}
