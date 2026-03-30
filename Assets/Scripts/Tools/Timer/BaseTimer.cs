public abstract class BaseTimer 
{
    protected float timer_duration;
    protected float current_time;
    protected bool is_timer_done;
    protected bool is_timer_paused;

    protected bool is_timer_active;

    public void StartTimer()
    {
        is_timer_active = true;
        ResetTimer();
    }

    public void StopTimer()
    {
        is_timer_active = false;
        ResetTimer();
    }

    public virtual void ResetTimer() => is_timer_done = false;
    public void UnPauseTimer() => is_timer_paused = false;
    public void PauseTimer() => is_timer_paused = true;
    public abstract void Tick(float deltaTime);
    public bool IsTimerDone => is_timer_done;
    public float GetCurrentTime => current_time;
    public bool IsTimerActive => is_timer_active;
}
