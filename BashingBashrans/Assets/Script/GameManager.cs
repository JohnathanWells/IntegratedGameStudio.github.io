using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [Header("Array Sizes")]
    public int numberOfLanes;
    public int numberOfAvailableEnemies;

    [Header("Other Elements in Level")]
    public Transform player;
    public Transform ProjectilesFolder;
    public Transform LeftDownCorner;
    public Transform RightUpCorner;
    public Transform enemiesFolder;
    public ParticleManager PM;
    public SoundEffectManager SFX;
    public bool[] lanesOccupied;
    public int maxNumberOfEnemiesInQueue = 3;
    private CombatScript playerScript;

    [Header("Game Over, Conditions and Enemy Respawn Management")]
    public bool gameOver = false;
    private bool paused = false;
    public int[] enemiesInQueue;
    public bool conditionIsAllEnemiesDefeated = false;
    //public bool conditionIsTime = false;
    public int secondsBetweenRecount = 30;
    private int secondsBeforeRecount = 0;
    //public int enemyDefeatedCondition = 7;
    public int enemiesRespawnEveryRecount = 3;
    //private int enemiesAlive = 0;
    public bool enemiesMove = false;
    public Transform[] availableEnemies;
    public Vector3[] EnemySpawnpoints;
    public AudioClip gameOverSound;

    [Header("UI")]
    public GUIStyle style;
    private Vector2 HealthTextSize;

    [Header("Events")]
    public bool BouldersActivated = false;
    public bool randomBoulderFall = true;
    public Transform Boulder;
    public float cooldownBetweenTraps = 30f;
    private float trapCooldown = 0;

    [Header("Others")]
    public Vector2 minPos;
    public Vector2 maxPos;
    public float distanceBetweenLanes;

    void Awake()
    {
        StartCoroutine(startTimeCount());
        //availableEnemies = new Transform[numberOfAvailableEnemies];
        //HealthTextSize = new Vector2((Screen.width * 400) / 551, (Screen.height * 100) / 310);
        enemiesInQueue = new int[numberOfLanes];
        lanesOccupied = new bool[numberOfLanes];
        HealthTextSize = new Vector2(300, 300);
        style.fontSize = style.fontSize * Screen.width/ 551;
        distanceBetweenLanes = getDistanceBetweenLanes();
        playerScript = player.GetComponent<CombatScript>();

        Debug.Log("MinPos " + minPos + "\nMaxPos " + maxPos);
        Debug.Log("Distance between lanes: " + distanceBetweenLanes);
    }

    void Update()
    {
        if (BouldersActivated)
        {
            trapCooldown += Time.deltaTime;
            if (trapCooldown >= cooldownBetweenTraps)
            {
                setFallingRock(randomBoulderFall);
            }
        }

        if (gameOver)
        {
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        //GUI.Box(new Rect(new Vector2(2, 2), HealthTextSize), "HP: " + playerScript.getHealth(), style);
        
        //string objectiveText;

        //if (conditionIsEnemiesDefeated)
        //{
        //    objectiveText = playerScript.getEnemiesDefeated() + "/" + enemyDefeatedCondition;
        //}
        //else
        //{
        //    objectiveText = "";
        //}

        //GUI.Box(new Rect(new Vector2(Screen.width - 300, Screen.height - 300), new Vector2(200, 200)), "Recount in: " + secondsBeforeRecount + "\n" + objectiveText, style); 
    }

    void setFallingRock(bool isRandom)
    {
        trapCooldown = 0;

        Transform trap;

        if (!isRandom)
        {
            trap = Instantiate(Boulder, player.position, Quaternion.Euler(new Vector3(45, 0, 0))) as Transform;
            trap.parent = ProjectilesFolder;
        }
        else if (randomBoulderFall)
        {
            int yLane = Mathf.RoundToInt(Random.Range(minPos.y, maxPos.y));
            trap = Instantiate(Boulder, new Vector3(Mathf.RoundToInt(Random.Range(minPos.x, maxPos.x)), yLane, yLane), Quaternion.Euler(new Vector3(45, 0, 0))) as Transform;
            trap.parent = ProjectilesFolder;
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

    IEnumerator startTimeCount()
    {
        while (true)
        {
            bool conditionAccomplished = false;
            secondsBeforeRecount = secondsBetweenRecount;
            for (int a = 0; a < secondsBetweenRecount; a++)
            {
                yield return new WaitForSeconds(1);
                secondsBeforeRecount--;
            }

            if (conditionIsAllEnemiesDefeated)
            {
                if (areEnemiesAlive())
                    addMoreEnemies();
                else
                    conditionAccomplished = true;
            }
            //else if (conditionIsAllEnemiesDefeated)
            //{
            //    if (playerScript.getEnemiesDefeated() < enemyDefeatedCondition)
            //    {
            //        spawnMoreEnemies();
            //    }
            //    else
            //        conditionAccomplished = true;
            //}

            for (int a = 0; a < numberOfLanes; a++)
            {
                Debug.Log("Lane " + a + ": " + enemiesInQueue[a] + " Occ: " + lanesOccupied[a]);
            }

            if (conditionAccomplished)
                break;
        }
    }

    //void spawnMoreEnemies()
    //{
    //    for (int a = 0; a < enemiesRespawnEveryRecount; a++)
    //    {
    //        int ran = Random.Range(0, numberOfLanes);
    //        int ranEnemy = Random.Range(0, numberOfAvailableEnemies);
    //        Debug.Log(ran + "\n" + ranEnemy);
    //        Transform enemy = Instantiate(availableEnemies[ranEnemy], new Vector3(EnemySpawnpoints[ran].x, EnemySpawnpoints[ran].y, EnemySpawnpoints[ran].y), Quaternion.identity) as Transform;
    //        enemy.parent = enemiesFolder;

    //        if (enemiesMove)
    //            enemy.GetComponent<EnemyTurretScript>().moveToTheLeft = true;
    //    }
    //}
    
    void addMoreEnemies()
    {
        int numberOfEnemiesPlaced = 0;
        bool[] laneChecked = new bool[numberOfLanes];
        int lanesFull = 0;
        int howManyLanesChecked = 0;
        int ran = 0;

        for (int b = 0; b < 100; b++)
        {            
            ran = Random.Range(0, numberOfLanes);

            if (!laneChecked[ran])
            {
                if (lanesOccupied[ran] && enemiesInQueue[ran] < maxNumberOfEnemiesInQueue)
                {
                    enemiesInQueue[ran]++;
                    numberOfEnemiesPlaced++;
                }
                else if (!lanesOccupied[ran] && enemiesInQueue[ran] == 0)
                {
                    spawnEnemy(-1, EnemySpawnpoints[ran]);
                    numberOfEnemiesPlaced++;
                }

                if (lanesOccupied[ran] && enemiesInQueue[ran] == maxNumberOfEnemiesInQueue)
                {
                    lanesFull++;
                }

                laneChecked[ran] = true;
                howManyLanesChecked++;

                if (numberOfEnemiesPlaced >= enemiesRespawnEveryRecount)
                    break;
                else
                {
                    if (howManyLanesChecked == numberOfLanes)
                    {
                        if (lanesFull >= numberOfLanes)
                            break;
                        else
                        {
                            for (int a = 0; a < numberOfLanes; a++)
                            {
                                laneChecked[a] = false;
                            }
                        }
                    }
                }
            }

        }
    }

    public void spawnEnemy(int negativeForRandom, Vector3 position)
    {
        int enemyNum = 0; 

        if (negativeForRandom < 0 || negativeForRandom > numberOfAvailableEnemies)
        {
            enemyNum = Random.Range(0, numberOfAvailableEnemies);
        }
        else
        {
            enemyNum = negativeForRandom;
        }

        Transform enemy = Instantiate(availableEnemies[enemyNum], position, Quaternion.identity) as Transform;
        enemy.GetComponentInChildren<EnemyTurretScript>().moveToTheLeft = enemiesMove;
        enemy.parent = enemiesFolder;
    }

    public float getDistanceBetweenLanes()
    {
        return Mathf.Abs((RightUpCorner.position.z - LeftDownCorner.position.z + LeftDownCorner.lossyScale.z/2 + RightUpCorner.lossyScale.z/2) / numberOfLanes);
    }

    public int getLane(Transform questioner)
    {
        return Mathf.FloorToInt((questioner.position.z - LeftDownCorner.position.z)/distanceBetweenLanes);
    }

    bool areEnemiesAlive()
    {
        bool areAlive = false;

        for (int a = 0; a < numberOfLanes; a++)
        {
            if (lanesOccupied[a])
            {
                areAlive = true;
                break;
            }
        }

        return areAlive;
    }
}
