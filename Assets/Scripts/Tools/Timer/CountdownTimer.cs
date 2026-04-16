public class CountdownTimer : BaseTimer
{
    public CountdownTimer(float timeDuration)
    {
        timer_duration = timeDuration;
        ResetTimer();
    }

    public override void ResetTimer()
    {
        base.ResetTimer();
        current_time = timer_duration;
    }

    public override void Tick(float deltaTime)
    {
        if (is_timer_done || is_timer_paused || !is_timer_active) return;

        current_time -= deltaTime;

        if (current_time <= 0) is_timer_done = true;
    }
}
