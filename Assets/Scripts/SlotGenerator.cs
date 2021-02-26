using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlotGenerator : MonoBehaviour
{
    public GameObject m_SlotPrefab;
    public float slotsize = 3.5f;
    public int width = 10;
    public int lenght = 12;
    public List<Slot> m_SlotsReferences;
    public List<List<Slot>> m_SlotsMatrix;
    void Start()
    {
        for (int i = 0; i < lenght; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var x = (slotsize * j - width * slotsize / 2f) + slotsize / 2;
                var y = 0;
                var z = (slotsize * i - lenght * slotsize / 2f) + slotsize / 2;
                var slot = Instantiate(m_SlotPrefab, new Vector3(x, y, z), Quaternion.identity);
                slot.transform.localScale *= slotsize * 0.97f;

            }
        }

    }


}
