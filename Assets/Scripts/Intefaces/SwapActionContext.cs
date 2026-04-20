using System;

public struct SwapActionContext : IActionContext
{
    public GridObject From;
    public GridObject To;
    public Action<GridObject, GridObject> SwapGridObjectData;

    public SwapActionContext(GridObject from, GridObject to, Action<GridObject, GridObject> swapGridObjectData)
    {
        From = from;
        To = to;
        SwapGridObjectData = swapGridObjectData;
    }
}
