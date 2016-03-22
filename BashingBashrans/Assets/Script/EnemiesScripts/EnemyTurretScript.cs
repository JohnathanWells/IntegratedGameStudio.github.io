using UnityEngine;
using System.Collections;

public class EnemyTurretScript : MonoBehaviour {

    [Header("Shooting")]
    public float cooldownTime = 1f;
    public float burstCooling = 0.5f;
    public int projectilesByBurst = 1;
    public bool randomBullets = false;
    public bool shootBurstInSingleLine = false;
    int shotsFired = 0;
    bool coolingDown = false;
    bool burstCooldown = false;
    float currentCool = 0;
    float burstCool = 0;
    int lane;
    bool canMove = false;

    [Header("Stats")]
    public int InitialHealth = 200;
    public string Name;
    public int currentHealth;
    public bool canReturnProjectiles = false;
    public int OddsAgaisntReturningProjectile = 1;
    public bool meelable = true;

    [Header("Movement")]
    public bool moveUpAndDown = false;
    public float speed = 1f;
    public int direction = 0;
    public float marginOFDisplacement = 0.1f;
    public int minLane = 0;
    public int maxLane = 4;
    int numberOfLanes;
    //public bool moveToTheLeft = false;
    //public float distanceTraveledBeforeDeath = 10f;
    //float initialXPos = 0;

    [Header("UI")]
    public GUIStyle enemyFont;
    public Rect UIName;
    public Vector2 offsetName;
    public Vector2 ruleOfThreeBasicResolution;
    Vector2 screenPositionOfText;
    Vector3 screenPos;

    [Header("Other Scripts")]
    public GameManager manager;
    SoundEffectManager SFX;
    ParticleManager PartM;
    //Transform projectileFolder;
    public cannonScript[] muzzles;

    [Header("Sounds and Particles")]
    public AudioClip damageSound;
    public AudioClip explosionSound;
    public AudioClip returnSound;
    public ParticleSystem explosion;

    int originalFontSize;
    Transform feet;
    float downCornerPos;
    float distanceBetweenLanes;

	void Start () {
        setManager();
        direction = Random.Range(0, 2) * 2 - 1;
        setBasics();
	}

    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Boulder"))
        {
            boulderReceived(c.transform);
        }

