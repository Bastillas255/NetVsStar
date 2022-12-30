using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralSensor : MonoBehaviour
{
    PacManMovement pacManMovement;
    Unit unit;

    int LastTurn = -1;
    public Vector3[] path;
    
    public float[] traceModules = new float[11];

    // Start is called before the first frame update
    void Start()
    {
        unit = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Unit>();
        unit.enabled = true;
        pacManMovement = GetComponent<PacManMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pacManMovement.turnCount != LastTurn)
        {
            //Debug.Log("Jugador: "+ pacManMovement.transform.position);//Vector3
            traceModules[0] = Mathf.Round(pacManMovement.transform.position.x);
            traceModules[1] = Mathf.Round(pacManMovement.transform.position.y);

            //Debug.Log("Adversario: " + unit.transform.position);//Vector3
            traceModules[2] = Mathf.Round(unit.transform.position.x);
            traceModules[3] = Mathf.Round(unit.transform.position.y);

            //Debug.Log("Recompensa: "+ pacManMovement.closestReward);//Vector3
            traceModules[4] = Mathf.Round(pacManMovement.closestReward.x);
            traceModules[5] = Mathf.Round(pacManMovement.closestReward.y);

            //Utilizar variable path para obtener la distancia después de hacer su respectivo request
            //Distancia J-A
            PathRequestManager.RequestPath(pacManMovement.transform.position, unit.transform.position, OnPathFound1);//int
            traceModules[6] = path.Length;

            //Distancia A-R
            PathRequestManager.RequestPath(unit.transform.position, pacManMovement.closestReward, OnPathFound2);//int
            traceModules[7] = path.Length;

            //Distancia J-R
            PathRequestManager.RequestPath(pacManMovement.transform.position, pacManMovement.closestReward, OnPathFound3);//int
            traceModules[8] = path.Length;

            //Debug.Log("RecObtenida: " + pacManMovement.rewardNumber);//int
            traceModules[9] = pacManMovement.rewardNumber;

            //Debug.Log("Turno: "+ pacManMovement.turnCount);//int
            traceModules[10] = pacManMovement.turnCount;

            
            LastTurn = pacManMovement.turnCount;
        }
    }

    public void OnPathFound1(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //Debug.Log("Distancia J-A: " + path.Length);
        }
    }

    public void OnPathFound2(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //Debug.Log("Distancia A-R: " + path.Length);
        }
    }

    public void OnPathFound3(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //Debug.Log("Distancia J-R: " + path.Length);
        }
    }
}
