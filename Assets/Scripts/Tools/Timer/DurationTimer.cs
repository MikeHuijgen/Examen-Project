using UnityEngine;

public class DurationTimer : BaseTimer
{
    public DurationTimer(){}
    public DurationTimer(float timeDuration)
    {
        timer_duration = timeDuration;
        ResetTimer();
    }

    public override void ResetTimer()
    {
        base.ResetTimer();
        current_time = 0;
    }

    public override void Tick(float deltaTime)
    {
        if (is_timer_done || is_timer_paused || !is_timer_active) return;

        current_time += deltaTime;

        if (timer_duration == 0 &&current_time > timer_duration) is_timer_done = true;
    }
}
