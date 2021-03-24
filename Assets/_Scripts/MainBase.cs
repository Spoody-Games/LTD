using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    public float m_MaxHealth = 1000;
    public GameObject m_HealtBar;
    float m_CurrentHealth;

    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    public void TakeDamage(float dmg)
    {
        m_CurrentHealth -= dmg;
        Vector3 sc = new Vector3(m_CurrentHealth / m_MaxHealth, 1, 1);
        m_HealtBar.transform.localScale = sc;
        if (m_CurrentHealth <= 0)
        {
            GameController.Instance.m_Buildings.Remove(transform);
            UIController.Instance.GameOver(false);
            Destroy(gameObject);
        }
    }

}
