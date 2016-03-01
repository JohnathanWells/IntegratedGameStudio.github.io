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

    public CombatScript combatScript;

    public GameManager manager;

    [Header("Transition")]
    public bool coolTransition = true;
    private Vector3 pointTowards;
    private Quaternion angleTowards;
    private bool inTransition = false;
    private int currentRoom;

    [Header("Animation Things")]
    public Animator PlayerAnimator;
    //PlayerAnimator.SetBool("Walking", true);
    void Start()
    {
        setManager();
    }

	void Update () {

        if (inTransition)
        {
            transitionMove();
        }
        else if (!manager.gameOver && canMove && !inTransition)
        {
            if (Input.GetButton("Horizontal"))
            {
               
                moveHorizontally(Input.GetAxisRaw("Horizontal"));
            }

            if (Input.GetButtonDown("Vertical"))
            {
                moveVertically(Mathf.RoundToInt(Input.GetAxisRaw("Vertical")));
            }
            if (Input.GetButtonUp("Horizontal"))
            {

                PlayerAnimator.SetBool("Walking", false);
            }

        }
	}

    void transitionMove()
    {
        Time.timeScale = 1f;
        if (coolTransition)
        {
            float step = Time.deltaTime * XVelocity;
            transform.position = Vector3.MoveTowards(transform.position, pointTowards, step);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, angleTowards, step);
        }
        else
        {
            transform.position = pointTowards;
            transform.rotation = angleTowards;
        }

        if (transform.position == pointTowards && transform.rotation == angleTowards)
        {
            inTransition = false;
            combatScript.transitionHappening(false);
            manager.transitionFunction(false, currentRoom);
            PlayerAnimator.SetBool("Walking", false);
        }
    }

    public void startTransMovement(Vector3 posTo, Quaternion angTo, int newRoom)
    {
        inTransition = true;
        currentRoom = newRoom;
        combatScript.transitionHappening(true);
        manager.transitionFunction(true, 0);
        pointTowards = posTo;
        angleTowards = angTo;
    }

    void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        obtainLimits();
        numberOfLanes = manager.numberOfLanes;
        lane = manager.obtainLane(transform);
        Debug.Log(lane);
        distanceBetweenLanes = manager.distanceBetweenLanes;
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
            PlayerAnimator.SetBool("Walking", true);
        }
         else
        { PlayerAnimator.SetBool("Walking", false); }
                  
      
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

    //public void transitionHappening(bool happening)
    //{
    //    inTransition = happening;
    //}
}