        if (c.CompareTag("Projectile"))
        {
            projectileReceived(c.transform);
        }
    }

	void Update () 
    {
        if (manager != null)
        {
            if (moveUpAndDown)
                turretMovement();

            simpleShooting();
        }
        else
            Debug.Log("MANAGER IS NULL");
	}

    void OnGUI()
    {
        //if (manager != null)
        //{
        //    screenPos = Camera.main.WorldToScreenPoint(transform.parent.position);
        //    screenPositionOfText = new Vector2(screenPos.x, screenPos.y);
        //    screenPositionOfText += offsetName;
        //    UIName.position = screenPositionOfText;
        //    GUI.Box(UIName, Name + "\n" + currentHealth + "/" + InitialHealth, enemyFont);
        //}
    }

    void turretMovement()
    {
        if (!shootBurstInSingleLine || (shootBurstInSingleLine && canMove))
        {
            float downCorner = manager.LeftDownCorner.position.z;
            float Max = downCorner + maxLane;
            float Min = downCorner + minLane;

            //Debug.Log(Max + "M, " + Min + "m, " + feet.position.z + "p, " + direction + "d");

            if (feet.position.z >= Max)
                direction = -1;
            else if (feet.position.z <= Min)
                direction = 1;

            feet.Translate(0, 0, speed * direction * Time.deltaTime);
        }
    }

    void shootCannons()
    {
        for (int a = 0; a < muzzles.Length; a++)
        {            
            muzzles[a].SendMessage("Shoot");
        }
    }

    void changeCannonAmmo()
    {
        for (int a = 0; a < muzzles.Length; a++)
        {
            if (randomBullets)
                muzzles[a].SendMessage("changeAmmo");
            else
                muzzles[a].SendMessage("nextAmmo");
        }
    }

    void simpleShooting()
    {
        if (coolingDown)
        {
            currentCool += Time.deltaTime;
            if (currentCool >= cooldownTime)
            {
                shotsFired = 0;

                changeCannonAmmo();
                coolingDown = false;
                currentCool = 0;
            }
        }

        if (burstCooldown)
        {
            burstCool += Time.deltaTime;
            if (burstCool >= burstCooling)
            {
                burstCooldown = false;
                burstCool = 0;
            }
        }

        //If its cooled down, it has cooled between burst, the condition for shootburstsingleline is fulfilled (or doesn't need the condition), is alligned or doesn't move up and down
        if (!coolingDown && !burstCooldown && ((moveUpAndDown && checkMarginOfErrorOFPosition(direction)) || !moveUpAndDown))
        {
            if (canMove && currentCool == 0)
                canMove = false;

            if (!shootBurstInSingleLine || (shootBurstInSingleLine && !canMove))
            {
                shootCannons();
                shotsFired++;

                if (shotsFired >= projectilesByBurst)
                {
                    coolingDown = true;
                    canMove = true;
                }
                else
                {
                    burstCooldown = true;
                }
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        SFX.PlaySound(damageSound);
        currentHealth -= damage;

        //Debug.Log("Received " + damage);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyTurret();
        }
    }

    void DestroyTurret()
    {
        PartM.spawnParticles(explosion, transform.position, explosion.duration);
        manager.SendMessage("addEnemyDestroyed");
        SFX.PlaySound(explosionSound);
        Destroy(gameObject);
    }

    int getHealth()
    {
        return currentHealth;
    }

    public bool getIfMeelable()
    {
        return meelable;
    }

    bool checkMarginOfErrorOFPosition(int dir)
    {
        if (dir > 0)
        {
            int nextLaneNum = Mathf.RoundToInt((feet.position.z - downCornerPos) / distanceBetweenLanes);
            float nextLanePos = (nextLaneNum * distanceBetweenLanes) + downCornerPos;

            if (feet.position.z >= nextLanePos - marginOFDisplacement)
            {
                feet.position = new Vector3(feet.position.x, feet.position.y, nextLanePos);
                //Debug.Log("Player: " + feet.position.z + "\n positionForshooting: " + nextLanePos);
                return true;
            }
            else
                return false;
        }
        else if (dir < 0)
        {
            int nextLaneNum = Mathf.FloorToInt((feet.position.z - downCornerPos) / distanceBetweenLanes);
            float nextLanePos = (nextLaneNum * distanceBetweenLanes) + downCornerPos;

            if (feet.position.z <= nextLanePos + marginOFDisplacement)
            {
                //Debug.Log("Player: " + feet.position.z + "\n positionForshooting: " + nextLanePos);
                feet.position = new Vector3(feet.position.x, feet.position.y, nextLanePos);
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    void projectileReceived(Transform c)
    {
        ProjectileScript properties = c.GetComponent<ProjectileScript>();

        if (properties.getBeingReturned())
        {
            if (canReturnProjectiles)
            {
                if (Random.Range(0, OddsAgaisntReturningProjectile) == 0)
                {
                    SFX.PlaySound(returnSound);
                    properties.EnemyReturnsAttack();
                }
                else
                {
                    ReceiveDamage(properties.Damage);
                    properties.projectileCrash(0);
                    Destroy(c.gameObject);
                }
            }
            else
            {
                ReceiveDamage(properties.Damage);
                properties.projectileCrash(0);
                Destroy(c.gameObject);
            }
        }
    }

    void boulderReceived(Transform c)
    {
        BoulderScript bould = c.GetComponent<BoulderScript>();

        ReceiveDamage(bould.damage);
        bould.DestroyBoulder();
    }
    
    void setBasics()
    {
        currentHealth = InitialHealth;
        feet = transform.parent;
        manager.SendMessage("addEnemyInLevel");
        //Debug.Log(Name + " Joined the session at " + feet.position);
        originalFontSize = enemyFont.fontSize;
        enemyFont.fontSize = Mathf.RoundToInt((Screen.width * originalFontSize) / ruleOfThreeBasicResolution.x);
        offsetName = new Vector2((Screen.width * offsetName.x) / ruleOfThreeBasicResolution.x, (Screen.height * offsetName.y) / ruleOfThreeBasicResolution.y);
    }

    public void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        PartM = manager.getPM();
        SFX = manager.getSFX();
        //projectileFolder = manager.ProjectilesFolder;
        lane = manager.obtainLane(transform.parent);
        numberOfLanes = manager.numberOfLanes - 1;
        downCornerPos = manager.LeftDownCorner.position.z;
        distanceBetweenLanes = manager.obtainDistanceBetweenLanes();
        if (maxLane > numberOfLanes)
            maxLane = numberOfLanes;
    }
}
