﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float XVelocity = 2f;
    public Transform playerModel;
    public Animator animator;
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
    //private Vector2 minPos;
    //private Vector2 maxPos;
    private bool canMoveToRight = true;
    private bool canMoveToLeft = true;
    private bool canMoveUp = true;
    private bool canMoveDown = true;
    private bool allowedToMove = true;

    public CombatScript combatScript;

    public GameManager manager;
    private levelManager highManager;

    [Header("Transition")]
    public bool coolTransition = true;
    private Vector3 pointTowards;
    private Quaternion angleTowards;
    private bool inTransition = false;
    private int currentRoom;

    [Header("Freeze")]
    public bool isff;
    public float freesztme = 10;
    public float freezeffct = 2;
    public float freeztart = 0;


   [Header("Animation")]

    public Animator playerAnimator;


    void Start()
    {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        setManager();

     //  playerAnimator = GameObject.FindGameObjectWithTag("PlayerModel").GetComponent<Animator>();

    }

    void Update () {
		

        if (inTransition)
        {
            transitionMove();
        }
        else if(isff)
        {
            froze();

        }
        else if (!manager.gameOver && canMove && !inTransition && isff == false && !manager.gameOver)
        {
            if (Input.GetButton("Horizontal"))
            {
                moveHorizontally(Input.GetAxisRaw("Horizontal"));
                playerAnimator.SetBool("Walking", true);
				playerAnimator.SetFloat("Direction", (Input.GetAxisRaw("Horizontal")));

            }
            else if (Input.GetButtonUp("Horizontal"))
            {
               playerAnimator.SetBool("Walking", false);
				playerAnimator.SetFloat("Direction", (Input.GetAxisRaw("Horizontal")));

            }

            if (Input.GetButtonDown("Vertical"))
            {
                moveVertically(Mathf.RoundToInt(Input.GetAxisRaw("Vertical")));
                playerAnimator.SetBool("Walking", true);
            }
            else if (Input.GetButtonUp("Vertical"))
            {
                playerAnimator.SetBool("Walking", false);
            }
           
        }
	}

   public void froze()
    {
       isff = true;
       if (freeztart == 0)
       {
           XVelocity = (XVelocity / freezeffct);
       }
       if (Input.GetButton("Horizontal"))
       {
           moveHorizontally(Input.GetAxisRaw("Horizontal"));
       }
       if (Input.GetButtonDown("Vertical"))
       {
           moveVertically(Mathf.RoundToInt(Input.GetAxisRaw("Vertical")));
       }
       freeztart+= Time.deltaTime;
       if(freeztart >= freesztme)
       {
           freeztart = 0;
           isff = false;
           XVelocity = (XVelocity * freezeffct);
       }
    }

    void transitionMove()
    {
        Time.timeScale = 1f;
        if (coolTransition)
        {
            playerAnimator.SetBool("Walking", true);
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
            playerAnimator.SetBool("Walking", false);
            inTransition = false;
            canMove = false;
            combatScript.transitionHappening(false);
            manager.transitionFunction(false, currentRoom);
        }
    }

    public void startTransMovement(Vector3 posTo, Quaternion angTo, int newRoom, int [] newOrder, float [] newSpeeds, bool smoothTrans, bool inCoolTransition)
    {
        inTransition = true;
        currentRoom = newRoom;
        combatScript.transitionHappening(true);
        highManager.changeOrderOfTrans(newOrder, newSpeeds, smoothTrans);

        coolTransition = inCoolTransition;
        pointTowards = posTo;
        angleTowards = angTo;
    }

    void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        //obtainLimits();
        numberOfLanes = manager.numberOfLanes;
        lane = manager.obtainLane(transform);
        //Debug.Log("Lane" + lane);
        distanceBetweenLanes = manager.obtainDistanceBetweenLanes();
		playerAnimator.SetBool("Walking", false);
		playerAnimator.SetFloat ("Direction", 0);

        //Debug.Log("Distance between lanes: " + distanceBetweenLanes);
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
            //Debug.Log(distanceBetweenLanes);

            transform.Translate(new Vector3(0, 0, VerticalD));
        }
    }

    public void changeAllowedToMove(bool value)
    {
        allowedToMove = value;
    }

    //public void transitionHappening(bool happening)
    //{
    //    inTransition = happening;
    //}
}
