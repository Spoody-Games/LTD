using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    Slot SelectedSlot;
    Vector3 StartPos;
    bool hasPlaced;
    public GameObject mainObject;
    public FigureData figureData;
    bool canPlace = true;
    private void Start()
    {

    }
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        StartPos = transform.position;
        hasPlaced = false;
        var prevobj = Instantiate(gameObject);
        prevobj.name = figureData.Name;
        mainObject = Instantiate(gameObject);
        mainObject.GetComponent<MouseMove>().enabled = false;
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
            if (SelectedSlot.IsFree())
            {
                Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(SelectedSlot);

                for (int i = 0; i < figureData.indexes.Count; i++)
                {
                    var x = CoreIndex.x + figureData.indexes[i].x;
                    var y = CoreIndex.y + figureData.indexes[i].y;

                    if (x < 0 || x >= 12) { WrongPlacement(); return; }
                    if (y < 0 || y >= 14) { WrongPlacement(); return; }

                    var tmpslot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                    if (!tmpslot.IsFree())
                    {
                        WrongPlacement();
                    }
                }
                if (canPlace)
                {
                    for (int i = 0; i < figureData.indexes.Count; i++)
                    {
                        var x = CoreIndex.x + figureData.indexes[i].x;
                        var y = CoreIndex.y + figureData.indexes[i].y;
                        var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                        slot.Occupy();
                    }
                    hasPlaced = true;
                }
                else
                {
                    WrongPlacement();
                }
            }
            else
            {
                //check for merging

                WrongPlacement();
            }
        }
        else
        {
            WrongPlacement();
        }


        if (!hasPlaced)
        {
            transform.position = StartPos;
        }
    }
    void WrongPlacement()
    {
        canPlace = false;
        Debug.LogWarning("WrongPlacement");
        Destroy(gameObject);
    }

}
