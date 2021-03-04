using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject m_Target;
    public List<Transform> m_Buildings;
    public List<Transform> m_SpawnPoints;
    public List<Transform> m_Enemies;
    public int EnemyCount;
    public GameObject m_enemy;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InvokeRepeating("Spawn", 1, 0.2f);
    }
    public void Spawn()
    {
        if (EnemyCount <= 0) { CancelInvoke("Spawn"); return; }
        EnemyCount--;
        var point = m_SpawnPoints.GetRandom();
        var enemy = Instantiate(m_enemy, point.position, Quaternion.identity);
        m_Enemies.Add(enemy.transform);
    }


}
