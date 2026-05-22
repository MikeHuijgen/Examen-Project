using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;

public class BlockVisualManager : MonoBehaviour
{
    [SerializeField] private ProfileToVisual[] profileToVisuals;
    [SerializeField] private int initialPoolSizePerMatch3Block = 15;
    private Dictionary<Match3BlockProfile, GameObject> _profileToVisualsDictionary;
    private Dictionary<Match3BlockProfile, List<GameObject>> _pool;
    private Dictionary<GridObject, GameObject> _activeBlockVisuals;

    private void Awake() => InitializePool();

    private void InitializePool()
    {
        _activeBlockVisuals = new Dictionary<GridObject, GameObject>();
        _profileToVisualsDictionary = new Dictionary<Match3BlockProfile, GameObject>();
        _pool = new Dictionary<Match3BlockProfile, List<GameObject>>();

        foreach (var profileToVisual in profileToVisuals)
        {
            _profileToVisualsDictionary.Add(profileToVisual.Match3BlockProfile, profileToVisual.Visual);
            _pool.Add(profileToVisual.Match3BlockProfile, new List<GameObject>());

            for (int i = 0; i < initialPoolSizePerMatch3Block; i++)
            {
                var newBlock = Instantiate(profileToVisual.Visual, transform);
                newBlock.SetActive(false);
                _pool[profileToVisual.Match3BlockProfile].Add(newBlock);
            }

        }
    }

    public bool TryEnableVisualByProfile(Match3BlockProfile match3BlockProfile, GridObject gridObject, Func<GridPosition, Vector3> GetWorldPos, float yOffset = 0)
    {
        if (!_pool.TryGetValue(match3BlockProfile, out var listOfVisuals)) return false;

        foreach (var visual in listOfVisuals)
        {
            if (visual.activeInHierarchy) continue;

            _activeBlockVisuals.Add(gridObject, visual);
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
        if (!_activeBlockVisuals.TryGetValue(gridObject, out var visual)) return;
        visual.SetActive(false);
        visual.transform.position = Vector3.zero;
        _activeBlockVisuals.Remove(gridObject);
    }

    public Tween[] SwapVisualTweens(GridObject gridObjectA, GridObject gridObjectB, Func<GridPosition, Vector3> GetWorldPosition , float tweenSpeed, Ease ease)
    {
        if(!_activeBlockVisuals.TryGetValue(gridObjectA, out var targetVisualA) || !_activeBlockVisuals.TryGetValue(gridObjectB, out var targetVisualB)) 
        {
            Debug.LogWarning("One or both of the grid objects are not a active block visual");
            return null;
        }

        var newPositionA = GetWorldPosition(gridObjectA.GetGridPosition);
        var newPositionB = GetWorldPosition(gridObjectB.GetGridPosition);

        var tweens = new Tween[]{targetVisualA.transform.DOMove(newPositionA, tweenSpeed).SetEase(ease), targetVisualB.transform.DOMove(newPositionB, tweenSpeed).SetEase(ease)};

        return tweens;
    }

    public Tween CreateVisualMoveTween(GridObject gridObject, Vector3 newPosition, float tweenSpeed, Ease ease, float tweenStrength)
    {
        if(!_activeBlockVisuals.TryGetValue(gridObject, out var targetVisual)) return null;

        return targetVisual.transform.DOMove(newPosition, tweenSpeed).SetEase(ease, tweenStrength).Pause();        
    }

    public void DisableMatchesVisuals(HashSet<Match> matches)
    {
        foreach (var match in matches)
        {
            for (int i = 0; i < match.MatchedObjectGroup.Length; i++)
            { 
                TryDisableVisualOnGridObject(match.MatchedObjectGroup[i]);
            }
        }
    }

    public void MoveVisualBinding(GridObject from, GridObject to)
    {
        if (!_activeBlockVisuals.TryGetValue(from, out var visual)) return;

        _activeBlockVisuals.Remove(from);
        _activeBlockVisuals[to] = visual;
    }
}

[Serializable]
public struct ProfileToVisual
{
    public Match3BlockProfile Match3BlockProfile;
    public GameObject Visual;
}
