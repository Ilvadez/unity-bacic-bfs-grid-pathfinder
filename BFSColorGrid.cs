using System.Collections.Generic;
using UnityEngine;

public class BFSColorGrid
{
    private List<Vector2> _objectesPosition = new (1000); 
    private Vector3 _size;
    public BFSColorGrid(Vector3 size)
    {
        _size = size;
    }
    public void AddToDrawGizmos(Vector2 newPos)
    {
        _objectesPosition.Add(newPos);      
    }

    public void Visual(Color color)
    {
        foreach(var i in _objectesPosition)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(new Vector2(i.x, i.y), _size);
        }
    }
}
