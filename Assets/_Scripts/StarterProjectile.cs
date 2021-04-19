using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StarterProjectile : MonoBehaviour
{
    public AnimationCurve curve;
    public EnemyBehaviour end;

    Vector3 start;
    float time;
    public float Speed;
    public float height;
    void Start()
    {
        start = transform.position;
    }


    void Update()
    {
        time += Time.deltaTime * Speed;
        Vector3 pos = Vector3.Lerp(start, end.transform.position, time);
        pos.y += curve.Evaluate(time) * height;
        transform.position = pos;
        if (Vector3.Distance(transform.position, end.transform.position) < 0.1f)
        {
            var rigid = end.GetComponentInChildren<Rigidbody>();
            rigid.transform.parent = null;
            rigid.isKinematic= false;
            rigid.useGravity = true;
            rigid.AddTorque(Vector3.left,ForceMode.Impulse);
            Destroy(rigid.gameObject,1f);
            end.Hit(1000f);
            Destroy(gameObject);
        }
    }

}
