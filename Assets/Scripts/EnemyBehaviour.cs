using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    public NavMeshAgent m_NavAgent;
    public Transform m_Target;
    // Start is called before the first frame update
    void Start()
    {
        m_NavAgent.SetDestination(m_Target.position);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
