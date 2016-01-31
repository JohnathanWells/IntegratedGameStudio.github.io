using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [Header("Array Sizes")]
    public int numberOfLanes;
    //public int numberOfAvailableEnemies;

    [Header("Other Elements in Level")]
    public Transform player;
    public Transform ProjectilesFolder;
    public Transform LeftDownCorner;
    public Transform RightUpCorner;
    public Transform enemiesFolder;
    public ParticleManager PM;
    public SoundEffectManager SFX;
    //private CombatScript playerScript;

    [Header("Game Over, Conditions and Enemy Respawn Management")]
    public bool gameOver = false;
    private bool paused = false;
    //public bool conditionIsEnemiesDefeated = false;
    //public bool conditionIsTime = false;
    //public int secondsBetweenRecount = 30;
    //private int secondsBeforeRecount = 0;
    //public int enemyDefeatedCondition = 7;
    //public int enemiesRespawnEveryRecount = 3;
    //private int enemiesAlive = 0;
    //public bool enemiesMove = false;
    //public Transform[] availableEnemies;
    //public Vector2[] EnemySpawnpoints;
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

    void Start()
    {
        //StartCoroutine(startTimeCount());
        //availableEnemies = new Transform[numberOfAvailableEnemies];
        HealthTextSize = new Vector2((Screen.width * 400) / 551, (Screen.height * 100) / 310);
        style.fontSize = style.fontSize * Screen.width/ 551;
        distanceBetweenLanes = getDistanceBetweenLanes();
        //playerScript = player.GetComponent<CombatScript>();

        Debug.Log("MinPos " + minPos + "\nMaxPos " + maxPos);
    }

    void Update()
    {
        if (BouldersActivated)
        {
            trapCooldown += Time.deltaTime;
            if (trapCooldown >= cooldownBetweenTraps)
            {
                Transform trap;
                int yLane = Mathf.RoundToInt(Random.Range(minPos.y, maxPos.y));
                trapCooldown = 0;

                if (randomBoulderFall)
                {
                    trap = Instantiate(Boulder, new Vector3(Mathf.RoundToInt(Random.Range(minPos.x, maxPos.x)), yLane, yLane), Quaternion.Euler(new Vector3(45, 0, 0))) as Transform;
                }
                else
                {
                    trap = Instantiate(Boulder, player.position, Quaternion.Euler(new Vector3(45, 0, 0))) as Transform;
                }
                
                trap.parent = ProjectilesFolder;
            }
        }

        if (gameOver)
        {
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        //GUI.Box(new Rect(new Vector2(2, 2), HealthTextSize), "HP: " + PlayerCombat.getHealth(), style);
        
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
    //    while(true)
    //    {
    //        bool conditionAccomplished = false;
    //        secondsBeforeRecount = secondsBetweenRecount;
    //        for (int a = 0; a < secondsBetweenRecount; a++)
    //        {
    //            yield return new WaitForSeconds(1);
    //            secondsBeforeRecount--;
    //        }

    //        if (conditionIsTime)
    //        {
    //            enemiesFolder.BroadcastMessage("AreYouAlive", null, SendMessageOptions.DontRequireReceiver);
    //            print(enemiesAlive);
    //            if (enemiesAlive > 0)
    //                spawnMoreEnemies();
    //            else
    //                conditionAccomplished = true;
    //        }
    //        else if (conditionIsEnemiesDefeated)
    //        {
    //            if (playerScript.getEnemiesDefeated() < enemyDefeatedCondition)
    //            {
    //                spawnMoreEnemies();
    //            }
    //            else
    //                conditionAccomplished = true;
    //        }

    //        if (conditionAccomplished)
    //            break;
    //    }
    //}

    //public void IAmAlive()
    //{
    //    enemiesAlive++;
    //}

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

    public float getDistanceBetweenLanes()
    {
        return Mathf.Abs((RightUpCorner.position.z - LeftDownCorner.position.z) / numberOfLanes);
    }

    public int getPlayerLane()
    {
        return Mathf.RoundToInt((player.position.z - LeftDownCorner.position.z)/distanceBetweenLanes);
    }
}
