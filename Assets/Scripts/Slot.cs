using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isRoad = false;
    const int adjacentsSize = 4;
    public Slot[] adjacents = new Slot[adjacentsSize];
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
