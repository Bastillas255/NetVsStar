using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceModules : MonoBehaviour
{
    private GameObject[] rewardObjects;
    private Vector3[] reward;



    public GameObject pacObject;
    PacManMovement pacManMovement;

    FileManager fm;
    public Vector3[] path;

    private SaveTraceData trace;
    private int LastTurnCount=-1;

    // Start is called before the first frame update
    void Start()
    {
        trace = new SaveTraceData();
        fm = new FileManager();

        pacManMovement = pacObject.GetComponent<PacManMovement>();
        rewardObjects = GameObject.FindGameObjectsWithTag("Consumable");
        reward =new Vector3[rewardObjects.Length];

        for (int i = 0; i < rewardObjects.Length; i++)
        {
            reward[i] = rewardObjects[i].transform.position;
        }
    }

    void LateUpdate()
    {
        if (pacManMovement.turnCount != LastTurnCount)
        {
            for (int i = 0; i < rewardObjects.Length; i++)
            {
                reward[i] = rewardObjects[i].transform.position;
            }

            trace.std_playerXPos = pacManMovement.transform.position.x;
            trace.std_playerYPos = pacManMovement.transform.position.y;

            trace.std_Reward1XPos = reward[0].x;
            trace.std_Reward1YPos = reward[0].y;

            trace.std_Reward2XPos = reward[1].x;
            trace.std_Reward2YPos = reward[1].y;

            trace.std_Reward3XPos = reward[2].x;
            trace.std_Reward3YPos = reward[2].y;

            trace.std_Reward4XPos = reward[3].x;
            trace.std_Reward4YPos = reward[3].y;

            fm.AddToFile("TraceData.txt", trace.ToJson());

            LastTurnCount = pacManMovement.turnCount;
        }
    }
}
