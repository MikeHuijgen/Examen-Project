using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public class Match3BlockPool : MonoBehaviour
{
    [SerializeField] private int initialPoolSizePerMatch3Block = 15;
    [SerializeField] private Match3Block[] match3BlockPrefabs;

    private Dictionary<FakeAttack, Queue<Match3Block>> _pool;
    private Dictionary<FakeAttack, Match3Block> _prefabMap;

    public void InitializePool(Transform parent)
    {
        _pool = new Dictionary<FakeAttack, Queue<Match3Block>>();
        _prefabMap = new Dictionary<FakeAttack, Match3Block>();

        foreach (var prefab in match3BlockPrefabs)
        {
            var attack = prefab.GetFakeAttack;

            if (_prefabMap.ContainsKey(attack)) continue;

            _prefabMap.Add(attack, prefab);
            _pool.Add(attack, new Queue<Match3Block>());

            for (int i = 0; i < initialPoolSizePerMatch3Block; i++)
            {
                var block = CreateNewBlock(prefab, parent);
                _pool[attack].Enqueue(block);
            }
        }
    }

    private Match3Block CreateNewBlock(Match3Block prefab, Transform parent)
    {
        var block = Instantiate(prefab, parent);
        block.gameObject.SetActive(false);
        return block;
    }

    public Match3Block GetMatch3BlockByAttackData(FakeAttack attack, Transform parent = null)
    {
        if (!_pool.ContainsKey(attack)) return null;

        var queue = _pool[attack];

        Match3Block block;

        if (queue.Count == 0)
        {
            block = CreateNewBlock(_prefabMap[attack], parent ?? transform);
        }
        else
        {
            block = queue.Dequeue();
        }

        block.gameObject.SetActive(true);
        return block;
    }

    public void ReturnMatch3Block(Match3Block block)
    {
        var attack = block.GetFakeAttack;

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
