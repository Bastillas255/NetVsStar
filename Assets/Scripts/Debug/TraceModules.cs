using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceModules : MonoBehaviour
{
    public GameObject pacObject;
    public GameObject ghostObject;
    public GameObject[] rewardObjects;
    public Vector3[] reward;

    private int[] playerMovements;

    PacManMovement pacManMovement;
    Unit unit;

    FileManager fm;
    SaveUserInput sui;

    int LastTurn=-1;
    public Vector3[] path;

    private SaveTraceData trace;

    // Start is called before the first frame update
    void Start()
    {
        trace = new SaveTraceData();
        sui = new SaveUserInput();
        fm = new FileManager();
        playerMovements = new int[4];
        rewardObjects = GameObject.FindGameObjectsWithTag("Consumable");

        for (int i = 0; i < rewardObjects.Length; i++)
        {
            reward[i] = rewardObjects[i].transform.position;
        }
    }

    private void RestartPlayerMovements()
    {
        for(int i = 0; i < playerMovements.Length; i++)
        {
            playerMovements[i] = 0;
        }
    }

    void LateUpdate()
    {
        if (pacManMovement.turnCount != LastTurn)
        {
            //Debug.Log("Jugador: "+ pacManMovement.transform.position);//Vector3
            trace.std_playerXPos = Mathf.Round(pacManMovement.transform.position.x);
            trace.std_playerYPos = Mathf.Round(pacManMovement.transform.position.y);

            //Debug.Log("Recompensa: "+ pacManMovement.closestReward);//Vector3


            //variables in SaveTraceData needs to be changed
            trace.std_Reward1XPos = Mathf.Round(reward[0].x);
            trace.std_Reward1YPos = Mathf.Round(reward[0].y);

            trace.std_Reward1XPos = Mathf.Round(reward[1].x);
            trace.std_Reward1YPos = Mathf.Round(reward[1].y);

            trace.std_Reward1XPos = Mathf.Round(reward[2].x);
            trace.std_Reward1YPos = Mathf.Round(reward[2].y);

            trace.std_Reward1XPos = Mathf.Round(reward[3].x);
            trace.std_Reward1YPos = Mathf.Round(reward[3].y);

            fm.AddToFile("TraceData.txt", trace.ToJson());

            LastTurn = pacManMovement.turnCount;

            RestartPlayerMovements();
            if(Input.GetKeyDown(KeyCode.UpArrow)) playerMovements[0] = 1;
            if(Input.GetKeyDown(KeyCode.DownArrow)) playerMovements[1] = 1;
            if(Input.GetKeyDown(KeyCode.LeftArrow)) playerMovements[2] = 1;
            if(Input.GetKeyDown(KeyCode.RightArrow)) playerMovements[3] = 1;

            sui.up = playerMovements[0];
            sui.down = playerMovements[1];
            sui.left = playerMovements[2];
            sui.right = playerMovements[3];

            fm.AddToFile("UserInputs.txt", sui.ToJson());
        }
    }
}
