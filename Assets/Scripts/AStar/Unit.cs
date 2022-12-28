using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Transform target;
    PacManMovement pac;
    public Vector3[] path;
    int targetIndex;
    int lastTurn;
    public bool displayPathGizmos;
    float lerpDuration = 1;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        pac=target.GetComponent<PacManMovement>();
        lastTurn = 0;

        //tracking modules
        PathRequestManager.RequestPath(transform.position, target.position, doNothing);//move one position closer
    }

    void doNothing(Vector3[] newPath, bool pathSuccessful)
    {

    }

    public void Chase(Transform pacman)
    {
        target = pacman;
        //formerLoopPosition = Vector3.zero;
    }

    private void Update()
    {
        if (pac.turnCount!=lastTurn)//if turn has changed
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);//move one position closer
            lastTurn = pac.turnCount;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StartCoroutine("TileMovement");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over: Ghost touch");
            Time.timeScale = 0;//game finish at this point
            //other.GetComponent<PacManMovement>().net.AddFitness(-1000f);
        }
    }

    public void OnDrawGizmos()
    {
        if (displayPathGizmos)
        {
            if (path != null)
            {

                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], Vector3.one);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }
        
    }

    private IEnumerator TileMovement()
    {
        //lerp
        if (true)// is not a diagonal
        {
            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                transform.position = Vector3.Lerp(transform.position, path[0], timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = path[0];
        }
    }
}
