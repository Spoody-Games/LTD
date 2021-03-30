using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public GameObject m_VictoryPanel;
    public GameObject m_LossPanel;
     private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    public void GameOver(bool victory)
    {
        if (victory)
        {
            m_VictoryPanel.SetActive(true);
        }
        else
        {
            m_LossPanel.SetActive(true);
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }



}
