using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceModules : MonoBehaviour
{
    public GameObject pacObject;
    public GameObject ghostObject;

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
        pacManMovement = pacObject.GetComponent<PacManMovement>();
        unit = ghostObject.GetComponent<Unit>();
        trace = new SaveTraceData();
        sui = new SaveUserInput();
        fm = new FileManager();
        playerMovements = new int[4];
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

            //Debug.Log("Adversario: " + unit.transform.position);//Vector3
            trace.std_enemyXPos = Mathf.Round(unit.transform.position.x);
            trace.std_enemyYPos = Mathf.Round(unit.transform.position.y);

            //Debug.Log("Recompensa: "+ pacManMovement.closestReward);//Vector3
            trace.std_closestRewardXPos = Mathf.Round(pacManMovement.closestReward.x);
            trace.std_closestRewardYPos = Mathf.Round(pacManMovement.closestReward.y);

            //Utilizar variable path para obtener la distancia después de hacer su respectivo request
            //Distancia J-A
            PathRequestManager.RequestPath(pacManMovement.transform.position, ghostObject.transform.position, OnPathFound1);//int
            trace.std_distPlayerEnemy = path.Length;

            //Distancia A-R
            PathRequestManager.RequestPath(ghostObject.transform.position, pacManMovement.closestReward, OnPathFound2);//int
            trace.std_distEnemyReward = path.Length;

            //Distancia J-R
            PathRequestManager.RequestPath(pacManMovement.transform.position, pacManMovement.closestReward, OnPathFound3);//int
            trace.std_distPlayerReward = path.Length;

            //Debug.Log("RecObtenida: " + pacManMovement.rewardNumber);//int
            trace.std_rewardsObtained = pacManMovement.rewardNumber;

            //Debug.Log("Turno: "+ pacManMovement.turnCount);//int
            trace.std_turnCount = pacManMovement.turnCount;

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
