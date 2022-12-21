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
    int rewardsCount=11;
    int turnCount = 0;

    //NN stuff
    private NeuralNetwork net;
    private bool initilized = false;
    private Transform hex; //transform where pac man get rewarded for pointing/closing in
    private Material mat;

    private int currentReward; //should we go one by one? wouldn't be better to have all the rewards positions on inputs? fitness calculate on most close one



    void Start()
    {
        //Initialize
        grid=GameObject.FindGameObjectWithTag("Grid").GetComponent<StarGrid>();
        rb = GetComponent<Rigidbody>();
        isMoving = false;
        mat = GetComponent<Renderer>().material;


        //alling itself to grid
        //pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        //transform.position = pacmanNode.worldPosition;//and center to it
    }


    void FixedUpdate()
    {
        if (initilized==true)
        {
            float distance = Vector2.Distance(transform.position, hex.position);
            if (distance > 20f)
                distance = 20f;
            mat.color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));
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
                //rewards(multiple hexagons) and finish door

                //turn movement

                //A* per net 

                if (!isMoving)
                {
                    isMoving = true;

                    float[] inputs = new float[2];

                    //Where the NN should go to get close to hex
                    inputs[0] = hex.position.x - transform.position.x;
                    inputs[1] = hex.position.y - transform.position.y;

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
                    targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    targetPosition = allignCheck.worldPosition;
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
