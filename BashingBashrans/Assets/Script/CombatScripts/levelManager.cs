using UnityEngine;
using System.Collections;

public class levelManager : MonoBehaviour {

    public GameObject[] levelParents;
    public Transform[] cameras;
    public GameManager[] managers;
    public float speedOfTransition = 1f;
    public bool UISwitch = false;
    public Transform Player;
    public bool CoolTransition = true;

    private int[] orderOfTrans;
    private Camera tempCam;
    private Camera tempObCam;

    private int currentTransCount = 0;
    private bool inTransition = false;
    private int currentManagerCount = 0;
    public int objectiveManagerNumber = 0;

	void Start () {
        Time.timeScale = 1f;
        //cameras = new Transform[levelParents.Length];
        //managers = new GameManager[levelParents.Length];

        for (int a = 1; a < managers.Length; a++)
        {
            managers[a].enemiesFolder.gameObject.SetActive(false);
            //cameras[a] = levelParents[a].GetComponentInChildren<Camera>().transform;
            //managers[a] = cameras[a].GetComponentInChildren<GameManager>();
        }

        //levelParents[0].SetActive(true);
	}
	
	void Update () {
        if (objectiveManagerNumber != currentManagerCount)
        {
            inTransition = true;
            moveCamera();
        }
        else
            inTransition = false;
	}

    public void changePoint(int newObjective)
    {
        //Debug.Log("Changing room camera to: " + newObjective);
        objectiveManagerNumber = newObjective;
        tempCam = cameras[currentManagerCount].GetComponent<Camera>();
        tempObCam = cameras[objectiveManagerNumber].GetComponent<Camera>();
        currentTransCount = 0;
    }

    public void changeOrderOfTrans(int[] nO)
    {
        orderOfTrans = nO;
        Debug.Log(orderOfTrans[0] + ", " + orderOfTrans[1] + ", " + orderOfTrans[2] + ", " + orderOfTrans[3] + ", " + orderOfTrans[4]);
    }

    public void moveCamera()
    {
        Time.timeScale = 1f;
        if (CoolTransition)
        {
            //Debug.Log("Current Count: " + currentTransCount);
            float step = Time.deltaTime * speedOfTransition;
            Vector3 temp = cameras[currentManagerCount].position;

            //if (orderOfTrans[0] == orderOfTrans[1] && orderOfTrans[1] == orderOfTrans[2])
            //{
            //    temp = cameras[objectiveManagerNumber].position;
            //}
            //else
            //{
            //Check what transition is done
            if (orderOfTrans[0] == currentTransCount && cameras[currentManagerCount].position.x == cameras[objectiveManagerNumber].position.x)
            {
                currentTransCount++;
                Debug.Log("Xdone");
            }
            if (orderOfTrans[1] == currentTransCount && cameras[currentManagerCount].position.y == cameras[objectiveManagerNumber].position.y)
            {
                currentTransCount++;
                Debug.Log("Ydone");
            }
            if (orderOfTrans[2] == currentTransCount && cameras[currentManagerCount].position.z == cameras[objectiveManagerNumber].position.z)
            {
                currentTransCount++;
                Debug.Log("Zdone");
            }
            if (orderOfTrans[3] == currentTransCount && cameras[currentManagerCount].rotation == cameras[objectiveManagerNumber].rotation)
            {
                currentTransCount++;
                Debug.Log("Rdone");
            }
            if (orderOfTrans[4] == currentTransCount && tempCam.fieldOfView == tempObCam.fieldOfView)
            {
                currentTransCount++;
                Debug.Log("Fdone");
            }

            //Check what to transition right now
            if (orderOfTrans[0] == currentTransCount)
            {
                temp = new Vector3(cameras[objectiveManagerNumber].position.x, temp.y, temp.z);
            }
            if (orderOfTrans[1] == currentTransCount)
            {
                temp = new Vector3(temp.x, cameras[objectiveManagerNumber].position.y, temp.z);
            }
            if (orderOfTrans[2] == currentTransCount)
            {
                temp = new Vector3(temp.x, temp.y, cameras[objectiveManagerNumber].position.z);
            }
            if (orderOfTrans[3] == currentTransCount)
            {
                cameras[currentManagerCount].rotation = Quaternion.RotateTowards(cameras[currentManagerCount].rotation, cameras[objectiveManagerNumber].rotation, step * 2);
            }
            if (orderOfTrans[4] == currentTransCount)
            {
                tempCam.fieldOfView = floatDamp(tempCam.fieldOfView, tempObCam.fieldOfView, speedOfTransition);
            }
            //}

            cameras[currentManagerCount].position = Vector3.MoveTowards(cameras[currentManagerCount].position, temp, step);
            //cameras[currentManagerCount].rotation = Quaternion.RotateTowards(cameras[currentManagerCount].rotation, cameras[objectiveManagerNumber].rotation, step * 2);
            
            //if (tempCam.fieldOfView != tempObCam.fieldOfView)
            //    tempCam.fieldOfView = floatDamp(tempCam.fieldOfView, tempObCam.fieldOfView, speedOfTransition * 2);
        }
        else
        {
            cameras[currentManagerCount].position = cameras[objectiveManagerNumber].position;
            cameras[currentManagerCount].rotation = cameras[objectiveManagerNumber].rotation;
            tempCam.fieldOfView = tempObCam.fieldOfView;
        }

        if (cameras[currentManagerCount].position == cameras[objectiveManagerNumber].position && cameras[currentManagerCount].rotation == cameras[objectiveManagerNumber].rotation && tempCam.fieldOfView == tempObCam.fieldOfView)
        {
            changeManager(currentManagerCount, objectiveManagerNumber);
            currentManagerCount = objectiveManagerNumber;
            managers[currentManagerCount].SendMessage("closeEntry");
        }
    }

    private float floatDamp(float current, float objective, float speed)
    {
        float result = current + speed * Time.deltaTime;
        //Debug.Log(result);

        if (result <= objective)
            return result;
        else
            return objective;
    }

    private void changeManager(int oldManager, int newManager)
    {
        cameras[oldManager].gameObject.SetActive(false);
        cameras[newManager].gameObject.SetActive(true);
        managers[oldManager].enemiesFolder.gameObject.SetActive(false);
        managers[newManager].enemiesFolder.gameObject.SetActive(true);
        //managers[oldManager].ProjectilesFolder.SendMessage("hideProjectiles", false);
        Player.BroadcastMessage("setManager");
        levelParents[newManager].BroadcastMessage("setManager");
    }

    public void playerTransition(bool isHappening)
    {
        levelParents[currentManagerCount].BroadcastMessage("transitionHappening", isHappening);
    }

    public bool getStatusOfTransition()
    {
        return inTransition;
    }
}
