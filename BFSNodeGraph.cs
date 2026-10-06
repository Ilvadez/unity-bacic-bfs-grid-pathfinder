using UnityEngine;

public class BFSNodeGraph
{
    private BFSNodeGraph _parent;
    public BFSNodeGraph GetParent => _parent;
    public Vector2Int GridPosition {get; private set;}
    public Vector2Int WorldPosition {get; private set;}
    public bool IsWalkable {get; private set;}
    public BFSNodeGraph(Vector2Int worldPosition, Vector2Int gridPosition, bool isWalkable )
    {
        WorldPosition = worldPosition;
        GridPosition = gridPosition;
        IsWalkable = isWalkable;
    }
    public void SetParent(BFSNodeGraph parent) => _parent = parent;
    public void Clear()
    {
        _parent = null;
    }
}