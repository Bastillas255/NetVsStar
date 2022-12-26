using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllignToGrid : MonoBehaviour
{
    private StarGrid grid;
    StarNode myNode;
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        myNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        transform.position = myNode.worldPosition;//and center to it
    }
}
