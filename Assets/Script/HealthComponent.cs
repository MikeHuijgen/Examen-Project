using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();   
    [SerializeField] private int maxHP;

    [SerializeField] private HealthBar healthBar;

    private int _currentHP;

    private float _currentTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _currentTime = 2f;
        _currentHP = maxHP;
    }

    private void Start()
    {
        healthBar.SetSliderMaxValue(maxHP);
        healthBar.UpdateHealthUI(_currentHP);
    }

    public void TakeDamage(int damage)
    {
        if (_currentHP == 0) return;

        _currentHP -= damage;
        healthBar.UpdateHealthUI(_currentHP);

        if (_currentHP > 0) return;
        _currentHP = 0;
        OnDeath?.Invoke();
    }

    private void Update()
    {
        float time = Time.deltaTime;
        _currentTime -= Time.deltaTime;
        if (_currentTime < 0)
        {
            TakeDamage(1);
            _currentTime = 2f;
        }
    }
    public void PrintText()
    {
        Debug.Log("i AM DEATH" + gameObject.name);
    }
}
