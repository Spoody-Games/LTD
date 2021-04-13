using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    public EnemyData m_data;
    public NavMeshAgent m_NavAgent;
    public Transform m_Target;
    public Transform m_MainTarget;
    NavMeshPathStatus prevStatus = NavMeshPathStatus.PathComplete;
    NavMeshPath prevPath;
    Vector3 targetpos;
    bool inRange = false;
    bool gameover = false;
    public GameObject m_Alarm;
    private void Awake()
    {
        PlacementController.FigurePlaced += OnFigurePlaced;
    }
    private void OnDestroy()
    {
        PlacementController.FigurePlaced -= OnFigurePlaced;
    }

    private void OnFigurePlaced(FigureSpawnSpot obj, Figure figurePlaced)
    {
        if (Vector3.Distance(figurePlaced.transform.position, this.transform.position) < figurePlaced.m_Data.m_AttackRadius)
            ShowAlarm();
    }

    void Start()
    {
        if (GameController.Instance.m_Buildings.Count > 0)
            m_MainTarget = GameController.Instance.m_Buildings[0];
        FindNewTarget();
        //m_NavAgent.speed = m_data.Speed;
    }
    public void Hit()
    {
        GameController.Instance.m_Enemies.Remove(transform);
        Destroy(gameObject);
    }

    public void FindNewTarget()
    {

        targetpos = m_MainTarget.GetComponent<Collider>().ClosestPoint(transform.position);
        m_Target = m_MainTarget;
        m_NavAgent.SetDestination(targetpos);

        /*
        if (GameController.Instance.m_Buildings.Count > 0)
        {
            if (m_data.BasePriority)
            {
                NavMeshPath path = new NavMeshPath();
                if (m_MainTarget)
                    m_NavAgent.CalculatePath(m_MainTarget.GetComponent<Collider>().ClosestPoint(transform.position), path);
                else return;
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    m_Target = m_MainTarget;
                    targetpos = m_MainTarget.GetComponent<Collider>().ClosestPoint(transform.position);
                }
                else
                {
                    m_Target = GameController.Instance.m_Buildings.GetClosestObject(transform);
                    targetpos = m_Target.GetComponent<Collider>().ClosestPoint(transform.position);
                }
            }
            else
            {
                m_Target = GameController.Instance.m_Buildings.GetClosestObject(transform);
                targetpos = m_Target.GetComponent<Collider>().ClosestPoint(transform.position);
            }
            m_NavAgent.SetDestination(targetpos);
        }
        else
        {
            gameover = true;
        }
        prevPath = m_NavAgent.path;
        */
    }
    public void ShowAlarm()
    {
        StartCoroutine(ShowAlarmRoutine());
    }

    IEnumerator ShowAlarmRoutine()
    {
        m_Alarm.SetActive(true);
        yield return new WaitForSeconds(1f);
        m_Alarm.SetActive(false);
    }


    void Update()
    {

        if (gameover) return;
        if (m_MainTarget == null) gameover = true;
        if (!inRange)
        {
            if (Vector3.Distance(transform.position, targetpos) < 2f)
            {
                Debug.LogWarning("inRange");
                inRange = true;
                StartCoroutine("Attack");
            }
        }

        /*
                if (m_NavAgent.path.status != prevStatus)
                {
                    prevStatus = m_NavAgent.path.status;
                    FindNewTarget();
                }
                if (!m_Target)
                {
                    FindNewTarget();
                }
                if (!inRange)
                {
                    if (Vector3.Distance(transform.position, targetpos) < 2f)
                    {
                        inRange = true;
                        StartCoroutine("Attack");
                    }
                }
        */
    }
    IEnumerator Attack()
    {
        if (!gameover)
        {
            if (m_Target)
            {
                if (m_Target == m_MainTarget)
                {
                    m_Target.GetComponent<MainBase>()?.TakeDamage(m_data.Damage);
                }
                else
                {
                    if (m_Target.GetComponent<Figure>())
                        if (m_Target.GetComponent<Figure>().m_Data.isTimed) yield return null;
                        else
                            m_Target.GetComponent<Figure>()?.TakeDamage(m_data.Damage);
                }
            }
            else
            {
                inRange = false;
                if (m_MainTarget)
                    FindNewTarget();
            }
            yield return new WaitForSeconds(m_data.ReloadTime);

            if (inRange)
                StartCoroutine("Attack");
        }
    }
}
