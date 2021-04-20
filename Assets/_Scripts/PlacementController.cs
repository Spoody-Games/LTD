using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
[System.Serializable]
public class TowerLoadInfo
{
    public TowerLoadInfo(Vector2Int index, FigureData _data)
    {
        CoreIndex = index;
        m_data = _data;
    }
    public Vector2Int CoreIndex;
    public FigureData m_data;
}
public class PlacementController : MonoBehaviour
{
    #region Variables
    public static System.Action<FigureSpawnSpot, Figure> FigurePlaced;
    private Vector3 screenPoint;
    private Vector3 offset;
    Slot SelectedSlot;
    Vector3 StartPos;
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
    public GameObject m_HandObject;
    #endregion
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
        if (GameController.Instance.GameOver) return;
        m_HandObject.SetActive(true);
        screenPoint = CameraController.Instance.m_Camera.WorldToScreenPoint(gameObject.transform.position);
        canPlace = true;
        transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        for (int i = 0; i < LevelConstructor.Instance.leveltoload.m_TowerDatas.Count; i++)
        {
            if (m_Figure.m_Data == LevelConstructor.Instance.leveltoload.m_TowerDatas[i].m_data)
            {
                HighLightSpots(LevelConstructor.Instance.leveltoload.m_TowerDatas[i].CoreIndex, m_Figure.m_Data, true);
                break;
            }
        }
        // foreach (Slot slot in SlotGenerator.Instance.m_SlotsReferences)
        // {
        //     if (slot.IsFree())
        //     {
        //         slot.GetComponent<MeshRenderer>().enabled = true;
        //     }
        // }
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
        if (GameController.Instance.GameOver) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = CameraController.Instance.m_Camera.ScreenToWorldPoint(curScreenPoint) + offset;

