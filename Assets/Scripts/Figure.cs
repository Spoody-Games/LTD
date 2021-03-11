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
    int level = 1;
    public float m_CurrentHealth;
    public GameObject m_Projectile;
    public FigureData m_Data;
    public GameObject m_HealtBar;
    public bool isActive = false;
    EnemyBehaviour m_Target;
    public Vector3 center;
    public GameObject[] blocks;
    int mergefactor = 1;
    public List<Vector2Int> OccupyingSlots;

    private void Start()
    {

        m_CurrentHealth = m_Data.m_Health;
        Vector3 sc = new Vector3(m_CurrentHealth / m_Data.m_Health, 1, 1);
        m_HealtBar.transform.localScale = sc;
    }
    private void Update()
    {
        if (m_Data.isTimed)
            TakeDamage(Time.deltaTime);
    }
    public void Activate()
    {
        isActive = true;
        if (!m_Data.isTimed)
            StartCoroutine("Shoot");
    }
    IEnumerator Shoot()
    {
        if (isActive)
        {
            if (m_Target)
            {
                var dist = Vector3.Distance(center, m_Target.transform.position);
                if (dist > m_Data.m_AttackRadius)
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
                    if (dist < m_Data.m_AttackRadius)
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
            yield return new WaitForSeconds(m_Data.m_ReloadTime / mergefactor);
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
    public void TakeDamage(float dmg)
    {
        if (!isActive) return;
        m_CurrentHealth -= dmg;
        Vector3 sc = new Vector3(m_CurrentHealth / m_Data.m_Health, 1, 1);
        m_HealtBar.transform.localScale = sc;
        if (m_CurrentHealth <= 0)
        {
            GameController.Instance.m_Buildings.Remove(transform);
            foreach (Vector2Int pos in OccupyingSlots)
            {
                SlotGenerator.Instance.m_SlotsMatrix[pos.x, pos.y].Deocuppy();
            }
            Destroy(gameObject);
        }
    }
    public void Merge()
    {
        Debug.LogWarning("merged");
        mergefactor++;
        m_CurrentHealth = m_Data.m_Health;
        Vector3 sc = new Vector3(m_CurrentHealth / m_Data.m_Health, 1, 1);
        m_HealtBar.transform.localScale = sc;
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
