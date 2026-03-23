public struct GridTile
{
    private GridPosition _gridPosition;

    public GridTile(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public GridPosition GetGridTilePosition => _gridPosition;
}
