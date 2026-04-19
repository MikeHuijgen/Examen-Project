using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public class Match3BlockPool : MonoBehaviour
{
    [SerializeField] private int initialPoolSizePerMatch3Block = 15;
    [SerializeField] private Match3BlockVisual[] match3BlockPrefabs;

    private Dictionary<BaseAttack, Queue<Match3BlockVisual>> _pool;
    private Dictionary<BaseAttack, Match3BlockVisual> _prefabMap;

    public void InitializePool(Transform parent)
    {
        _pool = new Dictionary<BaseAttack, Queue<Match3BlockVisual>>();
        _prefabMap = new Dictionary<BaseAttack, Match3BlockVisual>();

        foreach (var prefab in match3BlockPrefabs)
        {
            var attack = prefab.GetAttackData;

            if (_prefabMap.ContainsKey(attack)) continue;

            _prefabMap.Add(attack, prefab);
            _pool.Add(attack, new Queue<Match3BlockVisual>());

            for (int i = 0; i < initialPoolSizePerMatch3Block; i++)
            {
                var block = CreateNewBlock(prefab, parent);
                _pool[attack].Enqueue(block);
            }
        }
    }

    private Match3BlockVisual CreateNewBlock(Match3BlockVisual prefab, Transform parent)
    {
        var block = Instantiate(prefab, parent);
        block.gameObject.SetActive(false);
        return block;
    }

    public bool GetMatch3BlockByAttackData(BaseAttack attack, out Match3BlockVisual result)
    {
        result = null;
        if (!_pool.ContainsKey(attack)) return false;

        var queue = _pool[attack];

        Match3BlockVisual block;

        if (queue.Count == 0)
        {
            block = CreateNewBlock(_prefabMap[attack], transform);
        }
        else
        {
            block = queue.Dequeue();
        }

        block.gameObject.SetActive(true);
        result = block;
        return true;
    }

    public void ReturnMatch3Block(Match3BlockVisual block)
    {
        var attack = block.GetAttackData;

        if (!_pool.ContainsKey(attack))
        {
            Debug.LogError($"Trying to return block with unknown type: {attack}");
            Destroy(block.gameObject);
            return;
        }

        block.DOKill();
        block.gameObject.SetActive(false);

        _pool[attack].Enqueue(block);
    }
}
