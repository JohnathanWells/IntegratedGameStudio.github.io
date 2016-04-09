using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class levelManager : MonoBehaviour {

    public GameObject[] levelParents;
    public Transform[] cameras;
    public GameManager[] managers;
    //public float speedOfTransition = 1f;
    public bool UISwitch = false;
    public Transform Player;
    public bool CoolTransition = true;
    public MusicScript musicManager;
    public SoundEffectManager SFXManager;
    private AudioSource musicSource;
    private AudioSource soundSource;

    private int[] orderOfTrans;
    private float[] speedsOfTrans;
    private Camera tempCam;
    private Camera tempObCam;
    private CombatScript playerScript;

    private int currentTransCount = 0;
    private bool inTransition = false;
    public int currentManagerCount = 0;
    public int objectiveManagerNumber = 0;
    bool floorCleared = false;
    int acumulatedDamage = 0;

    [Header("UI", order = 1)]
    public Text playerHealthText;
    public Text TimeText;
    public GameObject pauseMenu;
    private float time = 0;
    private bool paused = false;

    [Header("Passwords")]
    public int floorNumber = 0;
    public string[] passwords;

	void Start () {
        SaveLoad.Load();
        Time.timeScale = 1f;
        //SaveLoad.Delete();
        //cameras = new Transform[levelParents.Length];
        //managers = new GameManager[levelParents.Length];

        for (int a = 0; a < managers.Length; a++)
        {
            managers[a].enemiesFolder.gameObject.SetActive(false);
            //cameras[a] = levelParents[a].GetComponentInChildren<Camera>().transform;
            //managers[a] = cameras[a].GetComponentInChildren<GameManager>();
        }

        managers[currentManagerCount].enemiesFolder.gameObject.SetActive(true);

        playerScript = Player.GetComponentInChildren<CombatScript>();
        //musicManager = GameObject.FindGameObjectWithTag("Music Manager").GetComponent<MusicScript>();
        musicManager.SendMessage("playFloor");

        //playerHealthText.rectTransform.position = new Vector2(playerHealthText.rectTransform.position.x / 558 * Screen.width, playerHealthText.rectTransform.position.y / 314 * Screen.height);
        //playerHealthText.rectTransform.sizeDelta = new Vector2(playerHealthText.rectTransform.sizeDelta.x * Screen.width / 558, playerHealthText.rectTransform.sizeDelta.y / 314 * Screen.height);
        //playerHealthText.fontSize = playerHealthText.fontSize * Screen.width / 558;
        //TimeText.rectTransform.position = new Vector2(TimeText.rectTransform.position.x * Screen.width / 558, TimeText.rectTransform.position.y * Screen.height / 314);
        obtainPasswords();
        musicSource = musicManager.source;
        soundSource = SFXManager.source;
        //levelParents[0].SetActive(true);
	}
	
	void Update () {
        if (!floorCleared)
        {
            time += Time.deltaTime;

            if (objectiveManagerNumber != currentManagerCount)
            {
                inTransition = true;
                moveCamera();
            }
            else
                inTransition = false;
        }

        if (Input.GetKey(KeyCode.K))
        {
            Application.CaptureScreenshot("Screenshot.png");
        }

        if (Input.GetButtonDown("Pause"))
        {
            togglePause();
        }
	}

    void OnGUI()
    {
        //if (Input.GetButtonDown("Pause"))
        //{
        //    Debug.Log(paused);
        //    togglePause();
        //    Debug.Log("Went to " + paused);
        //}

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int miliSeconds = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 100);

        TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliSeconds.ToString("00");
        playerHealthText.text = "HP: " + playerScript.getHealth();
    }

    public void togglePause()
    {
        if (!paused)
        {
            paused = true;
            playerHealthText.transform.parent.gameObject.SetActive(false);
            soundSource.Pause();
            musicSource.Pause();
            pauseMenu.SetActive(true);
            SaveLoad.Load();
            pauseMenu.SendMessage("setActiveButtons");
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            soundSource.UnPause();
            musicSource.UnPause();
            playerHealthText.transform.parent.gameObject.SetActive(true);
            pauseMenu.SetActive(false);
            paused = false;
        }
    }

    public void changePoint(int newObjective)
    {
        //Debug.Log("Changing room camera to: " + newObjective);
        objectiveManagerNumber = newObjective;
        tempCam = cameras[currentManagerCount].GetComponent<Camera>();
        tempObCam = cameras[objectiveManagerNumber].GetComponent<Camera>();
        currentTransCount = 0;
    }

    public void changeOrderOfTrans(int[] nO, float[] nS, bool nT)
    {
        orderOfTrans = nO;
        speedsOfTrans = nS;
        CoolTransition = nT;
        //Debug.Log(orderOfTrans[0] + ", " + orderOfTrans[1] + ", " + orderOfTrans[2] + ", " + orderOfTrans[3] + ", " + orderOfTrans[4]);
    }

    public void moveCamera()
    {
        Time.timeScale = 1f;
        if (CoolTransition)
        {
            //0 is position transition speed, 1 is rotation transition speed and 2 is field of view transition speed
            float step = Time.deltaTime * speedsOfTrans[0];
            float stepR = Time.deltaTime * speedsOfTrans[1];
            float stepF = Time.deltaTime * speedsOfTrans[2];
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
                //Debug.Log("Xdone");
            }
            if (orderOfTrans[1] == currentTransCount && cameras[currentManagerCount].position.y == cameras[objectiveManagerNumber].position.y)
            {
                currentTransCount++;
                //Debug.Log("Ydone");
            }
            if (orderOfTrans[2] == currentTransCount && cameras[currentManagerCount].position.z == cameras[objectiveManagerNumber].position.z)
            {
                currentTransCount++;
                //Debug.Log("Zdone");
            }
            if (orderOfTrans[3] == currentTransCount && cameras[currentManagerCount].rotation == cameras[objectiveManagerNumber].rotation)
            {
                currentTransCount++;
                //Debug.Log("Rdone");
            }
            if (orderOfTrans[4] == currentTransCount && tempCam.fieldOfView == tempObCam.fieldOfView)
            {
                currentTransCount++;
                //Debug.Log("Fdone");
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
                cameras[currentManagerCount].rotation = Quaternion.RotateTowards(cameras[currentManagerCount].rotation, cameras[objectiveManagerNumber].rotation, speedsOfTrans[1]);
            }
            if (orderOfTrans[4] == currentTransCount)
            {
                tempCam.fieldOfView = floatDamp(tempCam.fieldOfView, tempObCam.fieldOfView, speedsOfTrans[2]);
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
            Player.GetComponent<PlayerMovement>().changeCanMove(true);
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

    public string getPassword(int numberOfPassword)
    {
        //Debug.Log("Number: " + numberOfPassword);
        print(numberOfPassword + ": " + passwords[numberOfPassword]);
        return passwords[numberOfPassword];
    }

    private void obtainPasswords()
    {
        SaveLoad.Load();
        passwords = SaveLoad.savedGame.getAllPasswordsFromAFloor(floorNumber);

        //for (int a = 0; a < passwords.Length; a++)
            //Debug.Log("Password " + a + ": " + passwords[a]);
    }

    public float getTime()
    {
        return time;
    }

    public int getPlayerHealth()
    {
        return playerScript.getHealth();
    }

    public void floorIsCleared()
    {
        floorCleared = true;
        SaveLoad.Load();
        SaveLoad.savedGame.setUnlockFloor(floorNumber + 1, true);
    }

    public void accumulateDamage(int damage)
    {
        acumulatedDamage += damage;
    }

    public int getAccumulatedDamage()
    {
        return acumulatedDamage;
    }

    public void healPlayer()
    {
        playerScript.SendMessage("healPlayer");
    }
}
