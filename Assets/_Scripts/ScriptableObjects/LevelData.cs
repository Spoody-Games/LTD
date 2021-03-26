using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Level Data", order = 3)]
public class LevelData : ScriptableObject
{
    public List<ObstacleData> m_ObstacleList;
    public List<FigureData> m_figures;
}

[System.Serializable]
public class ObstacleData
{
    public Vector2Int index;
    public float m_ObstacleId;
}