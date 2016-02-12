using UnityEngine;
using System.Collections;

public class EnemyTurretScript : MonoBehaviour {

    [Header("Shooting")]
    public float cooldownTime = 1f;
    public float burstCooling = 0.5f;
    public int projectilesByBurst = 1;
    public Vector3 offsetShooting;
    public Transform[] possibleAmmo;
    private int sizeOfArray = 1;
    public bool shootMode = true;
    int shotsFired = 0;
    bool coolingDown = false;
    bool burstCooldown = false;
    float currentCool = 0;
    float burstCool = 0;
    int currentAmmo;
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
    GameManager manager;
    SoundEffectManager SFX;
    ParticleManager PartM;
    Transform projectileFolder;

    [Header("Sounds and Particles")]
    public AudioClip damageSound;
    public AudioClip explosionSound;
    public AudioClip returnSound;
    public ParticleSystem explosion;
    public Transform muzzle;
    public Vector3 flashOffset;

    ParticleSystem[] muzzleParticles;
    AudioClip[] ProjectilesSounds;
    int originalFontSize;
    Transform feet;
    int directionFacing;

	void Start () {
        direction = Random.Range(0, 2) * 2 - 1;
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        manager.SendMessage("addEnemyInLevel");
        PartM = manager.PM;
        SFX = manager.SFX;
        sizeOfArray = possibleAmmo.Length;
        projectileFolder = manager.ProjectilesFolder;
        currentHealth = InitialHealth;
        lane = manager.obtainLane(transform.parent);
        //manager.lanesOccupied[lane] = true;
        obtainPossibleMuzzleLights();
        feet = transform.parent;
        numberOfLanes = manager.numberOfLanes - 1;
        directionFacing = getDirectionFacing();
        offsetShooting.x *= directionFacing;
        originalFontSize = enemyFont.fontSize;
        enemyFont.fontSize = Mathf.RoundToInt((Screen.width * originalFontSize) / ruleOfThreeBasicResolution.x);
        offsetName = new Vector2((Screen.width * offsetName.x) / ruleOfThreeBasicResolution.x, (Screen.height * offsetName.y) / ruleOfThreeBasicResolution.y);
	}

    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Boulder"))
        {
            BoulderScript bould = c.GetComponent<BoulderScript>();

            ReceiveDamage(bould.damage);
            bould.DestroyBoulder();
        }

        if (c.CompareTag("Projectile"))
        {
            //Debug.Log("Turret received attack");

            ProjectileScript properties = c.GetComponent<ProjectileScript>();

            if (properties.checkIfBeingReturned())
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
                        properties.DestroyProjectile();
                    }
                }
                else
                {
                    ReceiveDamage(properties.Damage);
                    properties.DestroyProjectile();
                }
            }
        }
    }

	void Update () 
    {

        //if (moveToTheLeft)
        //{
        //    if (Mathf.Abs(transform.position.x - initialXPos) < distanceTraveledBeforeDeath)
        //    {
        //        transform.Translate(new Vector2(-speedToTheLeft * Time.deltaTime, 0));
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //

        if (moveUpAndDown)
        {
            if (feet.position.z > numberOfLanes)
                direction = -1;
            else if (feet.position.z <= 0)
                direction = 1;

            feet.Translate(0, 0, speed * direction * Time.deltaTime);
        }

        if (coolingDown)
        {
            currentCool += Time.deltaTime;
            if (currentCool >= cooldownTime)
            {
                shotsFired = 0;
                currentAmmo = Random.Range(0, sizeOfArray);
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
        	
        if (!coolingDown && !burstCooldown && checkMarginOfErrorOFPosition())
        {
            //Debug.Log("Shoot at: " + feet.position + "\nWith Direction: " + direction);
            Shoot();
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

    void OnGUI()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.parent.position);
        screenPositionOfText = new Vector2(screenPos.x, screenPos.y);
        screenPositionOfText += offsetName;
        UIName.position = screenPositionOfText;
        GUI.Box(UIName, Name + "\n" + currentHealth + "/" + InitialHealth, enemyFont);
    }

    void Shoot()
    {
        if (shootMode)
        {
            StartCoroutine(activateMuzzleLight(muzzleParticles[currentAmmo]));
            Vector3 rot = transform.rotation.eulerAngles;
            SFX.PlaySound(ProjectilesSounds[currentAmmo]);
            Transform shoot = Instantiate(possibleAmmo[currentAmmo], transform.position + offsetShooting, Quaternion.Euler(rot)) as Transform;
            shoot.parent = projectileFolder;
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
        //manager.lanesOccupied[lane] = false;
        SFX.PlaySound(explosionSound);

        //if (manager.enemiesInQueue[lane] > 0)
        //{
        //    manager.spawnEnemy(-1, transform.parent.position);
        //    manager.enemiesInQueue[lane]--;
        //}

        Destroy(gameObject);
    }

    void obtainPossibleMuzzleLights()
    {
        muzzleParticles = new ParticleSystem[sizeOfArray];
        ProjectilesSounds = new AudioClip[sizeOfArray];
        ProjectileScript temp;

        for (int n = 0; n < sizeOfArray; n++)
        {
            temp = possibleAmmo[n].GetComponent<ProjectileScript>();
            ParticleSystem part = Instantiate(temp.getMuzzleParticles(), Vector3.zero, Quaternion.Euler(new Vector3(0, -90, 0))) as ParticleSystem;
            ProjectilesSounds[n] = temp.getShootingSound();
            part.transform.parent = muzzle;
            part.transform.localPosition = flashOffset;
            part.gameObject.SetActive(false);
            muzzleParticles[n] = part;
            //Debug.Log("Part Assigned");
        }
    }

    IEnumerator activateMuzzleLight(ParticleSystem partSys)
    {
        //Debug.Log("CA = " + currentAmmo);
        partSys.gameObject.SetActive(true);
        partSys.time = 0;
        yield return new WaitForSeconds(partSys.duration);
        partSys.gameObject.SetActive(false);
    }

    int getHealth()
    {
        return currentHealth;
    }

    //void flipDirection()
    //{
    //    if (direction < 0)
    //        direction = 1;
    //    else
    //        direction = -1;
    //}

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

    int getDirectionFacing()
    {
        if (muzzle.position.x > feet.position.x)
            return 1;
        else
            return -1;
    }
}
