using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlotGenerator : MonoBehaviour
{
    public static SlotGenerator Instance;
    public GameObject m_SlotPrefab;
    public float slotsize = 3.5f;
    public int width = 10;
    public int lenght = 12;
    public Slot[] m_SlotsReferences;
    public Slot[,] m_SlotsMatrix;

    
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        // Slot Generator
        // for (int i = 0; i < lenght; i++)
        // {
        //     for (int j = 0; j < width; j++)
        //     {
        //         var x = (slotsize * j - width * slotsize / 2f) + slotsize / 2;
        //         var y = 0;
        //         var z = (slotsize * i - lenght * slotsize / 2f) + slotsize / 2;
        //         var slot = Instantiate(m_SlotPrefab, new Vector3(x, y, z), Quaternion.identity);
        //         slot.transform.localScale *= slotsize * 0.97f;
        //     }
        // }

        m_SlotsMatrix = m_SlotsReferences.ConvertMatrix(width, lenght);
    }




}
