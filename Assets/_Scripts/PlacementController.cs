using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class PlacementController : MonoBehaviour
{
    public static System.Action<FigureSpawnSpot, Figure> FigurePlaced;
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
    public Material m_HoloCannotPlaceMat;
    public List<GameObject> m_MeshesToLift;
    bool canPlace = false;
    bool mustmerge = false;

    Slot TmpSlot = null;
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
    public void OnMouseDown()
    {
        screenPoint = CameraController.Instance.m_Camera.WorldToScreenPoint(gameObject.transform.position);
        hasPlaced = false;
        canPlace = true;
        transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        foreach (Slot slot in SlotGenerator.Instance.m_SlotsReferences)
        {
            if (slot.IsFree())
            {
                slot.GetComponent<MeshRenderer>().enabled = true;
            }
        }

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
            x.transform.DOLocalMoveY(pos.y + 1, 0.2f);
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
            if (TmpSlot != SelectedSlot)
            {
                TmpSlot = SelectedSlot;
                canPlace = CheckSlot(SelectedSlot, m_Figure);

                if (canPlace)
                {
                    ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloMat);
                }
                else
                {
                    if (SelectedSlot.m_OccupyingFigure)
                    {
                        if (SelectedSlot.m_OccupyingFigure.m_Data == m_Figure.m_Data && SelectedSlot.m_OccupyingFigure.mergefactor == m_Figure.mergefactor && !m_Figure.m_Data.isTimed)
                        {
                            ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloMat);
                        }
                        else
                            ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloCannotPlaceMat);
                    }
                    else
                        ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloCannotPlaceMat);
                }
            }
        }
        if (objectHit)
        {
            ghost.transform.position = objectHit.position;
        }
        else
        {
            SelectedSlot = null;
            objectHit = null;
            ghost.transform.position = curPosition;
        }
    }
    private void OnMouseUp()
    {
        foreach (Slot slot in SlotGenerator.Instance.m_SlotsReferences)
        {
            slot.GetComponent<MeshRenderer>().enabled = false;
        }
        #region BaseMerging
        var spawnpoints = FigureSpawner.Instance.m_Spots;
        FigureSpawnSpot closest = null;
        for (int i = 0; i < spawnpoints.Count; i++)
        {
            float dist = 9999;
            if (m_Spot != spawnpoints[i])
            {
                dist = Vector3.Distance(transform.position, spawnpoints[i].transform.position);
            }
            if (dist < 10f)
            {
                closest = spawnpoints[i];
            }
        }
        if (closest)
        {
            if (closest.m_Figure.m_Data.figureType == m_Figure.m_Data.figureType)
            {
                if (closest.m_Figure.mergefactor == m_Figure.mergefactor)
                {
                    if (m_Figure.m_Data.isTimed)
                    {
                        WrongPlacement("Merging TimedTowers");
                        Destroy(ghost.gameObject);
                        return;
                    }
                    ghost.transform.position = closest.transform.position;
                    transform.position = closest.transform.position;
                    closest.m_Figure.Merge();
                    StartCoroutine(DelayedDestroy(0.2f));
                    return;
                }
            }
            else
            {
                WrongPlacement("Merging Different Figures");
                return;
            }
        }
        #endregion

        transform.position = ghost.transform.position;
        m_MeshesToLift.ForEach(x =>
        {
            var pos = x.transform.localPosition;
            x.transform.DOLocalMoveY(pos.y - 1, 0.2f);
        });
        Destroy(ghost);

        if (SelectedSlot)
        {
            Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(SelectedSlot);
            if (canPlace)
            {
                Debug.LogWarning("Placing");
                for (int i = 0; i < figureData.indexes.Count; i++)
                {
                    var x = CoreIndex.x + figureData.indexes[i].x;
                    var y = CoreIndex.y + figureData.indexes[i].y;
                    var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                    slot.Occupy(m_Figure);
                    m_Figure.OccupyingSlots.Add(new Vector2Int(x, y));
                    transform.GetComponentsInChildren<NavMeshObstacle>().ToList().ForEach(n => n.enabled = true);
                }
                hasPlaced = true;
                Debug.Log("Placed");
                if (!LevelConstructor.Instance.bDebugMode)
                {
                    FigurePlaced?.Invoke(m_Spot, m_Figure);
                }
            }
            else
            {
                Slot Tmps = CheckNeighboringSlots(SelectedSlot, m_Figure);
                if (Tmps != null)
                {
                    CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(Tmps);
                    SelectedSlot = Tmps;
                    if (mustmerge)
                    {
                        if (SelectedSlot.m_OccupyingFigure.mergefactor == m_Figure.mergefactor)
                        {
                            if (m_Figure.m_Data.isTimed)
                            {
                                WrongPlacement("Merging TimedTowers");
                                return;
                            }
                            SelectedSlot.m_OccupyingFigure.Merge();
                            StartCoroutine(DelayedDestroy(0.2f));
                            return;
                        }
                        else
                        {
                            WrongPlacement("Merging different leveled figures");
                            return;
                        }
                    }
                    else
                    {
                        ghost.transform.position = Tmps.transform.position;
                        transform.position = ghost.transform.position;
                        Debug.LogWarning("Placing");
                        for (int i = 0; i < figureData.indexes.Count; i++)
                        {
                            var x = CoreIndex.x + figureData.indexes[i].x;
                            var y = CoreIndex.y + figureData.indexes[i].y;
                            var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                            slot.Occupy(m_Figure);
                            m_Figure.OccupyingSlots.Add(new Vector2Int(x, y));
                            transform.GetComponentsInChildren<NavMeshObstacle>().ToList().ForEach(n => n.enabled = true);
                        }
                        hasPlaced = true;
                        Debug.Log("Placed");
                        if (!LevelConstructor.Instance.bDebugMode)
                        {
                            FigurePlaced?.Invoke(m_Spot, m_Figure);
                        }
                        return;
                    }
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

    public Slot CheckNeighboringSlots(Slot _Origin, Figure _fig)
    {
        mustmerge = false;
        List<Slot> adjacentSlots = new List<Slot>();
        adjacentSlots.Add(_Origin.Left());
        adjacentSlots.Add(_Origin.Right());
        adjacentSlots.Add(_Origin.Up());
        adjacentSlots.Add(_Origin.Down());
        if (_Origin.Left() != null)
        {
            adjacentSlots.Add(_Origin.Left().Up());
            adjacentSlots.Add(_Origin.Left().Down());
        }
        if (_Origin.Right() != null)
        {
            adjacentSlots.Add(_Origin.Right().Up());
            adjacentSlots.Add(_Origin.Right().Down());
        }
        for (int i = 0; i < adjacentSlots.Count; i++)
        {
            var check = CheckSlot(adjacentSlots[i], m_Figure);
            if (check || mustmerge)
            {
                return adjacentSlots[i];
            }
        }
        return null;
    }
    public bool CheckSlot(Slot _slot, Figure _fig)
    {
        if (_slot == null) return false;
        Debug.LogWarning(_slot);
        Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(_slot);
        if (_slot.IsFree())
        {
            Debug.LogWarning("CheckingSlot");
            for (int i = 0; i < _fig.m_Data.indexes.Count; i++)
            {
                var x = CoreIndex.x + _fig.m_Data.indexes[i].x;
                var y = CoreIndex.y + _fig.m_Data.indexes[i].y;

                if (x < 0 || x >= 12) { return false; }
                if (y < 0 || y >= 14) { return false; }

                var tmpslot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
                if (!tmpslot.IsFree())
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            if (_slot.m_OccupyingFigure)
                if (_slot.m_OccupyingFigure.m_Data == m_Figure.m_Data)
                    mustmerge = true;
            return false;
        }
    }

    IEnumerator DelayedDestroy(float time)
    {
        hasPlaced = true;
        yield return new WaitForSeconds(time);

        FigurePlaced?.Invoke(m_Spot, m_Figure);

        Destroy(ghost);
        Destroy(gameObject);

    }
    void WrongPlacement(string reason)
    {
        canPlace = false;
        Debug.LogWarning("WrongPlacement: " + reason);
        transform.position = StartPos;
        transform.localScale = transform.localScale * 0.8f;
        //Destroy(gameObject);
    }
}
