using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test: MonoBehaviour
{
    public Transform goalPos;
    public Transform startPos;
    public Vector2[] Obstancles;
    public Grid grid;

    private BFSGrid _bFSGrid;

    void Awake()
    {
        _bFSGrid = new BFSGrid(100, 100, grid, Obstancles);
    }
    void Start()
    {
        List<Vector2> positions = _bFSGrid.GetPath(startPos, goalPos).ToList();
        positions.Reverse();

    }
    void OnDrawGizmos()
    {
        _bFSGrid.OnDrawGizmos();
    }
}