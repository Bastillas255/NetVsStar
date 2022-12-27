using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    Transform target;
    public bool isPacmanMoving;
    float speed = 10f; //1.55f og
    public Vector3[] path;
    int targetIndex;
    Vector3 formerLoopPosition;


    public void Chase(Transform pacman)
    {
        target = pacman;
        //formerLoopPosition = Vector3.zero;
    }

    private void Update()
    {
        if (target != null)
        {
            //Unit only request path if target has moved
            //if (target.position != formerLoopPosition)
            //{
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            //}
            //formerLoopPosition = target.position;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        //Debug.Log("Adversario = " + transform.position);
        //Debug.Log("Distancia J-A = " + path.Length);
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position==currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PacManMovement>().net.AddFitness(-1000f);
        }
    }

    public void OnDrawGizmos()
    {
        if (path!=null)
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
