using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    public bool isReady;
    public bool isMoving;
    public bool isHumanControlled;

    Rigidbody rb;
    Vector2 directionPressed;
    //need to import StarGrid, take the neigbours list, and move to the neighbour in the pressed direction

    //need to get grid object on scene and then extract starGrid.cs
    private StarGrid grid;
    int distanceToMove;
    StarNode pacmanNode;
    StarNode allignCheck;
    Vector3 targetPosition;
    float lerpDuration = 1;

    //modules
    int rewardsCount = 11;
    int turnCount = 0;
    ClosestPath cPath;
    Vector3 closestReward;

    //NN stuff
    private NeuralNetwork net;
    private bool initilized = false;
    private Transform hex; //transform where pac man get rewarded for pointing/closing in
    private Material mat;


    //Rewards Stuff
    private Vector3[] keySpots;//KS has been filled, how to know which one has been visit?
    //we can make a star grid call? if we are one the same spot of one of the rewards? go through the list if movement has finish
    [SerializeField]
    private int rewardNumber;
    float minDistance;
    public float fitness;
    private Vector3 door;





    void Start()
    {
        //we need the keySpots vectors numbers, but alligned to grid
        keySpots = new Vector3[11] 
           {new Vector3(0,-7,0), 
            new Vector3(6,9,0), 
            new Vector3(-4,17,0), 
            new Vector3(-12,5,0), 
            new Vector3(-12,-5,0), 
            new Vector3(-28,5,0), 
            new Vector3(-20,5,0), 
            new Vector3(-20,15,0), 
            new Vector3(-30,-1,0), 
            new Vector3(-32,11,0), 
            new Vector3(0,3,0)};

        door = new Vector3(11, 11, 0);
        //Initialize
        grid =GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        rb = GetComponent<Rigidbody>();
        cPath = GetComponent<ClosestPath>();
        isMoving = false;
        mat = GetComponent<Renderer>().material;
        rewardNumber = 0;


        
        //put all rewards into keySpots

        //alling itself to grid
        //pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        //transform.position = pacmanNode.worldPosition;//and center to it
    }


    void FixedUpdate()
    {

        if (initilized==true)
        {
            fitness = net.GetFitness();
            //CreateRewards();
            if (fitness > 20f)
                fitness = 20f;
            mat.color = new Color(fitness/20, 1-(fitness/20), 1-(fitness/20));
            if (isHumanControlled)
            {
                directionPressed.x = Input.GetAxisRaw("Horizontal");
                directionPressed.y = Input.GetAxisRaw("Vertical");
                //Debug.Log(directionPressed);
                if (directionPressed.magnitude != 0)
                {
                    //pacman needs to move in grid diameter distances; next line is an example
                    //Debug.DrawLine(transform.position, new Vector3(transform.position.x+ ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius*2)* directionPressed.y), transform.position.z), Color.magenta);

                    //doesn't take into account collisions
                    //rb.MovePosition(new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z));
                    //rb.AddForce(new Vector3(((grid.nodeRadius * 2) * directionPressed.x), ((grid.nodeRadius * 2) * directionPressed.y), 0));
                    if (!isMoving)
                    {
                        isMoving = true;
                        //modules
                        //Debug.Log("Player (X,Y) = " + pacmanNode.worldPosition);
                        turnCount++;
                        //Debug.Log("Turn = " + turnCount);

                        //move to the next tile
                        targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                        allignCheck = grid.NodeFromWorldPoint(targetPosition);
                        targetPosition = allignCheck.worldPosition;
                        StartCoroutine("TileMovement");
                    }
                }
            }
            else
            {
                //some things could be better like find a way to a* to find closest reward,also fix bugs of going out of bounds
                //also the color of fitness
                //finish door

                //A* per net 

                if (!isMoving)
                {
                    for (int i = 0; i < keySpots.Length; i++)
                    {
                        float distanceToKey = Vector3.Distance(transform.position, keySpots[i]);
                        if (distanceToKey < minDistance)
                        {
                            minDistance = distanceToKey;
                            closestReward = keySpots[i];
                        }
                    }

                    if (rewardNumber == 11)
                    {
                        closestReward = door;
                    }
                    isMoving = true;

                    float[] inputs = new float[2];

                    //closestReward= cPath.SearchPaths(); //this is handled on the previous "for"

                    //NN needs to get close to ClosestReward

                    inputs[0] = closestReward.x - transform.position.x;
                    inputs[1] = closestReward.y - transform.position.y;

                    float[] output = net.FeedForward(inputs);//The information coming back from the NN, 

                    //this outpus are the movement magnitudes the net calculates
                    directionPressed.x = output[0];
                    directionPressed.y = output[1];


                    //but our movement is only ortgonal so we have to choose to move on x or y
                    if (Mathf.Abs(directionPressed.x) > Mathf.Abs(directionPressed.y))//which direction has more magnitude
                    {
                        directionPressed.x = Mathf.Sign(output[0]);
                    }
                    else
                    {
                        directionPressed.y = Mathf.Sign(output[1]);
                    }
                    //now we know where we are going we need to move in tiles

                    //we also need to check if that tile is walkable and if that tile is a keySpot

                    targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    targetPosition = allignCheck.worldPosition;
                    for (int i = 0; i < keySpots.Length; i++)
                    {
                        if (targetPosition ==keySpots[i])
                        {
                            Debug.Log("keySpot Collected At; "+ keySpots[i]);
                            rewardNumber++;
                            net.AddFitness(100f);//fitness based on how distant pac is from objectives
                            //erase from vector would be a better solution
                            if (i != keySpots.Length - 1)
                                keySpots[i] = keySpots[i + 1];
                            else
                                keySpots[i] = keySpots[i - 1];
                        }
                    }
                    
                    StartCoroutine("TileMovement");


                    //targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    //allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    //targetPosition = allignCheck.worldPosition;
                    rb.MovePosition(new Vector3(transform.position.x + (directionPressed.x), transform.position.y + (directionPressed.y), transform.position.z));

                    net.AddFitness((1f - Vector3.Distance(transform.position, hex.position)));//fitness based on how distant pac is from objectives
                }
            }
        }
    }

    //we created the rewards on intitialization of pacman bodies

    //needs a fix, instead of rewards this should be a list of positions, once a position is reached is also cleaned off the list

    //private void CreateRewards()
    //{
    //    if (rewardList != null)
    //    {
    //        for (int i = 0; i < rewardList.Count; i++)
    //        {
    //            GameObject.Destroy(rewardList[i].gameObject);
    //        }
    //    }

    //    rewardList = new List<GameObject>();

    //    for (int i = 0; i < rewardNumber; i++)
    //    {
    //        GameObject reward = ((GameObject)Instantiate(rewardPrefab, rewardSpawns[i].position, rewardPrefab.transform.rotation));
    //        rewardList.Add(reward);
    //    }
    //}

    private void OnDrawGizmos()
    {
        if (pacmanNode!=null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, Vector3.one * ((grid.nodeRadius * 2) - .1f));
        }
        
    }

    //consume the rewards
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Consumable"))
        {
            Destroy(other.gameObject);
            rewardsCount--;
            net.AddFitness(100f);//fitness based on how distant pac is from objectives
            //Debug.Log("RewardsLeft = " + rewardsCount);
        }

        if (other.CompareTag("Door"))
        {
            if (rewardNumber==11)
            {
                net.AddFitness(1000f);//fitness based on how distant pac is from objectives
                Time.timeScale = 0;
            }
            //Debug.Log("RewardsLeft = " + rewardsCount);
        }
    }

    private IEnumerator TileMovement()
    {
        //lerp
        if (allignCheck.walkable)
        {
            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            isMoving = false;
        }
        else
        {
            isMoving = false;
            yield break;
        }
    }

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.hex = hex;
        this.net = net;
        initilized = true;
    }
}
