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
    }

	void Update () {

        if (!manager.gameOver && canMove)
        {
            if (Input.GetButton("Horizontal"))
            {
                moveHorizontally(Input.GetAxisRaw("Horizontal"));
            }

            if (Input.GetButtonDown("Vertical"))
            {
                moveVertically(Mathf.RoundToInt(Input.GetAxisRaw("Vertical")));
            }
        }


	}

    void obtainLimits()
    {
        minPos = manager.minPos;
        maxPos = manager.maxPos;
    }

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

    public void moveHorizontally(float Ax)
    {
        if ((Ax > 0 && canMoveToRight) || (Ax < 0 && canMoveToLeft))
        {
            HorizontalD = XVelocity * Time.deltaTime * Input.GetAxisRaw("Horizontal");
            transform.Translate(new Vector3(HorizontalD, 0, 0));
        }
    }

    public void moveVertically(int dir)
    {
        if (((dir < 0 && canMoveDown && lane + dir >= 0) || (dir > 0 && canMoveUp && lane + dir < numberOfLanes)) /*&& (lane + dir >= 0 && lane + dir <= numberOfLanes - 1)*/)
        {
            lane += dir;

            VerticalD = distanceBetweenLanes * dir;

            transform.Translate(new Vector3(0, 0, VerticalD));
        }
    }
}
