using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainBase : MonoBehaviour
{
    public float m_MaxHealth = 1000;
    //public GameObject m_HealtBar;
    public MeshRenderer m_healthObj;
    float m_CurrentHealth;
    public List<TextMeshPro> m_texts;

    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_texts.ForEach(x => x.text = ((int)m_CurrentHealth / 10).ToString());
        m_healthObj.material.color = Color.HSVToRGB(m_CurrentHealth / m_MaxHealth / 3f, 1, 1);
        // m_healthObj.transform.DOLocalRotate(new Vector3(0, 180, 0), 2).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
    public void TakeDamage(float dmg)
    {
        m_healthObj.material.color = Color.HSVToRGB(m_CurrentHealth / m_MaxHealth / 3f, 1, 1);
        m_CurrentHealth -= dmg;
        Vector3 sc = new Vector3(m_CurrentHealth / m_MaxHealth, 1, 1);
        // m_HealtBar.transform.localScale = sc;
        if (m_CurrentHealth <= 0)
        {
            GameController.Instance.m_Buildings.Remove(transform);
            UIController.Instance.GameOver(false);
            Destroy(gameObject);
        }
        m_texts.ForEach(x => x.text = ((int)m_CurrentHealth / 10).ToString());
    }

}
