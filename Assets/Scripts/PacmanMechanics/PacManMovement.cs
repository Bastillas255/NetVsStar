using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    //something in here makes unity go boom, fix time
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
    //float lerpDuration = 1;

    //modules
    public int turnCount = 0;
    public Vector3 closestReward;

    //NN stuff
    public NeuralNetwork net;
    private bool initilized = false;

    //Rewards Stuff
    public int rewardNumber;
    public Vector3[] path;
    private int targetIndex;
    public Vector3[] rewardSpots;

    float[] inputs=new float[14];

    public bool displayGizmos;
    NeuralSensor ns;
    float[] nnOutput = new float[4];

    private float lerpDuration=0.5f;
    int biggestResultIndex;

    void Start()
    {
        //basic assigments
        rewardNumber = 0;
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        ns = GetComponent<NeuralSensor>();
        isMoving = false;

        //alling itself to grid
        pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        transform.position = pacmanNode.worldPosition;//and center to it        
    }

    //trainNN() is called from Manager.cs and it repeats until it has train to the last turn
    public void TrainNN(float[] traceData)
    {
        if (initilized == true)
        {
            while (true)
            {
                //NN analyses inputs and their outputs are stored
                nnOutput = net.FeedForward(traceData); //4 outputs, they return one of the rewards on the next to be made array

                //array of rewards
                rewardSpots= new Vector3[4];
                int aux = 0;
                for (int i = 2; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        rewardSpots[aux].x = traceData[i];
                        
                    }
                    else
                    {
                        rewardSpots[aux].y = traceData[i];
                        rewardSpots[aux].z = 1f;
                        aux++;
                    }
                }

                //we need to know which reward is the closest to NN
                float minDistance = Vector3.Distance(transform.position, rewardSpots[0]);
                for (int i = 0; i < rewardSpots.Length; i++)
                {
                    float Distance = Vector3.Distance(transform.position, rewardSpots[i]);
                    if (Distance < minDistance)
                    {
                        minDistance = Distance;
                        closestReward = rewardSpots[i];
                    }
                }
                //code adove is using vector3.distance instead of checking path lenght, the exact distance can only be found with the Coroutine bellow
                //StartCoroutine("UpdatePath", closestReward);

                //closestReward is on our hands and the 4 ouputs, now we can compare them
                float aux2= nnOutput[0];
                biggestResultIndex=0;
                for (int i = 0; i < nnOutput.Length; i++)
                {
                    if (nnOutput[i] > aux2)
                    {
                        aux2 = nnOutput[i];
                        biggestResultIndex=i;
                    }
                }

                //If rewardSpot selected is also the closestReward we can go to the next turn
                if (rewardSpots[biggestResultIndex]==closestReward)
                {
                    break;
                }
                else
                {
                    //If other reward is chosen, the nn mutates and asks again
                    net.Mutate();
                }
            }
        }
    }

    //on this Update the is only pacman movement mechanics, but control is on the player
    private void Update()
    {
        if (isHuman)
        {
            directionPressed.x = Input.GetAxisRaw("Horizontal");
            directionPressed.y = Input.GetAxisRaw("Vertical");
            if (directionPressed.magnitude != 0)
            {
                if (!isMoving)
                {
                    //move to the next tile
                    targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    targetPosition = allignCheck.worldPosition;

                    if (allignCheck.walkable)
                    {
                        turnCount++;
                        StartCoroutine("TileMovement");
                        isMoving = true;
                        
                    }
                }
            }
        }
    }


   //execution of NN
   //this Update is only pacman movement mechanics, but control is on the net
   void FixedUpdate()
   {
        if (initilized == true)
        {
            if (!isMoving)
            {
                //add the inputs
                
                inputs = ns.traceModules;

                for (int i = 0; i < 10; i++)
                {
                    inputs[i] = ns.traceModules[i];
                }
                


                float[] output = net.FeedForward(inputs);//The information coming back from the NN is stored on "output" array

                rewardSpots = new Vector3[4];
                int aux = 0;
                for (int i = 2; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        rewardSpots[aux].x = inputs[i];

                    }
                    else
                    {
                        rewardSpots[aux].y = inputs[i];
                        rewardSpots[aux].z = 0f;
                        aux++;
                    }
                }

                //code adove is using vector3.distance instead of checking path lenght, the exact distance can only be found with the Coroutine bellow
                //StartCoroutine("UpdatePath", closestReward);

                //closestReward is on our hands and the 4 ouputs, now we can compare them
                float aux2 = nnOutput[0];
                int biggestResultIndex = 0;
                for (int i = 0; i < nnOutput.Length; i++)
                {
                    if (nnOutput[i] > aux2)
                    {
                        aux2 = nnOutput[i];
                        biggestResultIndex = i;
                    }
                }


                //now we need to move
                
                //isMoving = true;
                //turnCount++;
                Debug.Log("rewardSpot selected: " + rewardSpots[biggestResultIndex]);
                StartCoroutine("UpdatePath", rewardSpots[biggestResultIndex]);
            }
        }
    }

    //search closest Path to Reward
    private void UpdatePath(Vector3 target)
    {
        PathRequestManager.RequestPath(transform.position, target, OnPathFound2);
    }

    public void OnPathFound2(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //targetIndex = 0;
            //neural sensors are also to blame, the are 1 turn behind, we should just take all that that in fixed update

            //we don't want to step on a path.length 0
            //when we are 1 node away in grid we should not use path, sometimes path.length becomes 0 an we get a out of index error
            Vector3 aux;
            turnCount++;
            if (path.Length==0)
            {
                Debug.Log("path with 0 lenght?");
                //move to closest reward, wich can be known with vector3 distance
                int aux4 = 0;
                for (int i = 2; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        rewardSpots[aux4].x = ns.traceModules[i];

                    }
                    else
                    {
                        rewardSpots[aux4].y = ns.traceModules[i];
                        rewardSpots[aux4].z = 1f;
                        aux4++;
                    }
                }

                //we need to know which reward is the closest to NN
                float minDistance = Vector3.Distance(transform.position, rewardSpots[0]);
                for (int i = 0; i < rewardSpots.Length; i++)
                {
                    float Distance = Vector3.Distance(transform.position, rewardSpots[i]);
                    if (Distance < minDistance)
                    {
                        minDistance = Distance;
                        closestReward = rewardSpots[i];
                    }
                }

                //aux = rewardSpots[biggestResultIndex];
                StopCoroutine("TileMovementNet");
                StartCoroutine("TileMovementNet", closestReward);
            }
            else
            {
                aux = path[0];
                //let's just give TileMovementNet
                StopCoroutine("TileMovementNet");
                StartCoroutine("TileMovementNet", aux); //aux here in case of uncomment
            }
        }
    }

    private IEnumerator TileMovementNet(Vector3 targetPosition)//og is only "vector3 targetPosition"
    {
        //lerp
        //Vector3 currentWaypoint = path[0];
        //while (true)
        //{
        //    if (transform.position == currentWaypoint)
        //    {
        //        targetIndex++;
        //        if (targetIndex >= path.Length)
        //        {
        //            yield break;
        //        }
        //        currentWaypoint = path[targetIndex];
        //    }

        //    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, 5f * Time.deltaTime);
        //    yield return null;

        //}
        //og tileMovementNet

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPosition;
        //turnCount ++;
        isMoving = false;
    }


    //RequestPath automatically activetes this function on callback, it just assign the path
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
        }
    }



    private IEnumerator TileMovement()
    {

        if (allignCheck.walkable)
        {
            //movement is using lerp 
            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            isMoving = false;
            //lerp done
        }
        else
        {
            isMoving = false;
            yield break;
        }
    }

    //self explanatory
    private void OnDrawGizmos()
    {
        if (displayGizmos)
        {
            if (pacmanNode != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.position, Vector3.one * ((grid.nodeRadius * 2) - .1f));
            }
        }
    }

    //consume the rewards collision detecction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Consumable"))
        {
            Debug.Log("collisiono con; "+other.name);
            //Destroy(other.gameObject);
            //instead of destroying the reward, it will get teleported away
            other.gameObject.transform.position = Vector3.up*1000;
        }
    }


    //initialization called from manager.cs, it sets the net on this pacman and the first sets of inputs of the net
    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true; 
    }
}
