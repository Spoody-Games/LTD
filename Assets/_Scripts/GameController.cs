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
    public bool DebugMode = false;
    public bool DrawRoad = false;
    public List<GameObject> m_DebugBuildings;
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
        if (DebugMode)
        {
            if (DrawRoad)
            {
                if (Input.GetMouseButton(0))
                {
                    Ray ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    Transform objectHit = null;
                    if (Physics.Raycast(ray, out hit, 1000, 1 << 8))
                    {
                        objectHit = hit.transform;
                        var SelectedSlot = hit.transform.GetComponent<Slot>();
                        SelectedSlot.isRoad = true;
                        SelectedSlot.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
            {
                if (m_DebugBuildings.Count > 0)
                {
                    var last = m_DebugBuildings[m_DebugBuildings.Count - 1];
                    m_DebugBuildings.Remove(last);
                    m_Buildings.Remove(last.transform);
                    Destroy(last);
                }
            }
        }
    }
}
