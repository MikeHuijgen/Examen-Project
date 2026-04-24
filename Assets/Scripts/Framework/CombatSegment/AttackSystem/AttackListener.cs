using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private LevelGrid levelGrid;
    private void OnEnable()
    {
        levelGrid.OnMatchDestroyed += HandleMatchDestroyed;
    }

    private void OnDisable()
    {
        levelGrid.OnMatchDestroyed -= HandleMatchDestroyed;
    }

    private void HandleMatchDestroyed(BaseAttack attack)
    {
        attackSystem.TriggerAttack(attack);
    }
}
