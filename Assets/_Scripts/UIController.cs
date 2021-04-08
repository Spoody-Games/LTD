using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public GameObject m_VictoryPanel;
    public GameObject m_LossPanel;
    public GameObject m_DebugBtns;
    public Text m_DrawText;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (LevelConstructor.Instance.bDebugMode)
        {
            m_DebugBtns.SetActive(true);
        }
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
