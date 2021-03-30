using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject m_MainBase;
    public List<Transform> m_Buildings;
    public List<Transform> m_SpawnPoints;
    public List<Transform> m_Enemies;
    bool started = false;
    public int EnemyCount;
    public GameObject m_enemy;
    public Plane m_RayPlane;
    public float interval;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_RayPlane = new Plane(Vector3.up, Vector3.zero);
        InvokeRepeating("Spawn", 3, interval);
    }
    public void Spawn()
    {

        if (EnemyCount <= 0) { CancelInvoke("Spawn"); return; }
        EnemyCount--;
        var point = m_SpawnPoints.GetRandom();
        var enemy = Instantiate(m_enemy, point.position, Quaternion.identity);
        m_Enemies.Add(enemy.transform);
        started = true;
    }
    private void Update()
    {
        if (started)
        {
            if (m_Enemies.Count == 0)
            {
                started = false;
                UIController.Instance.GameOver(true);
            }
        }

    }
}
