using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int _currentHP;
    [SerializeField] public int _maxHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _currentHP = _maxHP;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
