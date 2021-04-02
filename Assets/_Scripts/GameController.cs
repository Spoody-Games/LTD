using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<LevelData> LevelDatas;
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
    private IEnumerator Start()
    {
        m_RayPlane = new Plane(Vector3.up, Vector3.zero);

        //getData
        if (!LevelConstructor.Instance.bDebugMode)
        {
            if (LevelManager.m_Level == LevelDatas.Count)
                LevelManager.m_Level = 0;
            Debug.LogWarning(LevelManager.m_Level);
            var data = LevelDatas[LevelManager.m_Level];
            LevelConstructor.Instance.leveltoload = data;
            FigureSpawner.Instance.m_data = data;
            EnemyCount = data.m_Enemies[0].Count;

            LevelConstructor.Instance.Load();
            FigureSpawner.Instance.SpawnFigures();
            yield return new WaitForSeconds(3);
            StartCoroutine(Spawn(m_enemy));
            InvokeRepeating("Spawn", 3, interval);
        }


    }
    IEnumerator Spawn(GameObject _Prefab)
    {

        if (EnemyCount >= 0)
        {
            EnemyCount--;
            var point = m_SpawnPoints.GetRandom();
            var enemy = Instantiate(m_enemy, point.position, Quaternion.identity);
            m_Enemies.Add(enemy.transform);
            started = true;
        }
        yield return new WaitForSeconds(interval);
        StartCoroutine(Spawn(_Prefab));
    }
    private void Update()
    {
        if (started)
        {
            if (m_Enemies.Count == 0)
            {
                started = false;
                LevelManager.AdvanceLevel();
                UIController.Instance.GameOver(true);

            }
        }

    }
}
