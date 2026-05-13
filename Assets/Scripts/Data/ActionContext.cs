public struct ActionContext
{
    public GridSystem GridSystem;
    public BlockVisualManager BlockVisualManager;
    public MatchDetector MatchDetector;
    public LevelGridData LevelGridData;
    public GridActionProcessor GridActionProcessor;
    public Match3BlockProfileContainer Match3BlockProfileContainer;

    public ActionContext
    (
        GridSystem gridSystem,
        BlockVisualManager blockVisualManager, 
        MatchDetector matchDetector, 
        LevelGridData levelGridData, 
        GridActionProcessor gridActionProcessor, 
        Match3BlockProfileContainer match3BlockProfileContainer
    )
    {
        GridSystem = gridSystem;
        BlockVisualManager = blockVisualManager;
        MatchDetector = matchDetector;
        LevelGridData = levelGridData;
        GridActionProcessor = gridActionProcessor;
        Match3BlockProfileContainer = match3BlockProfileContainer;
    }
}
