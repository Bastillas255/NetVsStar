using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCordinator : MonoBehaviour
{
    private GameObject[] targets;
    private Unit[] ghosts;

    public GameObject ghostPrefab;
    [SerializeField]
    private Transform ghostSpawn;
    private List<Unit> ghostList = null;


    private void OnEnable()
    {
        //this should be done every time the timer resets
        targets = GameObject.FindGameObjectsWithTag("Player"); //with this we have all the PopulationSize of pacs

    }

    // Update is called once per frame
    void Update()
    {
        //send one ghost after the first pac
        for (int i = 0; i < targets.Length; i++)
        {
            
        }

    }
    private void CreateGhostBodies()
    {
        if (ghostList != null)
        {
            for (int i = 0; i < ghostList.Count; i++)
            {
                GameObject.Destroy(ghostList[i].gameObject);
            }

        }

        ghostList = new List<Unit>();

        for (int i = 0; i < targets.Length; i++) //targets.Length was populationSize
        {
            Unit ghost = ((GameObject)Instantiate(ghostPrefab, transform.position, ghostPrefab.transform.rotation)).GetComponent<Unit>();

            //ghost.Chase(targets[i]);//we should give the corresponding pac transform
            ghostList.Add(ghost);
        }

    }

}
