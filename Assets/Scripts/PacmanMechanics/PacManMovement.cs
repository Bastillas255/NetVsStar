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
    private Vector2 objective; //direction the human player pressed


    //Rewards Stuff
    private Vector3[] keySpots;
    public int rewardNumber;
    float minDistance = 1000f;
    private Vector3 door;
    public Vector3[] path;

    float[] inputs = new float[11];
    float[] stdArray = new float[11];

    GameObject[] keySpotAuxiliar;

    SaveTraceData saveTraceData;
    FileManager fm;
    public bool displayGizmos;
    Manager manager;
    NeuralSensor ns;

    void Start()
    {
        //keySpots are taken from rewards positions on the map
        keySpotAuxiliar = GameObject.FindGameObjectsWithTag("Consumable");
        keySpots = new Vector3[keySpotAuxiliar.Length];
        for (int i = 0; i < keySpotAuxiliar.Length; i++)
        {
            keySpots[i] = keySpotAuxiliar[i].transform.position;
        }

        //basic assigments
        rewardNumber = 0;
        door = new Vector3(11, 11, 0);
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Manager>();
        ns = GetComponent<NeuralSensor>();
        saveTraceData = new SaveTraceData();
        fm = new FileManager();
        isMoving = false;

        //alling itself to grid
        pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        transform.position = pacmanNode.worldPosition;//and center to it

        //this is made so the tracingModules.cs can store the closest reward to pacman correctly. this same code is repeated aftear each move
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

    //trainNN() is called from Manager.cs and it repeats until it has train to the last turn
    public void TrainNN(float[] traceData)
    {
        while(true)
        {
            //NN analyses inputs and their outputs are stored
            float[] nnOutput = new float[11];
            nnOutput = net.FeedForward(traceData);

            directionPressed.x = nnOutput[0];
            directionPressed.y = nnOutput[1];

            //We check if the direction "pressed" by the NN it's equals to the user input in that turn
            Vector2 result = directionPressed - objective;

            //If the difference is minimal (nn chose the same direction) it adds to the fitness
            if(result.magnitude > 0.1f)
            {
                Debug.Log("Fitness correcto en turno: " + traceData[10]);
                net.AddFitness(1f);
                break;
            }
            else
            {
                //If other direction is chosen, the nn mutates and asks again
                Debug.Log("Mutando en turno: " + traceData[10]);
                net.Mutate();
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
                    isMoving = true;
                    turnCount++;

                    //move to the next tile
                    targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    targetPosition = allignCheck.worldPosition;
                    StartCoroutine("TileMovement");

                    if (rewardNumber == 11)
                    {
                        closestReward = door;
                    }
                }
            }
        }
    }


    //(W, I, P,) this will now be only execution, no changes to the NN should be made here, like fitness and stuff, manager should adress this
   //on this Update the is only pacman movement mechanics, but control is on the net

   void FixedUpdate()
    {
        if (initilized == true)
        {
            if (!isMoving)
            {
                turnCount++;
                if (rewardNumber == 11)
                {
                    closestReward = door;
                }
                isMoving = true;

                //add the inputs
                inputs = ns.traceModules;

                float[] output = net.FeedForward(inputs);//The information coming back from the NN, 

                //this outputs are the movement magnitudes the net calculates
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
                //we also need to check if that tile is walkable
                targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                allignCheck = grid.NodeFromWorldPoint(targetPosition);
                targetPosition = allignCheck.worldPosition;

                StartCoroutine("TileMovement");
            }
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

    //consume the rewards or grab de door collion detecction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Consumable"))
        {
            rewardNumber++;
            //Update closest reward
            for (int i = 0; i < keySpots.Length; i++)
            {
                if (Vector3.Distance(keySpots[i], other.transform.position) < 1)
                {
                    keySpots[i] = Vector3.zero;
                }
            }
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Door"))
        {
            if (rewardNumber == 11)
            {
                Debug.Log("Game Over");
                Time.timeScale = 0;//game finish at this point
            }
        }
    }


    //movement couroutine
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


            //code to calculate closest reward after movement is done
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
            //update path
            StartCoroutine("StartSearch", closestReward);

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

    //RequestPath automatically activetes this function on callback, it just assign the path
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
        }
    }

    
    //initialization called from manager.cs, it sets the net on this pacman and the first sets of inputs of the net
    public void Init(NeuralNetwork net, Vector2 objective/*, float[] stdArray*/)
    {
        this.objective = objective;
        this.net = net;
        //this.stdArray = stdArray;
        initilized = true; 
    }

    //setter for the next sets of inputs to feedfoward the net
    public void SetObjectiveAndInputs(Vector2 objective/*, float[] stdArray*/)
    {
        this.objective = objective;
        //this.stdArray = stdArray;
    }


}
