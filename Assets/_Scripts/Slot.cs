using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isRoad = false;
    public bool isObstacle = false;
    const int adjacentsSize = 4;
    public Figure m_OccupyingFigure;
    private void Start()
    {
        if (isObstacle)
        {
            //GetComponent<MeshRenderer>().enabled = true;
            GetComponent<NavMeshObstacle>().enabled = true;
        }
        
    }
    public void EnableMesh()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }
    public bool IsFree()
    {
        return !(isOccupied || isRoad || isObstacle);
    }
    public void Occupy(Figure figure)
    {
        m_OccupyingFigure = figure;
        //EnableMesh();
        isOccupied = true;
    }
    public void Deocuppy()
    {
        m_OccupyingFigure = null;
        isOccupied = false;
    }

    public Slot Left()
    {
        var matr = SlotGenerator.Instance.m_SlotsMatrix;
        Vector2Int index = matr.FindSlotIndexInMatrix(this);
        if (index.x - 1 > 0)
            if (matr[index.x - 1, index.y] != null)
                return matr[index.x - 1, index.y];
        return null;
    }
    public Slot Right()
    {
        var matr = SlotGenerator.Instance.m_SlotsMatrix;
        Vector2Int index = matr.FindSlotIndexInMatrix(this);
        if (index.x + 1 < matr.GetLength(0))
            if (matr[index.x + 1, index.y] != null)
                return matr[index.x + 1, index.y];
        return null;
    }
    public Slot Up()
    {
        var matr = SlotGenerator.Instance.m_SlotsMatrix;
        Vector2Int index = matr.FindSlotIndexInMatrix(this);
        if (index.y + 1 < matr.GetLength(1))
            if (matr[index.x, index.y + 1] != null)
                return matr[index.x, index.y + 1];
        return null;
    }
    public Slot Down()
    {
        var matr = SlotGenerator.Instance.m_SlotsMatrix;
        Vector2Int index = matr.FindSlotIndexInMatrix(this);
        if (index.y - 1 > 0)
            if (matr[index.x, index.y - 1] != null)
                return matr[index.x, index.y - 1];
        return null;
    }
    public bool CheckIndex(Vector2Int index)
    {
        return (index.x < 0 || index.x > 11) && (index.y < 0 || index.y > 13);
    }

    //recursively check neighboring slots for placement;
}
