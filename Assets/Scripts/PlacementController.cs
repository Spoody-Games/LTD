using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacementController : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    Slot SelectedSlot;
    Vector3 StartPos;
    bool hasPlaced;
    internal GameObject mainObject;
    Figure m_Figure;
    FigureData figureData;
    public bool DEBUG = false;

    bool canPlace = true;
    private void Start()
    {
        m_Figure = GetComponent<Figure>();
        figureData = m_Figure.m_Data;
    }
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        StartPos = transform.position;
        hasPlaced = false;

        var prevobj = Instantiate(gameObject);
        prevobj.name = figureData.Name;

        mainObject = Instantiate(gameObject);
        mainObject.GetComponent<PlacementController>().enabled = false;
        mainObject.GetComponent<NavMeshObstacle>().enabled = false;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        curPosition.y = 0;
        mainObject.transform.position = curPosition;

        Ray ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Transform objectHit = null;
        if (Physics.Raycast(ray, out hit, 100, 1 << 8))
        {
            objectHit = hit.transform;
            SelectedSlot = hit.transform.GetComponent<Slot>();
        }
        if (objectHit)
            transform.position = objectHit.position;
        else
        {
            SelectedSlot = null;
            objectHit = null;
            transform.position = curPosition;
        }
    }
    private void OnMouseUp()
    {
        Destroy(mainObject);
        if (SelectedSlot)
        {
            Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(SelectedSlot);

            if (DEBUG)
            {
                for (int i = 0; i < figureData.indexes.Count; i++)
                {
                    var x = CoreIndex.x + figureData.indexes[i].x;
                    var y = CoreIndex.y + figureData.indexes[i].y;
                    var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                    slot.Occupy(m_Figure);

                    GetComponent<NavMeshObstacle>().enabled = true;
                    transform.GetComponentsInChildren<NavMeshObstacle>().ToList().ForEach(n => n.enabled = true);
                }
                hasPlaced = true;
                Debug.Log("Placed");
                return;
            }
            if (SelectedSlot.IsFree())
            {
                //PLACING
                Debug.Log("attempting Placement");
                for (int i = 0; i < figureData.indexes.Count; i++)
                {
                    var x = CoreIndex.x + figureData.indexes[i].x;
                    var y = CoreIndex.y + figureData.indexes[i].y;

                    if (x < 0 || x >= 12) { WrongPlacement("wrong X pos"); return; }
                    if (y < 0 || y >= 14) { WrongPlacement("wrong Y pos"); return; }

                    var tmpslot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                    if (!tmpslot.IsFree())
                    {
                        WrongPlacement("One of the Slots is occupied");
                        return;
                    }
                }
                if (canPlace)
                {
                    for (int i = 0; i < figureData.indexes.Count; i++)
                    {
                        var x = CoreIndex.x + figureData.indexes[i].x;
                        var y = CoreIndex.y + figureData.indexes[i].y;
                        var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                        slot.Occupy(m_Figure);

                        GetComponent<NavMeshObstacle>().enabled = false;
                        transform.GetComponentsInChildren<NavMeshObstacle>().ToList().ForEach(n => n.enabled = true);
                    }
                    hasPlaced = true;
                    Debug.Log("Placed");
                }
                else
                {
                    WrongPlacement("Cannot place");
                    return;
                }
            }
            else
            {
                //MERGING 
                if (SelectedSlot.m_OccupyingFigure.m_Data.figureType == m_Figure.m_Data.figureType)
                {
                    Debug.Log("attempting merge");
                    for (int i = 0; i < figureData.indexes.Count; i++)
                    {
                        var x = CoreIndex.x + figureData.indexes[i].x;
                        var y = CoreIndex.y + figureData.indexes[i].y;

                        if (x < 0 || x >= 12) { WrongPlacement("wrong X pos"); return; }
                        if (y < 0 || y >= 14) { WrongPlacement("wrong Y pos"); return; }

                        var tmpslot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                        if (tmpslot.isOccupied)
                        {
                            if (tmpslot.m_OccupyingFigure.m_Data.figureType != m_Figure.m_Data.figureType)
                            {
                                WrongPlacement("Merging different figures");
                                return;
                            }
                        }
                        else
                        {
                            WrongPlacement("Figures not aligned");
                            return;
                        }
                    }
                    Debug.Log("Merged");
                    hasPlaced = true;
                }
                else
                {
                    WrongPlacement("Merging different figures");
                    return;
                }
            }
        }
        else
        {
            WrongPlacement("Slot Not Found");
            return;
        }

        if (!hasPlaced)
        {
            transform.position = StartPos;
        }
        else
            Destroy(this);
    }
    void WrongPlacement(string reason)
    {
        canPlace = false;
        Debug.LogWarning("WrongPlacement: " + reason);
        Destroy(gameObject);
    }

}
