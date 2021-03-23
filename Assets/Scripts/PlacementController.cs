using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class PlacementController : MonoBehaviour
{
    public static System.Action<FigureSpawnSpot> FigurePlaced;
    private Vector3 screenPoint;
    private Vector3 offset;
    Slot SelectedSlot;
    Vector3 StartPos;
    bool hasPlaced;
    internal GameObject ghost;
    Figure m_Figure;
    FigureData figureData;
    FigureSpawnSpot m_Spot;
    public bool DEBUG = false;
    public Material m_HoloMat;
    public List<GameObject> m_MeshesToLift;
    bool canPlace = true;
    private void Start()
    {
        m_Figure = GetComponent<Figure>();
        figureData = m_Figure.m_Data;
    }
    public void SetSpot(FigureSpawnSpot spot)
    {
        m_Spot = spot;
        StartPos = transform.position;

    }
    void OnMouseDown()
    {

        screenPoint = CameraController.Instance.m_Camera.WorldToScreenPoint(gameObject.transform.position);
        hasPlaced = false;
        canPlace = true;

        // var prevobj = Instantiate(gameObject);
        // prevobj.name = figureData.Name;

        ghost = new GameObject();
        ghost.transform.position = transform.position;
        ghost.transform.localScale = transform.localScale;
        m_Figure.m_meshes.ForEach(x =>
        {
            var msh = Instantiate(x.gameObject, ghost.transform);
            msh.transform.localPosition += Vector3.down / 2;
            msh.GetComponent<MeshRenderer>().material = m_HoloMat;
        });
        m_MeshesToLift.ForEach(x =>
        {
            var pos = x.transform.localPosition;
            x.transform.DOLocalMoveY(pos.y + 1, 0.5f);
        });
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = CameraController.Instance.m_Camera.ScreenToWorldPoint(curScreenPoint) + offset;
        curPosition.y = 0;


        var plane = GameController.Instance.m_RayPlane;
        Vector3 inpoffset = new Vector3(0, 150, 0);
        Ray ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition + inpoffset);
        float planeHit;
        if (plane.Raycast(ray, out planeHit))
        {
            transform.position = ray.GetPoint(planeHit);
        }

        RaycastHit hit;
        Transform objectHit = null;
        if (Physics.Raycast(ray, out hit, 1000, 1 << 8))
        {
            objectHit = hit.transform;
            SelectedSlot = hit.transform.GetComponent<Slot>();
        }
        if (objectHit)
            ghost.transform.position = objectHit.position;
        else
        {
            SelectedSlot = null;
            objectHit = null;
            ghost.transform.position = curPosition;
        }
    }
    private void OnMouseUp()
    {
        transform.position = ghost.transform.position;
        m_MeshesToLift.ForEach(x =>
{
    var pos = x.transform.localPosition;
    x.transform.DOLocalMoveY(pos.y - 1, 0.5f);
});
        Destroy(ghost);
        if (SelectedSlot)
        {
            Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(SelectedSlot);

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
                        m_Figure.OccupyingSlots.Add(new Vector2Int(x, y));
                        GetComponent<NavMeshObstacle>().enabled = false;
                        transform.GetComponentsInChildren<NavMeshObstacle>().ToList().ForEach(n => n.enabled = true);
                    }
                    hasPlaced = true;
                    Debug.Log("Placed");
                    FigurePlaced?.Invoke(m_Spot);
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
                    Figure fig = null;
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
                        fig = tmpslot.m_OccupyingFigure;
                    }
                    Debug.Log("Merged");
                    FigurePlaced?.Invoke(m_Spot);
                    fig.Merge();
                    Destroy(gameObject);
                    return;
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
        {
            var fig = GetComponent<Figure>();
            Destroy(this);
            GameController.Instance.m_Buildings.Add(transform);
            fig.Activate();
        }
    }
    void WrongPlacement(string reason)
    {
        canPlace = false;
        Debug.LogWarning("WrongPlacement: " + reason);
        transform.position = StartPos;
        //Destroy(gameObject);
    }

}
