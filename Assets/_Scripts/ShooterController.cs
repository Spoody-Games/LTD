using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public bool isShooting = false;
    public List<GameObject> m_level;
    Figure m_figParent;
    public GameObject m_Target;
    public Transform m_ShootPos;
    public int level = 0;
    float damping = 5;
    public AudioSource m_Audio;
    public void Upgrade()
    {
        if (level >= 4) return;
        m_level[level].SetActive(false);
        level++;
        if (level <= m_level.Count)
            m_level[level].SetActive(true);
        GetComponentInChildren<Animator>().SetBool("Shooting", isShooting);
    }
    public void StartShooting()
    {
        isShooting = true;
        StartCoroutine("Shoot");
    }
    public void StopShooting()
    {
        isShooting = false;
    }
    private void Awake()
    {
        m_figParent = GetComponentInParent<Figure>();
        m_ShootPos = GetComponentInChildren<ShooPos>().transform;
    }
    private void Update()
    {
        if (m_Target)
        {
            var rotation = Quaternion.LookRotation(m_Target.transform.position - transform.position);
            rotation.x = 0;
            rotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
    }

    IEnumerator Shoot()
    {
        if (m_Target)
        {
            var dist = Vector3.Distance(transform.position, m_Target.transform.position);
            if (dist > m_figParent.m_Data.m_AttackRadius)
            {
                m_Target = null;
            }
        }
        else
        {
            var tmp = GameController.Instance.m_Enemies.GetClosestObject(transform);
            if (tmp)
            {
                var dist = Vector3.Distance(transform.position, tmp.position);
                if (dist < m_figParent.m_Data.m_AttackRadius)
                {
                    m_Target = tmp.gameObject;
                }
            }
        }
        if (!m_Target)
        {
            GetComponentInChildren<Animator>().SetBool("Shooting", false);
            yield return null;
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("Shooting", true);
            var projectile = Instantiate(m_figParent.m_Projectile, m_ShootPos.position, Quaternion.identity);
            EnemyBehaviour target = m_Target.GetComponent<EnemyBehaviour>();
            projectile.GetComponent<Projectile>().Init(target, m_figParent.m_Data.m_Damage);
            m_Audio.Play();
        }

        yield return new WaitForSeconds(m_figParent.m_Data.m_ReloadTime / m_figParent.mergefactor);
        StartCoroutine("Shoot");

    }

}