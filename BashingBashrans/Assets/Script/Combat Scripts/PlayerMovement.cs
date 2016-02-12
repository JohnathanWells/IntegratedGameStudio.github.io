using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float XVelocity = 2f;
    public Transform playerModel;
    //public Animation animation;
    //public float YVelocity = 1f;
    //public bool squareMovementX = false;
    //public bool squareMovementY = false;

    private float VerticalD;
    private float HorizontalD;
    private float distanceBetweenLanes = 0;
    private int lane = 0;
    private int numberOfLanes = 2;
    private bool canMove = true;
    private Vector2 minPos;
    private Vector2 maxPos;
    private bool canMoveToRight = true;
    private bool canMoveToLeft = true;
    private bool canMoveUp = true;
    private bool canMoveDown = true;

    public GameManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        obtainLimits();
        numberOfLanes = manager.numberOfLanes;
        lane = manager.obtainLane(transform);
        distanceBetweenLanes = manager.distanceBetweenLanes;
        //Debug.Log(lane);
        //Debug.Log(minPos + "\n" + maxPos);
    }

	void Update () {

        if (!manager.gameOver && canMove)
        {
            float Ax = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Horizontal") && ((Ax > 0 && canMoveToRight) || (Ax < 0 && canMoveToLeft)))
            {
                //changeFacingDirection(Ax);
                //animation.Play("Take 001");

                HorizontalD = XVelocity * Time.deltaTime * Input.GetAxisRaw("Horizontal");

                //if (transform.position.x + HorizontalD < minPos.x || transform.position.x + HorizontalD > maxPos.x)
                //    HorizontalD = 0;

                transform.Translate(new Vector3(HorizontalD, 0, 0));
            }

#region OldCode
            //else if (Input.GetButtonDown("Horizontal") && squareMovementX)
            //{
            //    HorizontalD = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

            //    if (transform.position.x + HorizontalD < minPos.x)
            //        HorizontalD = 0;
            //    else if (transform.position.x + HorizontalD > maxPos.x)
            //        HorizontalD = 0;

            //    transform.Translate(new Vector3(HorizontalD, 0, 0));
            //    Mathf.RoundToInt(transform.position.x);
            //}


            //if (!squareMovementY)
            //{
            //    VerticalD = YVelocity * Time.deltaTime * Input.GetAxisRaw("Vertical");

            //    if (transform.position.y + VerticalD < minPos.y)
            //        VerticalD = 0;
            //    else if (transform.position.y + VerticalD > maxPos.y)
            //        VerticalD = 0;

            //    transform.Translate(new Vector3(0, VerticalD, VerticalD));
            //}
            //else 
#endregion

            if (Input.GetButtonDown("Vertical"))
            {
                int dir = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

                if (((dir < 0 && canMoveDown && lane + dir >= 0) || (dir > 0 && canMoveUp && lane + dir < numberOfLanes)) /*&& (lane + dir >= 0 && lane + dir <= numberOfLanes - 1)*/)
                {
                    lane += dir;

                    VerticalD = distanceBetweenLanes * dir;

                    transform.Translate(new Vector3(0, 0, VerticalD));
                }
            }
        }


	}

    void obtainLimits()
    {
        minPos = manager.minPos;
        maxPos = manager.maxPos;
    }

    //void changeFacingDirection(float dir)
    //{
    //    if (dir > 0)
    //    {
    //        playerModel.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
    //    }
    //    else if (dir < 0)
    //    {
    //        playerModel.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    //    }
    //}

    public void receiveDetection(Vector2 direction, bool value)
    {
        if (direction.x == 1 && direction.y == 0)
        {
            canMoveToRight = value;
        }
        else if (direction.x == -1 && direction.y == 0)
        {
            canMoveToLeft = value;
        }
        else if (direction.x == 0 && direction.y == 1)
        {
            canMoveUp = value;
        }
        else if (direction.x == 0 && direction.y == -1)
        {
            canMoveDown = value;
        }
    }

    public void changeCanMove(bool val)
    {
        canMove = val;
    }

}
