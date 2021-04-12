using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class LevelConstructor : MonoBehaviour
{
    public static LevelConstructor Instance;
    private void Awake()
    {
        Instance = this;
    }
    public LevelData leveltoload;
    public Material m_obstMat;
    public GameObject m_debugBtns;
    public Transform m_Base;

    public bool bDebugMode = false;
    public bool bDrawRoad = false;
    public List<GameObject> m_DebugBuildings;
    public List<Slot> m_Roads;
    public List<GameObject> m_ObstaclesPrefabs;
    public GameObject m_roadPrefab;
    public void SpawnFigure(GameObject _figure)
    {
        var fig = Instantiate(_figure, new Vector3(0, 0, -35), Quaternion.identity);
        m_DebugBuildings.Add(fig);
    }
    public void RemoveRoad()
    {
        foreach (var slot in SlotGenerator.Instance.m_SlotsReferences)
        {
            if (slot.isRoad)
            {
                slot.isRoad = false;
                slot.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    public void DrawRoad()
    {
        bDrawRoad = !bDrawRoad;
        UIController.Instance.m_DrawText.text = "DRAW MODE : " + bDrawRoad;
    }
    public void SaveSlots()
    {

        LevelData asset = ScriptableObject.CreateInstance<LevelData>();
        asset.m_Road = new List<RoadData>();
        asset.m_figures = new List<FigureData>();
        asset.m_Obstacles = new List<ObstacleData>();
        asset.m_Baseposition = m_Base.transform.position;

        foreach (var slot in SlotGenerator.Instance.m_SlotsReferences)
        {
            var a = SlotGenerator.Instance.m_SlotsMatrix.FindSlotIndexInMatrix(slot);
            if (slot.isRoad)
            {
                RoadData tmp = new RoadData(a);
                asset.m_Road.Add(tmp);
            }
            else
            if (slot.isOccupied)
            {
                if (slot.m_OccupyingFigure)
                {
                    FigureData data = slot.m_OccupyingFigure.m_Data;
                    if (!asset.m_figures.Contains(data))
                    {
                        asset.m_figures.Add(data);
                    }
                }
            }
            else
            {
                ObstacleData data = new ObstacleData(a);
                asset.m_Obstacles.Add(data);
            }
        }

        AssetDatabase.CreateAsset(asset, "Assets/Data/LevelDataTest.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

    }
    public void Load()
    {
        leveltoload.m_Road.ForEach(x =>
        {
            var slot = SlotGenerator.Instance.m_SlotsMatrix.GetSlotByIndex(x.index);
            slot.isRoad = true;
            Instantiate(m_roadPrefab, slot.transform.position, Quaternion.identity);
        });
        leveltoload.m_Obstacles.ForEach(x =>
        {
            var slot = SlotGenerator.Instance.m_SlotsMatrix.GetSlotByIndex(x.index);
            slot.isObstacle = true;
            var obj = Instantiate(m_ObstaclesPrefabs[Random.Range(0, m_ObstaclesPrefabs.Count)], slot.transform.position, Quaternion.identity);
            obj.GetComponent<NavMeshObstacle>().enabled = true;
        });
    }
    private void Update()
    {
        if (bDebugMode)
        {
            if (bDrawRoad)
            {
                if (Input.GetMouseButton(0))
                {
                    Ray ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    Transform objectHit = null;
                    if (Physics.Raycast(ray, out hit, 1000, 1 << 8))
                    {
                        objectHit = hit.transform;
                        var SelectedSlot = hit.transform.GetComponent<Slot>();
                        SelectedSlot.isRoad = true;
                        SelectedSlot.GetComponent<MeshRenderer>().enabled = true;
                        if (!m_Roads.Contains(SelectedSlot))
                            m_Roads.Add(SelectedSlot);
                    }
                }
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
            {
                if (bDrawRoad)
                {
                    if (m_Roads.Count > 0)
                    {
                        var last = m_Roads[m_Roads.Count - 1];
                        m_Roads.Remove(last);
                        last.isRoad = false;
                        last.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else
                if (m_DebugBuildings.Count > 0)
                {
                    var last = m_DebugBuildings[m_DebugBuildings.Count - 1];
                    last.GetComponent<Figure>().ClearSlots();
                    m_DebugBuildings.Remove(last);
                    GameController.Instance.m_Buildings.Remove(last.transform);
                    Destroy(last);
                }
            }
        }
    }
}
