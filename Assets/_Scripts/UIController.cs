using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public GameObject m_VictoryPanel;
    public GameObject m_LossPanel;
    public Material m_obstMat;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    public void GameOver(bool victory)
    {
        if (victory)
        {
            m_VictoryPanel.SetActive(true);
        }
        else
        {
            m_LossPanel.SetActive(true);
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void SpawnFigure(GameObject _figure)
    {
        var fig = Instantiate(_figure, new Vector3(0, 0, -35), Quaternion.identity);
        GameController.Instance.m_DebugBuildings.Add(fig);
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
        GameController.Instance.DrawRoad = !GameController.Instance.DrawRoad;
    }
    public void SaveSlots()
    {
        LevelData asset = ScriptableObject.CreateInstance<LevelData>();
        asset.m_Road = new List<RoadData>();
        asset.m_figures = new List<FigureData>();
        asset.m_Obstacles = new List<ObstacleData>();

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
    public void Load(LevelData m_Data)
    {
        m_Data.m_Road.ForEach(x =>
        {
            var slot = SlotGenerator.Instance.m_SlotsMatrix.GetSlotByIndex(x.index);
            slot.isRoad = true;
            slot.GetComponent<MeshRenderer>().enabled = true;
        });
        m_Data.m_Obstacles.ForEach(x =>
        {
            var slot = SlotGenerator.Instance.m_SlotsMatrix.GetSlotByIndex(x.index);
            slot.isObstacle = true;
            slot.GetComponent<MeshRenderer>().material = m_obstMat;
            slot.GetComponent<MeshRenderer>().enabled = true;
        });
    }

}
