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
            moveCamera();
        }
	}

    public void changePoint(int newObjective)
    {
        //Debug.Log("Changing room camera to: " + newObjective);
        objectiveManagerNumber = newObjective;
    }

    public void moveCamera()
    {
        Time.timeScale = 1f;
        if (CoolTransition)
        {
            float step = Time.deltaTime * speedOfTransition;
            cameras[currentManagerCount].position = Vector3.MoveTowards(cameras[currentManagerCount].position, cameras[objectiveManagerNumber].position, step);
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
}
