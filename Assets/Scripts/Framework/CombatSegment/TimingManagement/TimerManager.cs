using UnityEngine;

public class TimerManager
{
    public bool RunTimer(ref CountdownTimer timer, float duration)
    {
        if (timer == null || !timer.IsTimerActive)
        {
            timer = new CountdownTimer(duration);
            timer.StartTimer();
            return false;
        }

        timer.Tick(Time.deltaTime);

        if (!timer.IsTimerDone) return false;

        timer.StopTimer();
        return true;
    }
}
