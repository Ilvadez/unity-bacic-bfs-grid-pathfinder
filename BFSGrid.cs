using System.Collections.Generic;
using UnityEngine;
public class BFSGrid
{
    private readonly Vector2Int[] _directionNeighbours = {new Vector2Int(1, 0), 
    new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)};
    private int _width;
    private int _height;
    private BFSNodeGraph[,] _graph;
    

    private HashSet<Vector2> _obstanclesPosition;
    private Grid _worldGrid;

    private BFSColorGrid _visualFindingPath;
    private BFSColorGrid _visualGetPath ;
    private BFSColorGrid _visualObstancles ;
    public BFSGrid(int width, int height, Grid worldGrid, Vector2[] obstanclesPosition = null)
    {
        _width = width;
        _height = height;
        _worldGrid = worldGrid;
        _visualFindingPath = new BFSColorGrid(_worldGrid.cellSize);
        _visualGetPath  = new BFSColorGrid(_worldGrid.cellSize);
        _visualObstancles  = new BFSColorGrid(_worldGrid.cellSize);
        SetObstancle(obstanclesPosition);
        InitGrid(_width, _height);
    }

    private void InitGrid(int width, int height)
    {
        _graph = new BFSNodeGraph[width, height];
        int y = Mathf.RoundToInt(-height / 2);
        int x = Mathf.RoundToInt(-width / 2);
        Vector2Int worldPosition = new Vector2Int(x, y);
        for(int i = 0; i < width; i ++)
        {
            for(int j = 0; j < height; j++)
            {
                if(_obstanclesPosition.Contains(worldPosition))
                {
                    _graph[i, j] = new BFSNodeGraph(worldPosition, new Vector2Int(i, j), false);
                    _visualObstancles.AddToDrawGizmos(ConvertToWorld((Vector3Int)_graph[i, j].WorldPosition));
                }
                else
                {
                    _graph[i, j] = new BFSNodeGraph(worldPosition, new Vector2Int(i, j), true);
                }
                worldPosition.y += 1;
            }
            worldPosition.y = y;
            worldPosition.x += 1;
        }
    }
    public Queue<Vector2> GetPath(Transform startWorldPos, Transform endWorldPos)
    {
        ClearParentNode();
        Vector2Int goalGridWorldPos = ConvertToGrid(endWorldPos.position);
        Vector2Int startGridWorldPos = ConvertToGrid(startWorldPos.position);
        Debug.Log(goalGridWorldPos);
        Debug.Log(startGridWorldPos);
        BFSNodeGraph goalNode = FindGoalPosition(startGridWorldPos, goalGridWorldPos);
        Queue<Vector2> path = new Queue<Vector2>();
        if(goalNode != null)
        {
            BFSNodeGraph currentNode = goalNode;
            bool isLookingFor = true;
            while(isLookingFor)
            {
                _visualGetPath.AddToDrawGizmos(ConvertToWorld((Vector3Int)currentNode.WorldPosition));
                path.Enqueue(ConvertToWorld((Vector3Int)currentNode.WorldPosition));
                if(currentNode.GetParent == null)
                {
                    break;
                }
                currentNode = currentNode.GetParent;
            }
        }
        foreach(var i in path)
        {
            Debug.Log(i);
        }
        return path;
    }
    private BFSNodeGraph FindGoalPosition(Vector2Int startWorldPos, Vector2Int goalPos)
    {
        Queue<BFSNodeGraph> queueNode = new Queue<BFSNodeGraph>();
        BFSNodeGraph startNode = FindNode(startWorldPos, _graph);
        queueNode.Enqueue(startNode);
        _visualFindingPath.AddToDrawGizmos(ConvertToWorld((Vector3Int)startNode.WorldPosition));
        BFSNodeGraph goalNode = null;

        Queue<BFSNodeGraph> reachedNode = new Queue<BFSNodeGraph>();
        reachedNode.Enqueue(startNode);
        while(queueNode.Count > 0)
        {
            BFSNodeGraph node = queueNode.Dequeue();
            List<BFSNodeGraph> neighbours = GetNeighbours(node, _graph);
            foreach(var i in neighbours)
            {
                if(!reachedNode.Contains(i))
                {
                    queueNode.Enqueue(i);
                    reachedNode.Enqueue(i);
                    i.SetParent(node);
                    _visualFindingPath.AddToDrawGizmos(ConvertToWorld((Vector3Int)i.WorldPosition));
                }
                if(i.WorldPosition == goalPos)
                {
                    return i;
                }
            }
        }
        return goalNode;
    }
    private List<BFSNodeGraph> GetNeighbours(BFSNodeGraph position, BFSNodeGraph[,] graph)
    {
        List<BFSNodeGraph> result = new List<BFSNodeGraph>(4);
        for(int i = 0; i < _directionNeighbours.Length; i++)
        {
            if(position.GridPosition.x + _directionNeighbours[i].x < 0 || 
            position.GridPosition.x + _directionNeighbours[i].x >= graph.GetLength(0))
            {
                continue;
            }
            else if(position.GridPosition.y + _directionNeighbours[i].y < 0 ||
            position.GridPosition.y + _directionNeighbours[i].y >= graph.GetLength(1))
            {
                continue;
            }
            Vector2Int pos = position.GridPosition + _directionNeighbours[i];
            if(_obstanclesPosition.Contains(_graph[pos.x, pos.y].WorldPosition))
            {
                continue;
            }
            result.Add(_graph[pos.x, pos.y]);
        }
        return result;
    }
    private BFSNodeGraph FindNode(Vector2Int worldPosition, BFSNodeGraph[,] graph)
    {
        BFSNodeGraph result = null;
        for(int i = 0; i < graph.GetLength(0); i++)
        {
            for(int j = 0; j < graph.GetLength(1); j++)
            {
                if(graph[i, j].WorldPosition == worldPosition)
                {
                    result = graph[i, j];
                }
            }
        }
        return result;
    }
    private void SetObstancle(Vector2[] obstanclePosition)
    {
        _obstanclesPosition = new HashSet<Vector2>();
        for(int i = 0; i < obstanclePosition.Length; i++)
        {
            _obstanclesPosition.Add(ConvertToGrid(obstanclePosition[i]));
        }
    }
    private void ClearParentNode()
    {
        for(int i = 0; i < _graph.GetLength(0); i++)
        {
            for(int j = 0; j < _graph.GetLength(1); j++)
            {
                _graph[i, j].Clear();
            }
        }
    }
    private Vector2Int ConvertToGrid(Vector3 position)
    {
        return (Vector2Int)_worldGrid.WorldToCell(position);
    }
    private Vector2 ConvertToWorld(Vector3Int gridPos)
    {
        return _worldGrid.GetCellCenterWorld(gridPos);
    }
    public void OnDrawGizmos()
    {
        _visualFindingPath.Visual(Color.aquamarine);
        _visualGetPath.Visual(Color.rosyBrown);
        _visualObstancles.Visual(Color.black);
    }
}
