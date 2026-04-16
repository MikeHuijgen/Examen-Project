using UnityEngine;

public class AttackListener : MonoBehaviour
{
    [SerializeField] private AttackSystem attackSystem;
    private void OnEnable()
    {
        LevelGrid.OnMatchDestroyed += HandleMatchDestroyed;
    }

    private void OnDisable()
    {
        LevelGrid.OnMatchDestroyed -= HandleMatchDestroyed;
    }

    private void HandleMatchDestroyed(BaseAttack attack)
    {
        attackSystem.TriggerAttack(attack);
    }
}
