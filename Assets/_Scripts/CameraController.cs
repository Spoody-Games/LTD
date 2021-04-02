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
    public float hor;

    // Use this for initialization
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.fieldOfView = calcVertivalFOV(hor, m_Camera.aspect);

    }

    private float calcVertivalFOV(float hFOVInDeg, float aspectRatio)
    {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
        float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        m_Camera.fieldOfView = calcVertivalFOV(hor, m_Camera.aspect);

    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVChanger : MonoBehaviour
{


    private Camera cam;

    public float hor;


    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = calcVertivalFOV(hor, cam.aspect);

    }

    private float calcVertivalFOV(float hFOVInDeg, float aspectRatio)
    {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
        float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        cam.orthographicSize = calcVertivalFOV(hor, cam.aspect);
        
    }
}*/
