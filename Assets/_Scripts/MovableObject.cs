using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public bool Move = false;
    void Update()
    {
        if (!Move) return;
        if (Input.GetMouseButton(0))
        {
            if (LevelConstructor.Instance.bDebugMode)
            {
                var plane = GameController.Instance.m_RayPlane;
                float moveY = Input.mousePosition.y / 2f;
                //Vector3 inpoffset = new Vector3(0, moveY, 0);
                Ray ray = CameraController.Instance.m_Camera.ScreenPointToRay(Input.mousePosition);
                float planeHit;
                if (plane.Raycast(ray, out planeHit))
                {
                    transform.position = ray.GetPoint(planeHit);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Slot>().isOccupied = true;
        other.GetComponent<MeshRenderer>().enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Slot>().isOccupied = false;
        other.GetComponent<MeshRenderer>().enabled = false;
    }
}
