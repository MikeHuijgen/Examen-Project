using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockVisualManager : MonoBehaviour
{
    [SerializeField] private ProfileToVisual[] profileToVisuals;
    [SerializeField] private int initialPoolSizePerMatch3Block = 15;
    private Dictionary<Match3BlockProfile, GameObject> _profileToVisualsDictionary;
    private Dictionary<Match3BlockProfile, List<GameObject>> _pool;
    private Dictionary<GridObject, GameObject> _ActiveBlockVisuals;

    void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _ActiveBlockVisuals = new Dictionary<GridObject, GameObject>();
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

    public bool TryEnableVisualByProfile(Match3BlockProfile match3BlockProfile, GridObject gridObject, Func<GridPosition, Vector3> GetWorldPos)
    {
        if (!_pool.TryGetValue(match3BlockProfile, out var listOfVisuals)) return false;

        foreach (var visual in listOfVisuals)
        {
            if (visual.activeInHierarchy) continue;

            _ActiveBlockVisuals.Add(gridObject, visual);
            visual.transform.position = GetWorldPos(gridObject.GetGridPosition);
            visual.SetActive(true);
            break;
        }

        return true;
    }

    public bool TryDisableVisualOnGridPosition(GridObject gridObject)
    {
        if (!_ActiveBlockVisuals.TryGetValue(gridObject, out var visual)) return false;
        visual.SetActive(false);
        visual.transform.position = Vector3.zero;
        _ActiveBlockVisuals.Remove(gridObject);
        return true;
    }
}

[Serializable]
public struct ProfileToVisual
{
    public Match3BlockProfile match3BlockProfile;
    public GameObject visual;
}
