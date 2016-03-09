using UnityEngine;
using System.Collections;

public class SensorScript : MonoBehaviour {

    public PlayerMovement movementScript;
    public bool horizontalSensor = true;
    public bool verticalSensor = false;
    public bool fixPosition = true;
    public Vector2 directionWithPlayer;
    public Transform feet;

    GameManager manager;

    void Start()
    {
        setManager();
        obtainDirectionWithPlayer();
        if (verticalSensor && fixPosition)
        {
            fixPositionOfSensor();
        }
    }

    void Update()
    {
        obtainDirectionWithPlayer();
    }

    void OnTriggerStay(Collider c)
    {
        if (c.tag == "Floor" || c.tag == "OpenDoor")
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
        if (c.tag == "Floor" || c.tag == "OpenDoor")
            sendDetection(false);
    }

    void sendDetection(bool detected)
    {
        movementScript.receiveDetection(directionWithPlayer, detected);
    }

    void fixPositionOfSensor()
    {
        int direction = Mathf.RoundToInt(directionWithPlayer.y);
        transform.position = new Vector3(feet.position.x, feet.position.y, feet.position.z + manager.obtainDistanceBetweenLanes() * direction);
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

    public void setManager()
    {
        manager = movementScript.manager;
        
        if (verticalSensor && fixPosition)
        {
            obtainDirectionWithPlayer();
            fixPositionOfSensor();
        }
    }
}
