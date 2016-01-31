using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float XVelocity = 1f;
    public float YVelocity = 1f;
    public Vector2 minPos;
    public Vector2 maxPos;
    public bool squareMovementX = false;
    public bool squareMovementY = false;

    private float VerticalD;
    private float HorizontalD;
    private float roundedMovementVertical = 1;
    private int lane = 0;

    GameManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        minPos = manager.LeftDownCorner.position;
        maxPos = manager.RightUpCorner.position;
        roundedMovementVertical = manager.getDistanceBetweenLanes();
        lane = Mathf.RoundToInt(maxPos.y - transform.position.y);
        //Debug.Log(lane);

    }

	void Update () {

        if (!manager.gameOver)
        {
            if (!squareMovementX)
            {
                HorizontalD = XVelocity * Time.deltaTime * Input.GetAxisRaw("Horizontal");

                if (transform.position.x + HorizontalD < minPos.x)
                    HorizontalD = 0;
                else if (transform.position.x + HorizontalD > maxPos.x)
                    HorizontalD = 0;

                transform.Translate(new Vector3(HorizontalD, 0, 0));
            }
            else if (Input.GetButtonDown("Horizontal") && squareMovementX)
            {
                HorizontalD = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

                if (transform.position.x + HorizontalD < minPos.x)
                    HorizontalD = 0;
                else if (transform.position.x + HorizontalD > maxPos.x)
                    HorizontalD = 0;

                transform.Translate(new Vector3(HorizontalD, 0, 0));
                Mathf.RoundToInt(transform.position.x);
            }


            if (!squareMovementY)
            {
                VerticalD = YVelocity * Time.deltaTime * Input.GetAxisRaw("Vertical");

                if (transform.position.y + VerticalD < minPos.y)
                    VerticalD = 0;
                else if (transform.position.y + VerticalD > maxPos.y)
                    VerticalD = 0;

                transform.Translate(new Vector3(0, VerticalD, 0));
            }
            else if (Input.GetButtonDown("Vertical") && squareMovementY)
            {
                VerticalD = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

                if (transform.position.y + VerticalD < minPos.y)
                    VerticalD = 0;
                else if (transform.position.y + VerticalD > maxPos.y)
                    VerticalD = 0;

                transform.Translate(new Vector3(0, VerticalD, 0));
                Mathf.RoundToInt(transform.position.y);
                transform.Translate(new Vector3(0, VerticalD, VerticalD));
            }
            else if (Input.GetButtonDown("Vertical") && squareMovementY)
            {
                int dir = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

                if (lane - dir >= 0 && lane - dir <= manager.numberOfLanes - 1)
                {
                    lane -= dir;

                    VerticalD = (lane) * roundedMovementVertical;
                    Debug.Log(roundedMovementVertical);

                    //if (transform.position.y + VerticalD >= minPos.y && transform.position.y + VerticalD <= maxPos.y)
                    transform.position = new Vector3(transform.position.x, maxPos.y - VerticalD, maxPos.y - VerticalD);
                }
            }
        }


	}

}
