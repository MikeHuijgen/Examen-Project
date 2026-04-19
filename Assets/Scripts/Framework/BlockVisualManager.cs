using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockVisualManager : MonoBehaviour
{
    [SerializeField] private ProfileToVisual[] profileToVisuals;
    [SerializeField] private int initialPoolSizePerMatch3Block = 15;
    private Dictionary<Match3BlockProfile, GameObject> _profileToVisualsDictionary;
    private Dictionary<Match3BlockProfile, List<GameObject>> _pool;
    private Dictionary<GridPosition, GameObject> _ActiveBlockVisuals;

    void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _ActiveBlockVisuals = new Dictionary<GridPosition, GameObject>();
        _profileToVisualsDictionary = new Dictionary<Match3BlockProfile, GameObject>();
        _pool = new Dictionary<Match3BlockProfile, List<GameObject>>();

        foreach (var profileToVisual in profileToVisuals)
        {
            _profileToVisualsDictionary.Add(profileToVisual.match3BlockProfile, profileToVisual.visual);
            _pool.Add(profileToVisual.match3BlockProfile, new List<GameObject>());

            for (int i = 0; i < initialPoolSizePerMatch3Block; i++)
            {
                var newBlock = Instantiate(profileToVisual.visual, transform);
                newBlock.SetActive(false);
                _pool[profileToVisual.match3BlockProfile].Add(newBlock);
            }

        }
    }

    public bool TryEnableBlockByProfile(Match3BlockProfile match3BlockProfile, GridPosition gridPosition, Func<GridPosition, Vector3> GetWorldPos)
    {
        if (!_pool.TryGetValue(match3BlockProfile, out var listOfVisuals)) return false;

        foreach (var visual in listOfVisuals)
        {
            if (visual.activeInHierarchy) continue;

            visual.SetActive(true);
            _ActiveBlockVisuals.Add(gridPosition, visual);
            visual.transform.position = GetWorldPos(gridPosition);
            break;
        }

        return true;
    }
}

[Serializable]
public struct ProfileToVisual
{
    public Match3BlockProfile match3BlockProfile;
    public GameObject visual;
}
