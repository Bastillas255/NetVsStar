using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceModules : MonoBehaviour
{
    public GameObject pacObject;
    public GameObject ghostObject;

    PacManMovement pacManMovement;
    Unit unit;

    int LastTurn=-1;

    // Start is called before the first frame update
    void Start()
    {
        pacManMovement = pacObject.GetComponent<PacManMovement>();
        unit = ghostObject.GetComponent<Unit>();
    }

    void Update()
    {
        if (pacManMovement.turnCount != LastTurn)
        {
            Debug.Log("Jugador: "+ pacManMovement.transform.position);
            Debug.Log("Adversario: " + unit.transform.position);
            Debug.Log("Recompensa: "+ pacManMovement.closestReward);
            Debug.Log("Distancia J-A: "+ unit.path.Length);
            //Debug.Log("Distancia A-R: " + unit.path.Length);
            //Debug.Log("Distancia J-R: " + PacManMovement.path.Length);
            Debug.Log("RecObtenida: " + pacManMovement.rewardNumber);
            Debug.Log("Tiempo: " + Time.deltaTime);
            Debug.Log("Turno: "+ pacManMovement.turnCount);


            LastTurn = pacManMovement.turnCount;
        }
    }
}
