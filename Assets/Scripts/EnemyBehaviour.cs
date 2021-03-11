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
        if (GameController.Instance.m_Buildings.Count > 0)
        {
            if (inRange) return;
            inRange = false;
            Debug.LogWarning("NEW TARGET");
            m_Target = GameController.Instance.m_Buildings.GetClosestObject(transform);
            targetpos = m_Target.GetComponent<Collider>().ClosestPoint(transform.position);
            m_NavAgent.SetDestination(targetpos);
        }
    }


    void Update()
    {
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
                m_Target.GetComponent<MainBase>().TakeDamage(m_data.Damage);
            }
            else
                m_Target.GetComponent<Figure>().TakeDamage(m_data.Damage);
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
