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
    public Vector3[] path;
    // Start is called before the first frame update
    void Start()
    {
        pacManMovement = pacObject.GetComponent<PacManMovement>();
        unit = ghostObject.GetComponent<Unit>();
    }

    void LateUpdate()
    {
        if (pacManMovement.turnCount != LastTurn)
        {
            Debug.Log("Jugador: "+ pacManMovement.transform.position);
            Debug.Log("Adversario: " + unit.transform.position);
            Debug.Log("Recompensa: "+ pacManMovement.closestReward);

            //Distancia J-A
            PathRequestManager.RequestPath(pacManMovement.transform.position, ghostObject.transform.position, OnPathFound1);

            //Distancia A-R
            PathRequestManager.RequestPath(ghostObject.transform.position, pacManMovement.closestReward, OnPathFound2);

            //Distancia J-R
            PathRequestManager.RequestPath(pacManMovement.transform.position, pacManMovement.closestReward, OnPathFound3);

            Debug.Log("RecObtenida: " + pacManMovement.rewardNumber);
            Debug.Log("Tiempo: " + Time.deltaTime);
            Debug.Log("Turno: "+ pacManMovement.turnCount);

            LastTurn = pacManMovement.turnCount;
        }
    }

    public void OnPathFound1(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            Debug.Log("Distancia J-A: " + path.Length);
        }
    }

    public void OnPathFound2(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            Debug.Log("Distancia A-R: " + path.Length);
        }
    }

    public void OnPathFound3(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            Debug.Log("Distancia J-R: " + path.Length);
        }
    }


}
