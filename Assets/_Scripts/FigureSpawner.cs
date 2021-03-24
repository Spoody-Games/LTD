using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    public List<GameObject> m_figures;
    public List<FigureSpawnSpot> m_Spots;
    private void Start()
    {
        m_Spots.ForEach(x => x.Spawnfigure(m_figures.GetRandom()));
    }
    private void Awake()
    {
        PlacementController.FigurePlaced += OnFigurePlaced;
    }
    private void OnDestroy()
    {
        PlacementController.FigurePlaced -= OnFigurePlaced;
    }

    private void OnFigurePlaced(FigureSpawnSpot obj)
    {
        Debug.LogWarning("PLACED");
        obj.Spawnfigure(m_figures.GetRandom());
    }
}
