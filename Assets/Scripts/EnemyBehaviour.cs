using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyData m_data;
    public NavMeshAgent m_NavAgent;
    public Transform m_Target;

    void Start()
    {
        m_NavAgent.SetDestination(GameController.Instance.m_Buildings.GetClosestObject(transform).position);
        //m_NavAgent.speed = m_data.Speed;
    }


    void Update()
    {

    }

}
