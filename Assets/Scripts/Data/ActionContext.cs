public struct ActionContext
{
    public GridSystem GridSystem;
    public BlockVisualManager BlockVisualManager;
    public MatchDetector MatchDetector;
    public LevelGridData LevelGridData;
    public GridActionProcessor GridActionProcessor;

    public ActionContext(GridSystem gridSystem, BlockVisualManager blockVisualManager, MatchDetector matchDetector, LevelGridData levelGridData, GridActionProcessor gridActionProcessor)
    {
        GridSystem = gridSystem;
        BlockVisualManager = blockVisualManager;
        MatchDetector = matchDetector;
        LevelGridData = levelGridData;
        GridActionProcessor = gridActionProcessor;
    }
}
