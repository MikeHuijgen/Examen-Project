using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();
    public UnityEvent OnTakeDamage = new UnityEvent();
    [SerializeField] private float maxHealth;

    [SerializeField] private HealthBar healthBar;

    private float _currentHealth;

    private void Awake() =>_currentHealth = maxHealth;
    

    private void Start()
    {
        healthBar.SetSliderMaxValue(maxHealth);
        healthBar.UpdateHealthUI(_currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth == 0) return;
        _currentHealth -= damage;
        OnTakeDamage?.Invoke();
        healthBar.UpdateHealthUI(_currentHealth);

        if (_currentHealth > 0) return;
        AudioManager.Instance.PlaySound("Die");
        _currentHealth = 0;
        OnDeath?.Invoke();
    }
}
