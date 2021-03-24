using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawnSpot : MonoBehaviour
{
    public void Spawnfigure(GameObject figure)
    {
        var fg = Instantiate(figure, transform.position, Quaternion.identity);
        fg.GetComponent<PlacementController>().SetSpot(this);
    }
}
