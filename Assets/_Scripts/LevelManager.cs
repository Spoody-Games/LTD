using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int m_Level = 0;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            m_Level = PlayerPrefs.GetInt("Level");
        }
        else
        {
            PlayerPrefs.SetInt("Level", 0);
        }
    }
    public static void AdvanceLevel()
    {
        m_Level++;
        PlayerPrefs.SetInt("Level", m_Level);
    }

}
