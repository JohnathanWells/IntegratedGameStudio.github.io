using UnityEngine;
using System.Collections;

public class entrySensorScript : MonoBehaviour {

    public int roomNumber = 0;
    public Transform playerObjective;
    public GameManager managerOfRoom;

    [Range(0, 4)]
    public int orderTransX = 0;
    [Range(0, 4)]
    public int orderTransY = 0;
    [Range(0, 4)]
    public int orderTransZ = 0;
    [Range(0, 4)]
    public int orderTransR = 0;
    [Range(0, 4)]
    public int orderTransF = 0;

    private int [] orderOfTrans;

    //private levelManager cameraControl;

    void Start()
    {
        int [] temp = { orderTransX, orderTransY, orderTransZ, orderTransR, orderTransF };
        orderOfTrans = temp;
        //When the player touches the sensor, the camera moves to the position of the camera of the room. 
        //cameraControl = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player") && managerOfRoom.getStatusOfStage())
        {
            c.GetComponentInParent<PlayerMovement>().startTransMovement(playerObjective.position, playerObjective.rotation, roomNumber, orderOfTrans);
            managerOfRoom.SendMessage("hideProjectiles", false);
            //cameraControl.SendMessage("changePoint", roomNumber);
        }
    }

}
