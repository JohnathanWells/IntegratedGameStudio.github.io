using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float XVelocity = 2f;
    public bool canMoveToRight = true;
    public bool canMoveToLeft = true;
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

    GameManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        getLimits();
        numberOfLanes = manager.numberOfLanes;
        lane = manager.getLane(transform);
        distanceBetweenLanes = manager.distanceBetweenLanes;
        //Debug.Log(lane);
        //Debug.Log(minPos + "\n" + maxPos);
    }

	void Update () {

        if (!manager.gameOver)
        {
            float Ax = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Horizontal") && ((Ax > 0 && canMoveToRight) || (Ax < 0 && canMoveToLeft)))
            {
                changeFacingDirection(Ax);
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

                if (lane + dir >= 0 && lane + dir <= numberOfLanes - 1)
                {
                    lane += dir;

                    VerticalD = distanceBetweenLanes * dir;

                    transform.Translate(new Vector3(0, 0, VerticalD));
                }
            }
        }


	}

    void getLimits()
    {
        minPos = manager.minPos;
        maxPos = manager.maxPos;
    }

    void changeFacingDirection(float dir)
    {
        if (dir > 0)
        {
            playerModel.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if (dir < 0)
        {
            playerModel.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
    }

}
