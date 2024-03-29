﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy Data", order = 2)]
public class EnemyData : ScriptableObject
{
    public float Speed;
    public float Health;
    public float Damage;
    public float ReloadTime;
    public int coinReward;
}
