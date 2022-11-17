using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    public bool isReady;
    public bool isMoving;

    Rigidbody rb;
    Vector2 directionPressed;
    //need to import StarGrid, take the neigbours list, and move to the neighbour in the pressed direction
    StarGrid grid;
    int distanceToMove;
    StarNode pacmanNode;
    StarNode allignCheck;
    Vector3 targetPosition;
    float lerpDuration = 1;

    void Start()
    {
        //initialize
        grid = GetComponentInParent<StarGrid>();
        rb = GetComponent<Rigidbody>();
        isMoving = false;

        //alling itself to grid
        pacmanNode = grid.NodeFromWorldPoint(transform.position);//in which square of the grid I'm in
        transform.position = pacmanNode.worldPosition;//and center to it
    }


    void Update()
    {
        //if (!isMoving)
        //{
            directionPressed.x = Input.GetAxisRaw("Horizontal");
            directionPressed.y = Input.GetAxisRaw("Vertical");
            Debug.Log(directionPressed);
            if (directionPressed.magnitude!=0)
            {
            //pacman needs to move in grid diameter distances; next line is an example
            //Debug.DrawLine(transform.position, new Vector3(transform.position.x+ ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius*2)* directionPressed.y), transform.position.z), Color.magenta);

            //doesn't take into account collisions
            //rb.MovePosition(new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z));
            //rb.AddForce(new Vector3(((grid.nodeRadius * 2) * directionPressed.x), ((grid.nodeRadius * 2) * directionPressed.y), 0));
                if (!isMoving)
                {
                    isMoving = true;
                    //move to the next tile
                    targetPosition = new Vector3(transform.position.x + ((grid.nodeRadius * 2) * directionPressed.x), transform.position.y + ((grid.nodeRadius * 2) * directionPressed.y), transform.position.z);
                    allignCheck = grid.NodeFromWorldPoint(targetPosition);
                    targetPosition = allignCheck.worldPosition;
                    StartCoroutine("TileMovement");
                }
                
            }
        //}
    }

    private void OnDrawGizmos()
    {
        if (pacmanNode!=null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, Vector3.one * ((grid.nodeRadius * 2) - .1f));
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
    //if (collisionCheck.walkable)
    //{
    //    rb.MovePosition(targetPosition);
    //}
    //yield return new WaitForSeconds(1);
}
