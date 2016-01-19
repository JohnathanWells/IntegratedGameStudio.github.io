using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Transform player;
    public CombatScript PlayerCombat;
    public Transform ProjectilesFolder;
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
    public int numberOfPossibleEnemies;

    public bool BouldersActivated = false;
    public Transform Boulder;
    public float cooldownBetweenTraps = 30f;
    private float trapCooldown = 0;

    void Start()
    {
        Enemies = new Transform[numberOfPossibleEnemies];
        HealthTextSize = new Vector2((Screen.width * 400) / 551, (Screen.height * 100) / 310);
        style.fontSize = style.fontSize * Screen.width/ 551;
    }

    void Update()
    {
        if (BouldersActivated)
        {
            trapCooldown += Time.deltaTime;
            if (trapCooldown >= cooldownBetweenTraps)
            {
                trapCooldown = 0;
                Transform trap = Instantiate(Boulder, new Vector3(Mathf.RoundToInt(Random.Range(minPos.x, maxPos.x)), Mathf.RoundToInt(Random.Range(minPos.y, maxPos.y)), 9), Quaternion.identity) as Transform;
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
}
