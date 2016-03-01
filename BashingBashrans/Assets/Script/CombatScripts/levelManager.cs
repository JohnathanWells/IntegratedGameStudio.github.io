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

    [Range(0, 2)]
    public int orderTransX = 0;
    [Range(0, 2)]
    public int orderTransY = 0;
    [Range(0, 2)]
    public int orderTransZ = 0;

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

        currentTransCount = 0;
    }

    public void moveCamera()
    {
        Time.timeScale = 1f;
        if (CoolTransition)
        {
            float step = Time.deltaTime * speedOfTransition;
            Vector3 temp = transform.position;

            if (orderTransX == orderTransY && orderTransX == orderTransZ)
            {
                temp = cameras[objectiveManagerNumber].position;
            }
            else
            {
                if (orderTransX == currentTransCount)
                {
                    temp = new Vector3(cameras[objectiveManagerNumber].position.x, temp.y, temp.z);
                }

                if (orderTransY == currentTransCount)
                {
                    temp = new Vector3(temp.x, cameras[objectiveManagerNumber].position.y, temp.z);
                }

                if (orderTransZ == currentTransCount)
                {
                    temp = new Vector3(temp.x, temp.y, cameras[objectiveManagerNumber].position.z);
                }

                //Check what transition is done
                if (orderTransX == currentTransCount && cameras[currentManagerCount].position.x == cameras[objectiveManagerNumber].position.x)
                {
                    currentTransCount++;
                    Debug.Log("Xdone");
                }

                if (orderTransY == currentTransCount && cameras[currentManagerCount].position.y == cameras[objectiveManagerNumber].position.y)
                {
                    currentTransCount++;
                    Debug.Log("Ydone");
                }

                if (orderTransZ == currentTransCount && cameras[currentManagerCount].position.z == cameras[objectiveManagerNumber].position.z)
                {
                    currentTransCount++;
                    Debug.Log("Zdone");
                }
            }

            cameras[currentManagerCount].position = Vector3.MoveTowards(cameras[currentManagerCount].position, temp, step);
            cameras[currentManagerCount].rotation = Quaternion.RotateTowards(cameras[currentManagerCount].rotation, cameras[objectiveManagerNumber].rotation, step);
        }
        else
        {
            cameras[currentManagerCount].position = cameras[objectiveManagerNumber].position;
            cameras[currentManagerCount].rotation = cameras[objectiveManagerNumber].rotation;
        }

        if (cameras[currentManagerCount].position == cameras[objectiveManagerNumber].position && cameras[currentManagerCount].rotation == cameras[objectiveManagerNumber].rotation)
        {
            changeManager(currentManagerCount, objectiveManagerNumber);
            currentManagerCount = objectiveManagerNumber;
            managers[currentManagerCount].SendMessage("closeEntry");
        }
    }

    private void changeManager(int oldManager, int newManager)
    {
        cameras[oldManager].gameObject.SetActive(false);
        cameras[newManager].gameObject.SetActive(true);
        managers[oldManager].enemiesFolder.gameObject.SetActive(false);
        managers[newManager].enemiesFolder.gameObject.SetActive(true);
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
