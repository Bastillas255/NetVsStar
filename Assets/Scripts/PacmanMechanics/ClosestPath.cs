using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPath : MonoBehaviour
{
    public GameObject[] targets;
    Vector3[] path;
    Vector3[] auxPath;
    float minDistance;
    
    public bool isDone;
    public Vector3 closestReward;


    public Vector3 SearchPaths()
    {
        targets = GameObject.FindGameObjectsWithTag("Consumable");
        //let's try with Vector3.distance
        for (int i = 0; i < targets.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestReward = targets[i].transform.position;
            }
        }
        return closestReward;

        //for (int i = 0; i < targets.Length; i++)
        //{
        //    PathRequestManager.RequestPath(transform.position, targets[i].transform.position, MinimumPath);
        //    //this lines give problems, becouse is not done when the request path is called
        //}
        //if (auxPath!=null)
        //{
        //    return closestReward = auxPath[auxPath.Length - 1];
        //}
    }

    //how to know we went throug all the targets?

    public void MinimumPath(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            if (auxPath==null)
            {
                auxPath = path;
            }
            if (path.Length< auxPath.Length)
            {
                auxPath = path;
            }
        }
    }
}
