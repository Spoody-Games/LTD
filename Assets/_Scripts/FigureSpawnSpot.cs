using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawnSpot : MonoBehaviour
{
    public Figure m_Figure;
    public bool isTrash = false;
    public void Spawnfigure(GameObject figure)
    {
        m_Figure = Instantiate(figure, transform.position, Quaternion.identity).GetComponent<Figure>();
        m_Figure.GetComponent<PlacementController>().SetSpot(this);
        m_Figure.transform.localScale = m_Figure.transform.localScale * 0.6f;
    }
    public void RemoveFig()
    {
        m_Figure = null;
    }
}
