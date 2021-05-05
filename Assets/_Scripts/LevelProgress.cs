using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UISlot
{
    public Sprite background;
    public Color textColor;
}

public class LevelProgress : MonoBehaviour
{
    public UISlot nonActive;
    public UISlot active;
    public UISlot current;

    public Transform container;

    public List<Transform> slots;
    void Start()
    {
        int currentLevel = LevelManager.m_Level;
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var text = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            var background = slot.GetComponent<Image>();
            int index = i + (slots.Count / 2 < currentLevel ? currentLevel - slots.Count / 2 : 0);
            text.text = (index+1).ToString();
            UISlot slotData = nonActive;

            if (currentLevel == index)
                slotData = current;
            if (currentLevel > index)
                slotData = active;

            text.color = slotData.textColor;
            background.sprite = slotData.background;
        }
    }

}