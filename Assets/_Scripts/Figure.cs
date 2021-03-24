using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public enum FigureType
{
    LineHorizontal,
    LineVertical,
    LDownLeft,
    LDownRight,
    LLeftDown,
    LLeftUp,
    LRightDown,
    LRightUp,
    LUpLeft,
    LUpRight,
    ZLeft,
    ZRight,
    ZupLeft,
    ZupRight,
    TDown,
    TLeft,
    TRight,
    TUp,
    Box,
    DEBUG
}
public class Figure : MonoBehaviour
{

    public float m_CurrentHealth;
    public GameObject m_Projectile;
    public FigureData m_Data;
    public GameObject m_HealtBar;
    public bool isActive = false;
    internal EnemyBehaviour m_Target;
    public Vector3 center;
    public GameObject[] blocks;
    public int mergefactor = 1;
    public List<Vector2Int> OccupyingSlots;
    public List<ShooterController> m_Shooters;
    public List<MeshRenderer> m_meshes;



    private IEnumerator Start()
    {
        m_CurrentHealth = m_Data.m_Health;
        Vector3 sc = new Vector3(m_CurrentHealth / m_Data.m_Health, 1, 1);
        m_HealtBar.transform.localScale = sc;
        //GetComponentsInChildren<ShooPos>().ToList().ForEach(x => m_ArchersShootPos.Add(x.transform));
        m_Shooters = GetComponentsInChildren<ShooterController>().ToList();


        var list = GetComponentsInChildren<Animator>().ToList();
        foreach (var anim in list)
        {
            anim.speed = 0;
            yield return new WaitForSeconds(Random.Range(0, 2f));
            anim.speed = 1;
        }
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
            m_Shooters.ForEach(x => x.StartShooting());
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
            StartCoroutine("Death");
            isActive = false;
        }
    }
    IEnumerator Death()
    {
        if (m_Shooters.Count > 0)
            for (int i = 0; i < m_Shooters.Count; i++)
            {
                Destroy(m_Shooters[i].gameObject);
                yield return new WaitForSeconds(0.1f);
            }
        transform.DOMoveY(transform.position.y - 3, 0.5f).OnComplete(() => Destroy(gameObject));
    }
    public void Merge()
    {
        Debug.LogWarning("merged");
        mergefactor++;
        m_CurrentHealth = m_Data.m_Health;
        if (m_Data.isTurret)
            GetComponentsInChildren<ShooterController>().ToList().ForEach(x => x.Upgrade());
        Vector3 sc = new Vector3(m_CurrentHealth / m_Data.m_Health, 1, 1);
        m_HealtBar.transform.localScale = sc;
    }
}
