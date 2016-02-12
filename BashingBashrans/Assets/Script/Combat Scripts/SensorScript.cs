using UnityEngine;
using System.Collections;

public class SensorScript : MonoBehaviour {

    public PlayerMovement movementScript;
    public bool horizontalSensor = true;
    public bool verticalSensor = false;
    public Vector2 directionWithPlayer;

    GameManager manager;

    void Start()
    {
        manager = movementScript.manager;
        obtainDirectionWithPlayer();

        if (verticalSensor)
        {
            fixPositionOfSensor(Mathf.RoundToInt(directionWithPlayer.y));
        }
    }

    void Update()
    {
        obtainDirectionWithPlayer();
    }

    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Floor"))
        {
            sendDetection(true);
        }
        else
        {
            sendDetection(false);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Floor"))
            sendDetection(false);
    }

    void sendDetection(bool detected)
    {
        movementScript.receiveDetection(directionWithPlayer, detected);
    }

    void fixPositionOfSensor(int direction)
    {
        
        transform.position = new Vector3(0, 0, manager.distanceBetweenLanes * transform.position.z / 1);
    }

    void obtainDirectionWithPlayer()
    {
        if (horizontalSensor)
        {
            if (transform.position.x > movementScript.transform.position.x)
                directionWithPlayer = Vector2.right;
            else
                directionWithPlayer = Vector2.left;
        }
        else if (verticalSensor)
        {
            if (transform.position.z > movementScript.transform.position.z)
                directionWithPlayer = Vector2.up;
            else
                directionWithPlayer = Vector2.down;
        }
    }
}
