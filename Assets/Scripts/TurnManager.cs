using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //based on if both agents know which tile to go next, let time resume
    //after both are done, pause again
    //repeat
    public GameObject pacman;
    public GameObject ghost;

    PacManMovement pmm;
    Unit unit;


    public void Start()
    {
        //pacman= GetComponentInParent<PacManMovement>();
        pmm=pacman.GetComponent<PacManMovement>();
        unit = ghost.GetComponent<Unit>();
    }

    public void Update()
    {
        
        if (!pmm.isMoving)
        {
            unit.StopAllCoroutines();
            Debug.Log("coruutines stoped");
        }
    }
}
