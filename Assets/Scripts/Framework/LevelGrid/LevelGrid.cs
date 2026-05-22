using System;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private Transform tileVisualHolder;
    [SerializeField] private LevelGridData levelGridData;
    [SerializeField] private Match3BlockProfileContainer match3BlockProfileContainer;
    [SerializeField] private BlockVisualManager blockVisualManager;
    [SerializeField] private GridActionProcessor gridActionProcessor;
    private GridSystem _gridSystem;
    private MatchDetector _matchDetector;
    private GridHit _beginTouchGridPosition;
    private GridHit? _currentSelectedGridPosition;
    private bool _allowInput = true;
    private ActionContext _actionContext;

    private void Awake()
    {
        _matchDetector = new MatchDetector();
        _gridSystem = new GridSystem(
            levelGridData.GridWidth,
            levelGridData.GridHeight,
            levelGridData.GridCellWidth,
            levelGridData.GridCellHeight);
        gridActionProcessor = new GridActionProcessor();

        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        _gridSystem.GenerateGrid();
        _gridSystem.CreateGridTileVisuals(levelGridData.GridTileVisual, tileVisualHolder);

        _actionContext = new ActionContext
        {
            GridSystem = _gridSystem, 
            BlockVisualManager = blockVisualManager,
            MatchDetector = _matchDetector,
            LevelGridData = levelGridData,
            GridActionProcessor = gridActionProcessor,
            Match3BlockProfileContainer = match3BlockProfileContainer
        };
        
        gridActionProcessor.ProcessAction(new ReshuffleAction(new ReshuffleActionParameters{Context = _actionContext}));

        CharacterInput.Instance.OnNewFingerDownInput += OnNewFingerDownInput;
        CharacterInput.Instance.OnNewFingerUpInput += OnNewFingerUpInput;
    }

    private void OnEnable() => gridActionProcessor.OnParentActionComplete += () => _allowInput = true;

    private void OnDisable()
    {
        CharacterInput.Instance.OnNewFingerDownInput -= OnNewFingerDownInput;
        CharacterInput.Instance.OnNewFingerUpInput -= OnNewFingerUpInput;
        gridActionProcessor.OnParentActionComplete -= () => _allowInput = true;
    }

    private void OnNewFingerDownInput(Vector2 fingerPosition)
    {
        var newGridHit = _gridSystem.ConvertScreenPositionToGridHit(fingerPosition);

        _beginTouchGridPosition = newGridHit;
    }

    private void OnNewFingerUpInput(Vector2 fingerPosition)
    {
        if (!_allowInput) return;
        var newGridHit = _gridSystem.ConvertScreenPositionToGridHit(fingerPosition);


        var endTouchGridPosition = newGridHit;

        if (endTouchGridPosition.HitGridPosition == _beginTouchGridPosition.HitGridPosition && !_currentSelectedGridPosition.HasValue)
        {
            _currentSelectedGridPosition = _beginTouchGridPosition;
            _gridSystem.SelectTileByGridPosition(_currentSelectedGridPosition.Value.HitGridPosition);
            return;
        }

        var isClickMove = _beginTouchGridPosition.HitGridPosition == endTouchGridPosition.HitGridPosition;

        if (isClickMove)
        {
            var endGridPosition = _gridSystem.CalculateClickedEndGridPosition(_currentSelectedGridPosition.Value.HitGridPosition, endTouchGridPosition.RawX, endTouchGridPosition.RawY, levelGridData.ClickTolerance);

            endGridPosition = _gridSystem.CheckGridBounds(endGridPosition);

            if (endGridPosition == _currentSelectedGridPosition.Value.HitGridPosition)
            {
                ResetCurrentGridPosition();
                return;
            }

            if (_gridSystem.IsDiagonalMove(_currentSelectedGridPosition.Value.HitGridPosition, endGridPosition))
            {
                ResetCurrentGridPosition();
                return;
            }


            HandleMove(_currentSelectedGridPosition.Value.HitGridPosition, endGridPosition);
        }
        else
        {
            var endGridPosition = _gridSystem.CalculateSwipeEndGridPosition(_beginTouchGridPosition, endTouchGridPosition, levelGridData.SwipeDirectionTolerance, levelGridData.SwipeMaxDiagonalDeviation);

            endGridPosition = _gridSystem.CheckGridBounds(endGridPosition);

            HandleMove(_beginTouchGridPosition.HitGridPosition, endGridPosition);
        }

        _currentSelectedGridPosition = null;
    }

    private void ResetCurrentGridPosition()
    {
        if (_currentSelectedGridPosition == null) return;
        _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.HitGridPosition);
        _currentSelectedGridPosition = null;
    }

    private void HandleMove(GridPosition beginGridPosition, GridPosition endGridPosition)
    {
        if (_currentSelectedGridPosition != null) _gridSystem.DeselectTileByGridPosition(_currentSelectedGridPosition.Value.HitGridPosition);

        _allowInput = false;

        var beginGridObject = _gridSystem.GetGridObjectByGridPosition(beginGridPosition);
        var endGridObject = _gridSystem.GetGridObjectByGridPosition(endGridPosition);

        if (beginGridObject == null || endGridObject == null || beginGridObject == endGridObject)
        {
            _allowInput = true;
            return;
        }

        if (!beginGridObject.GetMatch3BlockProfile.HasRule("Swap") || !endGridObject.GetMatch3BlockProfile.HasRule("Swap"))
        {
            _allowInput = true;
            return; 
        }

        var swapParameters = new SwapActionParameters
        {  
            Context = _actionContext,
            From = beginGridObject, 
            To = endGridObject, 
        };

        gridActionProcessor.ProcessAction(new SwapAction(swapParameters));
    }

    public void ShuffleGridOnHit()
    {
        _allowInput = false;
        ResetCurrentGridPosition();
        gridActionProcessor.CancelCurrentChain();

        _actionContext = new ActionContext
        {
            GridSystem = _gridSystem, 
            BlockVisualManager = blockVisualManager,
            MatchDetector = _matchDetector,
            LevelGridData = levelGridData,
            GridActionProcessor = gridActionProcessor,
            Match3BlockProfileContainer = match3BlockProfileContainer
        };
        
        gridActionProcessor.ProcessAction(new ReshuffleAction(new ReshuffleActionParameters{Context = _actionContext}));        
    }
}
