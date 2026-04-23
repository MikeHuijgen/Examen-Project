using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
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

    public bool TryEnableVisualByProfile(Match3BlockProfile match3BlockProfile, GridObject gridObject, Func<GridPosition, Vector3> GetWorldPos, float yOffset = 0)
    {
        if (!_pool.TryGetValue(match3BlockProfile, out var listOfVisuals)) return false;

        foreach (var visual in listOfVisuals)
        {
            if (visual.activeInHierarchy) continue;

            _ActiveBlockVisuals.Add(gridObject, visual);
            var visualPosition = GetWorldPos(gridObject.GetGridPosition);
            visualPosition.y = visualPosition.y + yOffset;
            visual.transform.position = visualPosition;
            visual.SetActive(true);
            break;
        }

        return true;
    }

    public void TryDisableVisualOnGridObject(GridObject gridObject)
    {
        if (!_ActiveBlockVisuals.TryGetValue(gridObject, out var visual)) return;
        visual.SetActive(false);
        visual.transform.position = Vector3.zero;
        _ActiveBlockVisuals.Remove(gridObject);
    }

    public IEnumerator MoveVisualWithTweenRoutine(GridObject gridObject, Vector3 newPosition, float tweenSpeed, Ease ease, float tweenStrength = 1)
    {
        if(!_ActiveBlockVisuals.TryGetValue(gridObject, out var targetVisual)) yield return null;

        yield return targetVisual.transform.DOMove(newPosition, tweenSpeed).SetEase(ease).WaitForCompletion();
    }

    public Tween CreateVisualMoveTween(GridObject gridObject, Vector3 newPosition, float tweenSpeed, Ease ease, float tweenStrength = 1)
    {
        if(!_ActiveBlockVisuals.TryGetValue(gridObject, out var targetVisual)) return null;

        return targetVisual.transform.DOMove(newPosition, tweenSpeed).SetEase(ease, tweenStrength);        
    }

    public IEnumerator DisableMatchesVisuals(HashSet<Match> matches)
    {
        foreach (var match in matches)
        {
            for (int i = 0; i < match.MatchedObjectGroup.Length; i++)
            { 
                TryDisableVisualOnGridObject(match.MatchedObjectGroup[i]);
            }
        }

        yield return new WaitForSeconds(.15f);
    }

    public void MoveVisualBinding(GridObject from, GridObject to)
    {
        if (!_ActiveBlockVisuals.TryGetValue(from, out var visual)) return;

        _ActiveBlockVisuals.Remove(from);
        _ActiveBlockVisuals[to] = visual;
    }
}

[Serializable]
public struct ProfileToVisual
{
    public Match3BlockProfile match3BlockProfile;
    public GameObject visual;
}
