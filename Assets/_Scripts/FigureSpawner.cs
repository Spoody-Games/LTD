using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    public static FigureSpawner Instance;
    public LevelData m_data;
    public List<FigureSpawnSpot> m_Spots;
    public void SpawnFigures()
    {
        if (!LevelConstructor.Instance.bDebugMode)
            m_Spots.ForEach(x => x.Spawnfigure(m_data.m_figures.GetRandom().m_Prefab));
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
        Debug.Log("Figure Placed");
        obj.Spawnfigure(m_data.m_figures.GetRandom().m_Prefab);
    }
}