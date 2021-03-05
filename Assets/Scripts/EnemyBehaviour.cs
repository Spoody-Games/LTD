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

    void Start()
    {
        m_MainTarget = GameController.Instance.m_Buildings[0];
        m_NavAgent.SetDestination(m_MainTarget.position);
        m_Target = m_MainTarget;
        //m_NavAgent.speed = m_data.Speed;
    }
    public void Hit()
    {
        GameController.Instance.m_Enemies.Remove(transform);
        //Destroy(gameObject);
    }

    public void FindNewTarget()
    {
        Debug.LogWarning("Changing Target");
        if (m_NavAgent.path.status == NavMeshPathStatus.PathComplete)
        {
            m_Target = m_MainTarget;
        }
        else
        {
            m_Target = GameController.Instance.m_Buildings.GetClosestObject(transform);
        }
    }


    void Update()
    {
        if (m_NavAgent.path.status != prevStatus)
        {
            prevStatus = m_NavAgent.path.status;
            FindNewTarget();
        }
        //Debug.LogWarning(Vector3.Distance(transform.position, m_Target.position));

    }

}
