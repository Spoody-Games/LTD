using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public EnemyBehaviour m_Target;
    bool ready = false;
    public void Init(EnemyBehaviour target)
    {
        m_Target = target;
        ready = true;
    }
    private void Update()
    {
        if (ready)
        {
            if (m_Target)
            {
                var dist = Vector3.Distance(transform.position, m_Target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, m_Target.transform.position, 0.1f);
                if (dist < 0.1f)
                {
                    Debug.LogWarning("HIT");
                    m_Target.Hit();
                    Destroy(gameObject);
                }
            }
            else Destroy(gameObject);
        }
    }

}
