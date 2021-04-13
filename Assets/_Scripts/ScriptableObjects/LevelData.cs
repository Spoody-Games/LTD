using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level Data", order = 3)]
public class LevelData : ScriptableObject
{
    public List<FigureData> m_figures;
    public List<RoadData> m_Road;
    public List<EnemiesData> m_Enemies;
    public List<ObstacleData> m_Obstacles;
    public Vector3 m_Baseposition = new Vector3(0, 0.5f, -25.03f);
    public List<int> m_ActiveSpawnPointIndex;
}

[System.Serializable]
public class ObstacleData
{
    public ObstacleData(Vector2Int a)
    {
        index = a;
    }
    public Vector2Int index;
}

[System.Serializable]
public class RoadData
{
    public Vector2Int index;
    public RoadData(int a, int b)
    {
        index.x = a; index.y = b;
    }
    public RoadData(Vector2Int a)
    {
        index = a;
    }
}
[System.Serializable]
public class EnemiesData
{
    public GameObject Prefab;
    public int Count;

}