using UnityEngine;
using System.Collections;

public class EnemyTurretScript : MonoBehaviour {

    [Header("Shooting")]
    public float cooldownTime = 1f;
    public float burstCooling = 0.5f;
    public int projectilesByBurst = 1;
    int shotsFired = 0;
    bool coolingDown = false;
    bool burstCooldown = false;
    float currentCool = 0;
    float burstCool = 0;
    int lane;

    [Header("Stats")]
    public int InitialHealth = 200;
    public string Name;
    public int currentHealth;
    public bool canReturnProjectiles = false;
    public int OddsAgaisntReturningProjectile = 1;

    [Header("Movement")]
    public bool moveUpAndDown = false;
    public float speed = 1f;
    public int direction = 0;
    public float marginOFDisplacement = 0.1f;
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
        if (manager != null)
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.parent.position);
            screenPositionOfText = new Vector2(screenPos.x, screenPos.y);
            screenPositionOfText += offsetName;
            UIName.position = screenPositionOfText;
            GUI.Box(UIName, Name + "\n" + currentHealth + "/" + InitialHealth, enemyFont);
        }
    }

    void turretMovement()
    {
        float downCorner = manager.LeftDownCorner.position.z;

        if (feet.position.z > numberOfLanes + downCorner)
            direction = -1;
        else if (feet.position.z <= downCorner)
            direction = 1;

        feet.Translate(0, 0, speed * direction * Time.deltaTime);
    }

    void shootCannons()
    {
        for (int a = 0; a < muzzles.Length; a++)
        {
            muzzles[a].SendMessage("Shoot");
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

        if (!coolingDown && !burstCooldown && ((moveUpAndDown && checkMarginOfErrorOFPosition()) || !moveUpAndDown))
        {
            shootCannons();
            shotsFired++;

            if (shotsFired >= projectilesByBurst)
            {
                coolingDown = true;
            }
            else
            {
                burstCooldown = true;
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        SFX.PlaySound(damageSound);
        currentHealth -= damage;
        
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

    bool checkMarginOfErrorOFPosition()
    {
        if ((direction > 0) && (Mathf.RoundToInt(feet.position.z) - marginOFDisplacement <= feet.position.z))
        {
            feet.position = new Vector3(feet.position.x, feet.position.y, Mathf.RoundToInt(feet.position.z));
            //Debug.Log((Mathf.RoundToInt(feet.position.z) - marginOFDisplacement));
            return true;
        }
        else if ((direction < 0) && (Mathf.FloorToInt(feet.position.z) + marginOFDisplacement >= feet.position.z))
        {
            //Debug.Log((Mathf.FloorToInt(feet.position.z) + marginOFDisplacement));
            feet.position = new Vector3(feet.position.x, feet.position.y, Mathf.FloorToInt(feet.position.z));
            return true;
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
                    properties.projectileCrash();
                }
            }
            else
            {
                ReceiveDamage(properties.Damage);
                properties.projectileCrash();
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
        PartM = manager.PM;
        SFX = manager.SFX;
        //projectileFolder = manager.ProjectilesFolder;
        lane = manager.obtainLane(transform.parent);
        numberOfLanes = manager.numberOfLanes - 1;
    }
}
