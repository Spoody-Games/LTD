using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialAnim : MonoBehaviour
{
    public Material m_Mat;
    private void Start()
    {
        Color color = m_Mat.color;
        color.a = 0.8f;
        m_Mat.color = color;
        color.a = 0.4f;
        m_Mat.DOFade(0.4f, 0.8f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
