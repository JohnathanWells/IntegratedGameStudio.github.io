using UnityEngine;
using System.Collections;

public class EnemyTurretScript : MonoBehaviour {

    [Header("Shooting")]
    public float cooldownTime = 1f;
    public float burstCooling = 0.5f;
    public int projectilesByBurst = 1;
    public Vector3 offsetShooting;
    public Transform[] projectile;
    private int sizeOfArray = 1;
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
    public GUIStyle style;

    [Header("Movement")]
    public bool moveToTheLeft = false;
    public float speedToTheLeft = 1f;
    public float distanceTraveledBeforeDeath = 10f;
    float initialXPos = 0;

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

    [Header("UI")]
    public Rect HealthBox;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        PartM = manager.PM;
        SFX = manager.SFX;
        sizeOfArray = projectile.Length;
        projectileFolder = manager.ProjectilesFolder;
        currentHealth = InitialHealth;
        initialXPos = transform.position.x;
        lane = manager.getLane(transform.parent);
        //Debug.Log(Name + ": #" + lane);
        manager.lanesOccupied[lane] = true;
        HealthBox = new Rect(new Vector2(Screen.width * HealthBox.position.x / 551, (Screen.height * HealthBox.position.y / 310) + transform.position.z * (Screen.height * HealthBox.size.y / 310)), new Vector2(Screen.width * HealthBox.size.x / 551, Screen.height * HealthBox.size.y / 310));
        style.fontSize = (Screen.width * style.fontSize) / 551;
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

	void Update () {

        if (moveToTheLeft)
        {
            if (Mathf.Abs(transform.position.x - initialXPos) < distanceTraveledBeforeDeath)
            {
                transform.Translate(new Vector2(-speedToTheLeft * Time.deltaTime, 0));
            }
            else
            {
                Destroy(gameObject);
            }
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
        	
        if (!coolingDown && !burstCooldown)
        {
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
        GUI.Box(HealthBox, Name + "\n" + currentHealth + "/" + InitialHealth, style);
    }

    void Shoot()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        Transform shoot = Instantiate(projectile[currentAmmo], transform.position + offsetShooting, Quaternion.Euler(rot)) as Transform;
        shoot.parent = projectileFolder;
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
        manager.lanesOccupied[lane] = false;
        SFX.PlaySound(explosionSound);

        if (manager.enemiesInQueue[lane] > 0)
        {
            manager.spawnEnemy(-1, transform.parent.position);
            manager.enemiesInQueue[lane]--;
        }

        Destroy(gameObject);
    }
}
