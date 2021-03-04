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
    bool isActive = false;
    GameObject m_Target;
    private void Start()
    {

    }
    private void Update()
    {
        while (!m_Target)
        {
            var tmp = GameController.Instance.m_Enemies.GetClosestObject(transform);
            if (Vector3.Distance(transform.position, tmp.position) < m_AttackRadius)
            {
                m_Target = tmp.gameObject;
            }
        }
    }
    IEnumerator Shoot()
    {
        if (!m_Target) yield return null;
        else
        {
            // var projectile = Instantiate(m_projectile);
            // projectile.target = m_Target;
            yield return new WaitForSeconds(m_ReloadTime);
            StartCoroutine(Shoot());
        }
    }
}
