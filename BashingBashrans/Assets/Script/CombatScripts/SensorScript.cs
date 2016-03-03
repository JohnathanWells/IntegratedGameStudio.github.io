﻿using UnityEngine;
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
        transform.position = new Vector3(feet.position.x, feet.position.y, manager.distanceBetweenLanes * direction + transform.position.z);
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
    }
}