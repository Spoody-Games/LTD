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
    Vector3 targetpos;
    bool inRange = false;
    bool gameover = false;

    void Start()
    {
        if (GameController.Instance.m_Buildings.Count > 0)
            m_MainTarget = GameController.Instance.m_Buildings[0];
        if (m_Target)
        {
            m_NavAgent.SetDestination(m_Target.position);
        }
        else
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
        if (GameController.Instance.m_Buildings.Count > 0)
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
            m_NavAgent.SetDestination(targetpos);
        }
        else
        {
            gameover = true;
        }
    }


    void Update()
    {
        if (gameover) return;
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
    }
    IEnumerator Attack()
    {
        if (m_Target)
        {
            if (m_Target == m_MainTarget)
            {
                m_Target.GetComponent<MainBase>()?.TakeDamage(m_data.Damage);
            }
            else
                m_Target.GetComponent<Figure>()?.TakeDamage(m_data.Damage);
        }
        else
        {
            inRange = false;
            FindNewTarget();
        }
        yield return new WaitForSeconds(m_data.ReloadTime);

        if (inRange)
            StartCoroutine("Attack");
    }
}
