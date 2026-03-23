public struct GridTileData
{
    private GridPosition _gridPosition;

    public GridTileData(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public GridPosition GetGridTilePosition => _gridPosition;
}
