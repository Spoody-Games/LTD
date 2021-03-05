using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FigureType
{
    Line,
    Box,
    DEBUG
}
public class Figure : MonoBehaviour
{
    public float m_AttackRadius;
    public float m_Damage;
    public float m_ReloadTime;

    public GameObject m_Projectile;
    public FigureData m_Data;
    public bool isActive = false;
    EnemyBehaviour m_Target;
    public Vector3 center;
    public GameObject[] blocks;
    private void Start()
    {

    }
    private void Update()
    {

    }
    public void Activate()
    {
        isActive = true;
        StartCoroutine("Shoot");
    }
    IEnumerator Shoot()
    {
        if (isActive)
        {
            if (m_Target)
            {
                var dist = Vector3.Distance(center, m_Target.transform.position);
                if (dist > m_AttackRadius)
                {
                    m_Target = null;
                }
            }
            else
            {
                var tmp = GameController.Instance.m_Enemies.GetClosestObject(transform);
                if (tmp)
                {
                    var dist = Vector3.Distance(center, tmp.position);
                    if (dist < m_AttackRadius)
                    {
                        SetTarget(tmp);
                    }
                }
            }


            if (!m_Target)
            {
                yield return null;
            }
            else
            {
                var projectile = Instantiate(m_Projectile, center, Quaternion.identity);
                projectile.GetComponent<Projectile>().Init(m_Target);
            }
            yield return new WaitForSeconds(m_ReloadTime);
            StartCoroutine("Shoot");
        }
    }
    public void SetTarget(Transform target)
    {
        if (target != null)
        {
            m_Target = target.GetComponent<EnemyBehaviour>();
            StartCoroutine("Shoot");
        }
        else
        {
            m_Target = null;
            StopCoroutine("Shoot");
        }
    }


    public void findCenter()
    {
        var totalX = 0f;
        var totalZ = 0f;
        foreach (var block in blocks)
        {
            totalX += block.transform.position.x;
            totalZ += block.transform.position.z;
        }
        var centerX = totalX / blocks.Length;
        var centerZ = totalZ / blocks.Length;
        center = new Vector3(centerX, 0, centerZ);
    }
}
