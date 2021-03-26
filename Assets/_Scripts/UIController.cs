using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public void SpawnFigure(GameObject _figure)
    {
        var fig = Instantiate(_figure, new Vector3(0, 0, -35), Quaternion.identity);


    }
    public void DrawRoad()
    {
        GameController.Instance.DrawRoad = true;
    }

}
