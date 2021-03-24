using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Camera m_Camera;
    private void Awake()
    {
        Instance = this;
    }
}
