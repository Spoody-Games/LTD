﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FigureData", menuName = "ScriptableObjects/Figure Data", order = 1)]
public class FigureData : ScriptableObject
{
    public string Name;
    public FigureType figureType;
    public bool isTurret;
    public bool isTimed;
    public List<Vector2Int> indexes;

    public float m_AttackRadius;
    public float m_Damage;
    public float m_ReloadTime;
    public float m_Health;
    public GameObject m_Prefab;

}
