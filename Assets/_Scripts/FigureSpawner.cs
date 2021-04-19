using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    public static FigureSpawner Instance;
    public LevelData m_data;
    public List<FigureSpawnSpot> m_Spots;
    Figure m_prevFig;
    public void SpawnFigures()
    {
        if (!LevelConstructor.Instance.bDebugMode)
            m_Spots.ForEach(x =>
            {
                if (!x.isTrash)
                {
                    x.Spawnfigure(GetNewFigure());
                }
            });
    }
    GameObject GetNewFigure()
    {
        GameObject _fig = m_data.m_figures.GetRandom().m_Prefab;
        if (m_prevFig != null)
        {
            Debug.LogWarning(m_prevFig);
            Debug.LogWarning(_fig);
            if (_fig.GetComponent<Figure>().m_Data.isTimed && m_prevFig.m_Data.isTimed)
            {
                return GetNewFigure();
            }
        }
        return _fig;
    }
    private void Awake()
    {
        Instance = this;
        PlacementController.FigurePlaced += OnFigurePlaced;
    }
    private void OnDestroy()
    {
        PlacementController.FigurePlaced -= OnFigurePlaced;
    }
    private void OnFigurePlaced(FigureSpawnSpot obj, Figure figurePlaced)
    {
        m_prevFig = figurePlaced;
        Debug.Log("Figure Placed");
        obj.Spawnfigure(GetNewFigure());
    }
}