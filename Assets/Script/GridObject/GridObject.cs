public class GridObject
{
    private GridPosition _gridPosition;

    public GridObject(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public GridPosition GetGridTilePosition => _gridPosition;
}
