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
    SaveUserInput sui;

    public Vector3[] path;

    private SaveTraceData trace;
    private int LastTurnCount=-1;

    // Start is called before the first frame update
    void Start()
    {
        trace = new SaveTraceData();
        sui = new SaveUserInput();
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

            if (reward[0].y==1000)
            {
                trace.std_Reward1Grabed = 1;
            }
            if (reward[1].y == 1000)
            {
                trace.std_Reward2Grabed = 1;
            }
            if (reward[2].y == 1000)
            {
                trace.std_Reward3Grabed = 1;
            }
            if (reward[3].y == 1000)
            {
                trace.std_Reward4Grabed = 1;
            }

            fm.AddToFile("TraceData.txt", trace.ToJson());

            LastTurnCount = pacManMovement.turnCount;

            float[] rewardSelection = new float[4];
            Vector3[] rewardsVectorPosition = new Vector3[4];

            rewardsVectorPosition[0] = new Vector3(trace.std_Reward1XPos, trace.std_Reward1YPos, 0f);
            rewardsVectorPosition[1] = new Vector3(trace.std_Reward2XPos, trace.std_Reward2YPos, 0f);
            rewardsVectorPosition[2] = new Vector3(trace.std_Reward3XPos, trace.std_Reward3YPos, 0f);
            rewardsVectorPosition[3] = new Vector3(trace.std_Reward4XPos, trace.std_Reward4YPos, 0f);

            float minDistance = 1000f; //Vector3.Distance(new Vector3(std_playerXPos, std_playerYPos), /*rewardSpots[0]*/);
            for (int i = 0; i < rewardsVectorPosition.Length; i++)
            {
                float distance = Vector3.Distance(pacManMovement.transform.position, rewardsVectorPosition[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
                rewardSelection[i] = distance;
            }

            for(int i = 0; i < rewardSelection.Length; i++)
            {
                if(rewardSelection[i] == minDistance)
                {
                    rewardSelection[i] = 1f;
                }
                else
                {
                    rewardSelection[i] = 0f;
                }
            }

            sui.rewardSelection = rewardSelection;

            fm.AddToFile("ClosestRewardEveryTurn.txt", sui.ToJson());
        }
    }

    // public void CalculateDistance()
    // {
    //     Vector3[] rewardsVectorPosition = new Vector3()[4];
    //     Vector3 playerVectorPosition = new Vector3(std_playerXPos, std_playerYPos, 0f);

    //     rewardsVectorPosition[0] = new Vector3(std_Reward1XPos, std_Reward1YPos, 0f);
    //     rewardsVectorPosition[1] = new Vector3(std_Reward2XPos, std_Reward2YPos, 0f);
    //     rewardsVectorPosition[2] = new Vector3(std_Reward3XPos, std_Reward3YPos, 0f);
    //     rewardsVectorPosition[3] = new Vector3(std_Reward4XPos, std_Reward4YPos, 0f);

    //     //minDistance = 100f; //Vector3.Distance(new Vector3(std_playerXPos, std_playerYPos), /*rewardSpots[0]*/);
    //     for (int i = 0; i < rewardsVectorPosition.Length; i++)
    //     {
    //         float distance = Vector3.Distance(playerVectorPosition, rewardsVectorPosition[i]);
    //         if (distance < minDistance)
    //         {
    //             // minDistance = distance;
    //             // closestReward = new Vector3(rewardsVectorPosition[i]);
                
    //         }
    //     }
    // }
}
