using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public GameObject m_VictoryPanel;
    public GameObject m_LossPanel;
    public GameObject m_MenuPanel;
    public GameObject m_DebugBtns;
    public Button m_NoThanksBtn;
    public Button m_NoThanksBtn2;
    public Text m_DrawText;
    public AudioSource m_VictoryAudio;
    public AudioSource m_LossAudio;
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
    public void StartGame()
    {
        m_MenuPanel.SetActive(false);
        GameController.Instance.StartGame();
    }

    public void GameOver(bool victory)
    {
        if (victory)
        {
            m_VictoryPanel.SetActive(true);
            NoThanksAppear(m_NoThanksBtn);
            m_VictoryAudio.Play();
        }
        else
        {
            m_LossAudio.Play();
            m_LossPanel.SetActive(true);
            NoThanksAppear(m_NoThanksBtn2);
        }
    }
    int count = 0;
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot("Screenshots/Screen" + count + ".png");
            count++;
        }
#endif
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    private void NoThanksAppear(Button noThanks)
    {
        var text =
        noThanks.GetComponentInChildren<TextMeshProUGUI>();
        text.alpha = 0f;
        DOTween.To(() => text.alpha, x => text.alpha = x, 1, 2f).SetDelay(3f).OnStart(() =>
        {
            noThanks.interactable = true;
        });
    }

}
