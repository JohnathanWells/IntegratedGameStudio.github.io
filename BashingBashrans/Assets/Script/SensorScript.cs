using UnityEngine;
using System.Collections;

public class SensorScript : MonoBehaviour {

    public PlayerMovement movementScript;

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
        if (transform.position.x > movementScript.transform.position.x)
        {
            movementScript.canMoveToRight = detected;
            return;
        }
        else if (transform.position.x < movementScript.transform.position.x)
        {
            movementScript.canMoveToLeft = detected;
            return;
        }
    }
}