        var plane = GameController.Instance.m_RayPlane;
        float moveY = Input.mousePosition.y / 2f;
        Ray ray;
        if (Input.mousePosition.y > Screen.height / 3.35)
        {
            Vector3 inpoffset = new Vector3(0, moveY, 0);
            ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition + inpoffset);
        }
        else
        {
            ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);
        }
        float planeHit;
        if (plane.Raycast(ray, out planeHit))
        {
            transform.position = ray.GetPoint(planeHit);
        }

        RaycastHit hit;
        Transform objectHit = null;
        closest = GetClosestSpot();
        if (closest != null)
        {
            if (!LevelConstructor.Instance.bDebugMode)
            {
                ghost.transform.position = closest.transform.position;
                if (closest.isTrash)
                {
                    ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloMat);
                }
                else
                if (closest.m_Figure.m_Data == m_Figure.m_Data && closest.m_Figure.mergefactor == m_Figure.mergefactor && !m_Figure.m_Data.isTimed)
                {
                    ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloMat);
                }
                else
                {
                    ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloCannotPlaceMat);
                }
                return;
            }
        }
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
                    else if (closest)
                    {
                        if (closest.m_Figure.m_Data.figureType == m_Figure.m_Data.figureType)
                            ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloMat);
                        else ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloCannotPlaceMat);

                    }
                    else ghost.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.material = m_HoloCannotPlaceMat);
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
            curPosition.y = 0.5f;
            ghost.transform.position = curPosition;
        }
    }
    FigureSpawnSpot closest;
    public FigureSpawnSpot GetClosestSpot()
    {
        var spawnpoints = FigureSpawner.Instance.m_Spots;
        FigureSpawnSpot closest = null;
        for (int i = 0; i < spawnpoints.Count; i++)
        {
            float dist = Mathf.Infinity;
            var tmpdist = dist;
            if (m_Spot != spawnpoints[i])
            {
                tmpdist = Vector3.Distance(transform.position, spawnpoints[i].transform.position);
                if (tmpdist < dist && tmpdist < 10f)
                {
                    dist = tmpdist;
                    closest = spawnpoints[i];
                }
            }
        }
        return closest;
    }
    private void OnMouseUp()
    {
        if (GameController.Instance.GameOver) return;
        m_HandObject.SetActive(false);
        //unhighlight spots
        for (int i = 0; i < LevelConstructor.Instance.leveltoload.m_TowerDatas.Count; i++)
        {
            if (m_Figure.m_Data == LevelConstructor.Instance.leveltoload.m_TowerDatas[i].m_data)
            {
                HighLightSpots(LevelConstructor.Instance.leveltoload.m_TowerDatas[i].CoreIndex, m_Figure.m_Data, false);
                break;
            }
        }
        // foreach (Slot slot in SlotGenerator.Instance.m_SlotsReferences)
        // {
        //     slot.GetComponent<MeshRenderer>().enabled = false;
        // }
        //lower Meshes
        m_MeshesToLift.ForEach(x =>
        {
            var pos = x.transform.localPosition;
            x.transform.DOLocalMoveY(pos.y - 1, 0.2f);
        });

        #region BaseMerging


        if (closest)
        {
            if (closest.isTrash)
            {
                transform.position = closest.transform.position;
                FigurePlaced?.Invoke(m_Spot, m_Figure);
                StartCoroutine(DelayedDestroy(0.2f));
                return;
            }
            if (closest.m_Figure.m_Data.figureType == m_Figure.m_Data.figureType)
            {
                if (closest.m_Figure.mergefactor == m_Figure.mergefactor)
                {
                    if (m_Figure.m_Data.isTimed)
                    {
                        WrongPlacement("Merging TimedTowers");
                        return;
                    }
                    ghost.transform.position = closest.transform.position;
                    transform.position = closest.transform.position;
                    closest.m_Figure.Merge();
                    FigurePlaced?.Invoke(m_Spot, m_Figure);
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
        Destroy(ghost);

        if (SelectedSlot)
        {
            Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(SelectedSlot);
            if (canPlace)
            {
                if (LevelConstructor.Instance.bDebugMode)
                {
                    TowerLoadInfo newTowerData = new TowerLoadInfo(CoreIndex, m_Figure.m_Data);
                    LevelConstructor.Instance.m_towers.Add(newTowerData);
                }

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
                PlacedTower();
                if (!LevelConstructor.Instance.bDebugMode)
                {
                    FigurePlaced?.Invoke(m_Spot, m_Figure);
                }
            }
            else
            {
                #region  ChekingNeighboringSlots
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
                            ghost.transform.position = SelectedSlot.m_OccupyingFigure.transform.position;
                            transform.position = SelectedSlot.m_OccupyingFigure.transform.position;

                            SelectedSlot.m_OccupyingFigure.Merge();
                            FigurePlaced?.Invoke(m_Spot, m_Figure);
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
                        PlacedTower();
                        Debug.Log("Placed");
                        if (!LevelConstructor.Instance.bDebugMode)
                        {
                            FigurePlaced?.Invoke(m_Spot, m_Figure);
                        }
                        return;
                    }
                }
                else
                {
                    WrongPlacement("Cannot Place");
                }
                #endregion
            }
        }
        else
        {
            WrongPlacement("Slot Not Found");
            return;
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
        Vector2Int CoreIndex = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(_slot);
        if (_slot.IsFree())
        {
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
        //when merging
        yield return new WaitForSeconds(time);

        Destroy(ghost);
        Destroy(gameObject);
    }
    void WrongPlacement(string reason)
    {
        canPlace = false;
        Debug.LogWarning("WrongPlacement: " + reason);
        transform.position = StartPos;
        transform.localScale = transform.localScale * 0.6f;
        Destroy(ghost);
    }
    public void PlacedTower()
    {
        var fig = GetComponent<Figure>();
        GameController.Instance.m_Buildings.Add(transform);
        fig.Activate();
        Destroy(this);
    }

    public void HighLightSpots(Vector2Int core, FigureData _data, bool state)
    {
        for (int i = 0; i < _data.indexes.Count; i++)
        {
            var x = core.x + _data.indexes[i].x;
            var y = core.y + _data.indexes[i].y;
            var slot = SlotGenerator.Instance.m_SlotsMatrix[x, y];
            if (slot.isOccupied)
            {
                if (slot.m_OccupyingFigure.mergefactor == m_Figure.mergefactor)
                    slot.GetComponent<MeshRenderer>().enabled = state;
            }
            else
                slot.GetComponent<MeshRenderer>().enabled = state;
        }
    }
}
