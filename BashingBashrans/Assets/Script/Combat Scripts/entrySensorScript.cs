using UnityEngine;
using System.Collections;

public class entrySensorScript : MonoBehaviour {

    public int roomNumber = 0;
    public Transform playerObjective;
    public GameManager managerOfRoom;

    private levelManager cameraControl;

    void Start()
    {
        //When the player touches the sensor, the camera moves to the position of the camera of the room. 
        cameraControl = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player") && managerOfRoom.getStatusOfStage())
        {
            c.GetComponentInParent<PlayerMovement>().startTransMovement(playerObjective.position, playerObjective.rotation, roomNumber);
            //cameraControl.SendMessage("changePoint", roomNumber);
        }
    }

}
