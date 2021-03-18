using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public EnemyBehaviour m_Target;
    bool ready = false;
    Vector3 target;
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
                target = m_Target.transform.position;
            }
            if (target == Vector3.zero) Destroy(gameObject);
            var dist = Vector3.Distance(transform.position, target);


            transform.position = Vector3.MoveTowards(transform.position, target, Mathf.Min(Time.deltaTime * 25f, dist));

            if (dist < 0.1f)
            {
                if (m_Target)
                    m_Target.Hit();
                Destroy(gameObject);
            }
        }
    }

}
