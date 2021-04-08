using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Camera m_Camera;
    private void Awake()
    {
        Instance = this;
        PlacementController.FigurePlaced += OnFigurePlaced;
    }
    private void OnDestroy()
    {
        PlacementController.FigurePlaced -= OnFigurePlaced;
    }
    public float hor;

    // Use this for initialization
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.fieldOfView = calcVertivalFOV(hor, m_Camera.aspect);
    }
    private void OnFigurePlaced(FigureSpawnSpot obj, Figure figurePlaced)
    {
        transform.DOShakePosition(0.1f, 0.5f, 5);
    }

    private float calcVertivalFOV(float hFOVInDeg, float aspectRatio)
    {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
        float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }

    void Update()
    {
        m_Camera.fieldOfView = calcVertivalFOV(hor, m_Camera.aspect);
    }
}