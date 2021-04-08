using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnim : MonoBehaviour
{
    public Material m_Mat;
    public float speed = 0.1f;
    private void Update()
    {
       
            var offset = m_Mat.mainTextureOffset;
            m_Mat.mainTextureOffset = new Vector2(offset.x + Time.deltaTime * speed, offset.y - Time.deltaTime * speed);
        

    }
}
