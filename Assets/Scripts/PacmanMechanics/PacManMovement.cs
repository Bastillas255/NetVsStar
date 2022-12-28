using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    [HideInInspector]
    public bool isReady;
    [HideInInspector]
    public bool isMoving;
    public bool isHuman;

    Vector2 directionPressed;

    //Grid stuff
    private StarGrid grid;
    StarNode pacmanNode;
    StarNode allignCheck;
    Vector3 targetPosition;
    float lerpDuration = 1;

    //modules
    public int turnCount = 0;
    public Vector3 closestReward;

    //NN stuff
    public NeuralNetwork net;
    private bool initilized = false;
    private Transform hex; //transform where pac man get rewarded for pointing/closing in
    private Material mat;


    //Rewards Stuff
    private Vector3[] keySpots;
    public int rewardNumber;
    float minDistance= 1000f;
    public float fitness;
    private Vector3 door;
    int aux = 4;
    public Vector3[] path;

    float[] inputs = new float[13];

    GameObject[] keySpotAuxiliar;

    void Start()
    {
        //keySpots should be taken for rewards on the map
        keySpotAuxiliar= GameObject.FindGameObjectsWithTag("Consumable");
        keySpots = new Vector3[keySpotAuxiliar.Length];
        for (int i = 0; i < keySpotAuxiliar.Length; i++)
        {
            keySpots[i] = keySpotAuxiliar[i].transform.position;
        }

        door = new Vector3(11, 11, 0);
        //Initialize
        grid =GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        isMoving = false;
        mat = GetComponent<Renderer>().material;
        rewardNumber = 0;
        
        //alling itself to grid
        pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        transform.position = pacmanNode.worldPosition;//and center to it

        //tracking modules
        for (int i = 0; i < keySpots.Length; i++)
        {
            float Distance = Vector3.Distance(transform.position, keySpots[i]);
            if (Distance < minDistance)
            {
                minDistance = Distance;
                closestReward = keySpots[i];
            }
        }
        StartCoroutine("StartSearch", closestReward);
    }

    private void Update()
    {
        if (isHuman)
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

                    
                    
                    //update closestReward for traceModules
                    
                   
                    if (rewardNumber == 11)
                    {
                        closestReward = door;
                    }
                }
            }
        }
    }


    void FixedUpdate()
    {

        if (initilized==true)
        {
            fitness = net.GetFitness();
            if (fitness > 20f)
                fitness = 20f;
            mat.color = new Color(fitness/20, 1-(fitness/20), 1-(fitness/20));

            if (!isMoving)
            {
                //wall vision
                foreach (StarNode n in grid.GetNeighboursWithDiagonals(pacmanNode))
                {
                    inputs[aux] = (n.walkable) ? 0 : 1; //walkable put a 0 on inputs
                    aux++;

                    if (grid.playerNode == n)
                    {
                        inputs[2] = grid.playerNode.worldPosition.x;
                        inputs[3] = grid.playerNode.worldPosition.y;
                    }
                }

                //improve this, atleast on Human control
                //some things could be better like find a way to a* to find closest reward
                for (int i = 0; i < keySpots.Length; i++)
                {
                    StartCoroutine("StartSearch", keySpots[i]);
                    float distanceToKey = path.Length;
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
                    directionPressed.y = 0;
                }
                else
                {
                    directionPressed.y = Mathf.Sign(output[1]);
                    directionPressed.x = 0;
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
                //rb.MovePosition(new Vector3(transform.position.x + (directionPressed.x), transform.position.y + (directionPressed.y), transform.position.z));

                net.AddFitness((1f - Vector3.Distance(transform.position, hex.position)));//fitness based on how distant pac is from objectives
            }
            
        }
    }

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
            rewardNumber++;
            if (!isHuman)
            {
                net.AddFitness(100f);//fitness based on how distant pac is from objectives
            }
            //how to erase this horrid consumable from list

            for (int i = 0; i < keySpots.Length; i++)
            {
                if (Vector3.Distance(keySpots[i], other.transform.position) < 1)
                {
                    keySpots[i] = Vector3.zero;
                }
            }
            Destroy(other.gameObject);
           
            //Debug.Log("RewardsLeft = " + rewardsCount);
        }

        if (other.CompareTag("Door"))
        {
            if (rewardNumber==11)
            {
                if (!isHuman)
                {
                    net.AddFitness(1000f);//fitness based on how distant pac is from objectives
                }
                
                Debug.Log("Game Over");
                Time.timeScale = 0;//game finish at this point
            }
            //Debug.Log("RewardsLeft = " + rewardsCount);
        }
    }

    private IEnumerator TileMovement()
    {
        //lerp
        if (allignCheck.walkable)
        {
            if (true)// is not a diagonal
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
                //code to calculate closest reward
                minDistance = 1000f;
                for (int i = 0; i < keySpots.Length; i++)
                {
                    if (keySpots[i] != Vector3.zero)
                    {
                        float Distance = Vector3.Distance(transform.position, keySpots[i]);
                        if (Distance < minDistance)
                        {
                            minDistance = Distance;
                            closestReward = keySpots[i];
                        }
                    }
                }

                //Debug.Log("Closest Reward; " + closestReward);
                StartCoroutine("StartSearch", closestReward);
            }
            
        }
        else
        {
            isMoving = false;
            yield break;
        }
    }
    //search closest Path to Reward
    private void StartSearch(Vector3 target)
    {
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
        }
    }

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.hex = hex;
        this.net = net;
        initilized = true;
    }
}
