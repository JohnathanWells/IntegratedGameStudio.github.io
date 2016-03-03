using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [Header("Other Elements in Level")]
    public Transform player;
    public Transform ProjectilesFolder;
    public Transform LeftDownCorner;
    public Transform RightUpCorner;
    public int numberOfLanes;
    public Transform enemiesFolder;
    public ParticleManager PM;
    public SoundEffectManager SFX;
    public Transform entryDoor;
    public Transform exitDoor;
    public Animator DoorAnimator;
    private CombatScript playerScript;
    private levelManager highManager;
    int enemiesAtBeginning = 0;
    int enemiesDestroyed = 0;

    [Header("Game Over, Conditions and Enemy Respawn Management")]
    public bool gameOver = false;
    public bool paused = false;
    public AudioClip gameOverSound;
    public string nextLevelName;
    private bool stageCleared = false;

    [Header("UI")]
    public Camera cameraLinked;
    public GUIStyle PlayerFont;
    private Vector2 PlayerHealthTextSize;
    public bool UIActivated = true;

    [Header("Events")]
    public bool BouldersActivated = false;
    public bool randomBoulderFall = true;
    public Transform Boulder;
    public float cooldownBetweenTraps = 30f;
    public Vector3 offsetOfTrap;
    private float trapCooldown = 0;

    [Header("Others")]

    public Vector2 minPos;
    public Vector2 maxPos;
    public float distanceBetweenLanes;

    void Awake()
    {
        setStart();
    }

    void Update()
    {
        if (!gameOver)
        {
            if (Input.GetButtonDown("Pause"))
            {
                switchPause();
            }

            if (BouldersActivated)
            {
                boulderSetCount();
            }
        }

        if (gameOver)
        {
            gameIsOver();
        }
    }

    void OnGUI()
    {

        if (paused)
        {
            if (GUI.Button(new Rect(new Vector2(Screen.width / 2 - 100, Screen.height / 2 - 25), new Vector2(200, 50)), "Resume"))
                switchPause();
        }

        //if (enemiesDestroyed == enemiesAtBeginning)
        //{
        //    if (GUI.Button(new Rect(new Vector2(Screen.width / 2 - 100, Screen.height / 2 - 50), new Vector2(200, 100)), "Enemies Defeated\nContinue"))
        //    {
        //        Time.timeScale = 1f;
        //        //Application.LoadLevel(nextLevelName);
        //        nextBattle();
        //    }
        //}

        if (UIActivated)
            GUI.Box(new Rect(new Vector2(2,2), PlayerHealthTextSize), "HP: " + playerScript.getHealth(), PlayerFont);
    }

    public void transitionFunction(bool isHappening, int newRoomNumber)
    {
        //highManager.SendMessage("playerTransition", isHappening);

        if (!isHappening)
        {
            highManager.changePoint(newRoomNumber);
        }
    }

    void setFallingRock(bool isRandom)
    {
        trapCooldown = 0;

        Transform trap;

        if (!isRandom)
        {
            trap = Instantiate(Boulder, player.position + offsetOfTrap, Quaternion.Euler(new Vector3(90, 0, 0))) as Transform;
            trap.parent = ProjectilesFolder;
        }
        else if (randomBoulderFall)
        {
            int yLane = Mathf.RoundToInt(Random.Range(minPos.y, maxPos.y));
            trap = Instantiate(Boulder, new Vector3(Mathf.RoundToInt(Random.Range(minPos.x, maxPos.x)), yLane, yLane), Quaternion.Euler(new Vector3(45, 0, 0))) as Transform;
            trap.parent = ProjectilesFolder;
        }
    }

    void gameIsOver()
    {
        Time.timeScale = 0f;
    }

    void boulderSetCount()
    {
        trapCooldown += Time.deltaTime;
        if (trapCooldown >= cooldownBetweenTraps)
        {
            setFallingRock(randomBoulderFall);
        }
    }

    Vector2 getPlayerPos()
    {
        return player.position;
    }

    public void GameOver()
    {
        SFX.PlaySound(gameOverSound);
        gameOver = true;
    }

    public IEnumerator destroyParticleSystem(ParticleSystem system, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(system.gameObject);
    }

    //IEnumerator startTimeCount()
    //{
    //    while (true)
    //    {
    //        bool conditionAccomplished = false;
    //        secondsBeforeRecount = secondsBetweenRecount;
    //        for (int a = 0; a < secondsBetweenRecount; a++)
    //        {
    //            yield return new WaitForSeconds(1);
    //            secondsBeforeRecount--;
    //        }

    //        if (conditionIsAllEnemiesDefeated)
    //        {
    //            if (areEnemiesAlive())
    //                addMoreEnemies();
    //            else
    //                conditionAccomplished = true;
    //        }
    //        //else if (conditionIsAllEnemiesDefeated)
    //        //{
    //        //    if (playerScript.getEnemiesDefeated() < enemyDefeatedCondition)
    //        //    {
    //        //        spawnMoreEnemies();
    //        //    }
    //        //    else
    //        //        conditionAccomplished = true;
    //        //}

    //        for (int a = 0; a < numberOfLanes; a++)
    //        {
    //            Debug.Log("Lane " + a + ": " + enemiesInQueue[a] + " Occ: " + lanesOccupied[a]);
    //        }

    //        if (conditionAccomplished)
    //            break;
    //    }
    //}
    
    //void addMoreEnemies()
    //{
    //    int numberOfEnemiesPlaced = 0;
    //    bool[] laneChecked = new bool[numberOfLanes];
    //    int lanesFull = 0;
    //    int howManyLanesChecked = 0;
    //    int ran = 0;

    //    for (int b = 0; b < 100; b++)
    //    {            
    //        ran = Random.Range(0, numberOfLanes);

    //        if (!laneChecked[ran])
    //        {
    //            if (lanesOccupied[ran] && enemiesInQueue[ran] < maxNumberOfEnemiesInQueue)
    //            {
    //                enemiesInQueue[ran]++;
    //                numberOfEnemiesPlaced++;
    //            }
    //            else if (!lanesOccupied[ran] && enemiesInQueue[ran] == 0)
    //            {
    //                spawnEnemy(-1, EnemySpawnpoints[ran]);
    //                numberOfEnemiesPlaced++;
    //            }

    //            if (lanesOccupied[ran] && enemiesInQueue[ran] == maxNumberOfEnemiesInQueue)
    //            {
    //                lanesFull++;
    //            }

    //            laneChecked[ran] = true;
    //            howManyLanesChecked++;

    //            if (numberOfEnemiesPlaced >= enemiesRespawnEveryRecount)
    //                break;
    //            else
    //            {
    //                if (howManyLanesChecked == numberOfLanes)
    //                {
    //                    if (lanesFull >= numberOfLanes)
    //                        break;
    //                    else
    //                    {
    //                        for (int a = 0; a < numberOfLanes; a++)
    //                        {
    //                            laneChecked[a] = false;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //    }
    //}

    //public void spawnEnemy(int negativeForRandom, Vector3 position)
    //{
    //    int enemyNum = 0; 

    //    if (negativeForRandom < 0 || negativeForRandom > numberOfAvailableEnemies)
    //    {
    //        enemyNum = Random.Range(0, numberOfAvailableEnemies);
    //    }
    //    else
    //    {
    //        enemyNum = negativeForRandom;
    //    }

    //    Transform enemy = Instantiate(availableEnemies[enemyNum], position, Quaternion.identity) as Transform;
    //    enemy.GetComponentInChildren<EnemyTurretScript>().moveToTheLeft = enemiesMove;
    //    enemy.parent = enemiesFolder;
    //}

    public float obtainDistanceBetweenLanes()
    {
        return Mathf.Abs((RightUpCorner.position.z - LeftDownCorner.position.z + LeftDownCorner.lossyScale.z/2 + RightUpCorner.lossyScale.z/2) / numberOfLanes);
    }

    public int obtainLane(Transform questioner)
    {
        return Mathf.FloorToInt((questioner.position.z - LeftDownCorner.position.z)/distanceBetweenLanes);
    }

    //bool areEnemiesAlive()
    //{
    //    bool areAlive = false;

    //    for (int a = 0; a < numberOfLanes; a++)
    //    {
    //        if (lanesOccupied[a])
    //        {
    //            areAlive = true;
    //            break;
    //        }
    //    }

    //    return areAlive;
    //}

    void switchPause()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1f;
        }
        else
        {
            paused = true;
            Time.timeScale = 0f;
        }
    }

    public void addEnemyInLevel()
    {
        enemiesAtBeginning++;
    }

    public void addEnemyDestroyed()
    {
        enemiesDestroyed++;

        Debug.Log(enemiesDestroyed + "/" + enemiesAtBeginning);

        if (enemiesDestroyed == enemiesAtBeginning)
        {
            stageIsCleared();
            openExit();
        }
    }

    void nextBattle()
    {
        highManager.SendMessage("goToNextPoint");
    }

    public void setStart()
    {
        //StartCoroutine(startTimeCount());
        //availableEnemies = new Transform[numberOfAvailableEnemies];
        //numberOfAvailableEnemies = availableEnemies.Length;
        Time.timeScale = 1f;
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        player = highManager.Player;
        PlayerHealthTextSize = new Vector2((Screen.width * 400) / 551, (Screen.height * 100) / 310);
        //enemiesInQueue = new int[numberOfLanes];
        //lanesOccupied = new bool[numberOfLanes];
        PlayerFont.fontSize = PlayerFont.fontSize * Screen.width / 551;
        distanceBetweenLanes = obtainDistanceBetweenLanes();
        playerScript = player.GetComponentInChildren<CombatScript>();
        //Debug.Log("MinPos " + minPos + "\nMaxPos " + maxPos);
        //Debug.Log("Distance between lanes: " + distanceBetweenLanes);
      //  DoorAnimator = FindGameObjectWithTag "Door";
    }

    public void switchUI(bool value)
    {
        if (UIActivated)
            UIActivated = value;
        else
            UIActivated = value;
    }

    private void stageIsCleared()
    {
        stageCleared = true;
        DoorAnimator.SetBool("Open", true);

    }

    public bool getStatusOfStage()
    {
        return stageCleared;
    }

    public void closeEntry()
    {
        entryDoor.gameObject.SetActive(true);
    }

    public void openExit()
    {
        exitDoor.gameObject.SetActive(false);
    }

    public void hideProjectiles(bool value)
    {
        ProjectilesFolder.gameObject.SetActive(value);
    }
}
