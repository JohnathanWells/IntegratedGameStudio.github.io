using UnityEngine;
using System.Collections;

public class EnemyTurretScript : MonoBehaviour {

    [Header("Shooting")]
    public float cooldownTime = 1f;
    public float burstCooling = 0.5f;
    public int projectilesByBurst = 1;
    public Transform[] projectile;
    public int sizeOfArray = 1;
    int shotsFired = 0;
    bool coolingDown = false;
    bool burstCooldown = false;
    float currentCool = 0;
    float burstCool = 0;
    int currentAmmo;

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

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        PartM = manager.PM;
        SFX = manager.SFX;
        projectileFolder = manager.ProjectilesFolder;
        currentHealth = InitialHealth;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        initialXPos = transform.position.x;
	}

    void OnTriggerEnter(Collider c)
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
                currentAmmo = Random.Range(0, sizeOfArray - 1);
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
        	
        if (!coolingDown && !burstCooldown && Random.Range(0, 2) == 0)
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
        GUI.Box(new Rect(new Vector2(80 * -transform.position.y + 10, Screen.height * (1.75f/2)), new Vector2(400, 800)), Name + "\n" + currentHealth + "/" + InitialHealth, style);
    }

    void Shoot()
    {
        Transform shoot = Instantiate(projectile[currentAmmo], transform.position, Quaternion.identity) as Transform;
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
        PartM.spawnParticles(explosion, new Vector3(transform.position.x, transform.position.y, -9.9f), explosion.duration);
        //manager.PlayerCombat.addKillCount();
        SFX.PlaySound(explosionSound);
        Destroy(gameObject);
    }

    //public void AreYouAlive()
    //{
    //    manager.IAmAlive();
    //}
}
