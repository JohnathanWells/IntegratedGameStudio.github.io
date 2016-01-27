using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Transform player;
    public CombatScript PlayerCombat;
    public Transform ProjectilesFolder;
    public Transform LeftDownCorner;
    public Transform RightUpCorner;
    public ParticleManager PM;
    public SoundEffectManager SFX;
    public Vector2 minPos;
    public Vector2 maxPos;
    public AudioClip gameOverSound;

    bool paused = false;
    public bool gameOver = false;

    public GUIStyle style;
    private Vector2 HealthTextSize;

    private Transform []Enemies;
    public int numberOfLanes;

    public bool BouldersActivated = false;
    public bool randomBoulderFall = true;
    public Transform Boulder;
    public float cooldownBetweenTraps = 30f;
    private float trapCooldown = 0;

    void Start()
    {
        //Enemies = new Transform[numberOfLanes];
        HealthTextSize = new Vector2((Screen.width * 400) / 551, (Screen.height * 100) / 310);
        style.fontSize = style.fontSize * Screen.width/ 551;
        minPos = LeftDownCorner.position;
        maxPos = RightUpCorner.position;

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
        GUI.Box(new Rect(new Vector2(2, 2), HealthTextSize), "HP: " + PlayerCombat.getHealth(), style);

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

    public float getDistanceBetweenLanes()
    {
        float angle = LeftDownCorner.eulerAngles.x;

        if (angle != 0)
            return LeftDownCorner.parent.parent.lossyScale.y * Mathf.Sin(LeftDownCorner.eulerAngles.x);
        else
            return LeftDownCorner.parent.parent.lossyScale.y * angle;
    }
}
