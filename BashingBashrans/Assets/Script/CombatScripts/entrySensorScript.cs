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

    public float speedOfPositionTransition = 1f;
    public float speedOfRotationTransition = 1f;
    public float speedOfFieldTransition = 1f;

    public bool smoothTransition = true;
    //0 is regular room, 1 is empty room, 2 is boss
    public int typeOfRoom = 0;

    private float[] speedsOfTrans;
    private int [] orderOfTrans;


    private levelManager cameraControl;

    void Start()
    {
        cameraControl = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        int [] temp = { orderTransX, orderTransY, orderTransZ, orderTransR, orderTransF };
        float[] tempF = { speedOfPositionTransition, speedOfRotationTransition, speedOfFieldTransition };
        speedsOfTrans = tempF;
        orderOfTrans = temp;
        //When the player touches the sensor, the camera moves to the position of the camera of the room. 
        //cameraControl = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
    }

    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Player") && managerOfRoom.getStatusOfStage())
        {
            c.GetComponentInParent<PlayerMovement>().startTransMovement(playerObjective.position, playerObjective.rotation, roomNumber, orderOfTrans, speedsOfTrans, smoothTransition);
            managerOfRoom.SendMessage("hideProjectiles", false);
            changeMusic();
        }
    }

    void changeMusic()
    {
        if (typeOfRoom == 1)
        {
            cameraControl.musicManager.SendMessage("stopSong");
        }
        else if (typeOfRoom == 2)
        {
            cameraControl.musicManager.SendMessage("playBoss");
        }
    }
}
