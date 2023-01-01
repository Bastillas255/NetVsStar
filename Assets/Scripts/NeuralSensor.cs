using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralSensor : MonoBehaviour
{
    PacManMovement pacManMovement;
    public GameObject[] rewardObjects;
    public Vector3[] reward;

    public Vector3[] path;
    
    public float[] traceModules = new float[14];
    private int LastTurnCount = -1;

    // Start is called before the first frame update
    void Start()
    {
        traceModules = new float[14];
        pacManMovement = GetComponent<PacManMovement>();

        rewardObjects = GameObject.FindGameObjectsWithTag("Consumable");
        reward = new Vector3[rewardObjects.Length];
        for (int i = 0; i < rewardObjects.Length; i++)
        {
            reward[i] = rewardObjects[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pacManMovement.turnCount != LastTurnCount)
        {
            for (int i = 0; i < rewardObjects.Length; i++)
            {
                reward[i] = rewardObjects[i].transform.position;
            }

            //Debug.Log("Jugador: "+ pacManMovement.transform.position);//Vector3
            traceModules[0] = pacManMovement.transform.position.x;
            traceModules[1] = pacManMovement.transform.position.y;

            //Debug.Log("Adversario: " + unit.transform.position);//Vector3
            traceModules[2] = reward[0].x;
            traceModules[3] = reward[0].y;

            //Debug.Log("Recompensa: "+ pacManMovement.closestReward);//Vector3
            traceModules[4] = reward[1].x;
            traceModules[5] = reward[1].y;

            traceModules[6] = reward[2].x;
            traceModules[7] = reward[2].y;

            traceModules[8] = reward[3].x;
            traceModules[9] = reward[3].y;

            if (reward[0].y == 1000)
            {
                traceModules[10] = 1;
            }
            if (reward[1].y == 1000)
            {
                traceModules[11] = 1;
            }
            if (reward[2].y == 1000)
            {
                traceModules[12] = 1;
            }
            if (reward[3].y == 1000)
            {
                traceModules[13] = 1;
            }

            LastTurnCount = pacManMovement.turnCount;
        }
    }
}
