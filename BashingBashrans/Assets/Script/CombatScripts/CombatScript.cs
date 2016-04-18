using UnityEngine;
using System.Collections;

public class CombatScript : MonoBehaviour {

    [Header("Health")]
    public int initialHealth = 100;
    public bool godMode = false;
    public int recoverItems = 1;
    private int currentHealth;
    private bool dead = false;

    [Header("Punching")]
    public float punchingTime = 0.5f;
    public float punchCooldown = 0.2f;
    public Color punchingColor;
    public Color damageColor;
    public Color chargingColor;
    private bool canPunch = true;
    public WeaponScript weapon;

    [Header("Sounds")]
    public AudioClip receiveDamageSound;
    public AudioClip returnPassiveProjectileSound;
    public AudioClip punchSound;
    public AudioClip healSound;

    [Header("Particles")]
    public ParticleSystem healingParticles;

    [Header("Other Scripts Access")]
    public Animation animation;
    public PlayerMovement movementScript;
    private GameManager manager;
    private levelManager highManager;
    private SoundEffectManager SFX;
    private ParticleManager PM;

    [Header("Burns")]
    public float timeBetweenBurningDamage = 1;
    private int burningDamage = 0;
    private bool burning = false;
    private float burnTaim = 0;

    private bool inTransition = false;

    //[Header("Poison")]
    //public float minsick = 2;
    //public float maxsick = 4;
    //public float sick = 0;
    //public float ptime = 0;
    //public bool isp = false;

    public PlayerMovement pm;
    public Transform feet;
    public Transform sensors;

    [Header("Animation")]
    public Animator playerAnimator;
   public float painanimation;
    public float defeatanimation;
    public float winanimation;

    void Start () {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        playerAnimator = GameObject.FindGameObjectWithTag("PlayerModel").GetComponent<Animator>();
        PM = highManager.PM;

        SaveLoad.Load();
        recoverItems = SaveLoad.savedGame.healthKits;
        highManager.SendMessage("updateNumberOfItems", recoverItems);

        setManager();
        currentHealth = initialHealth;
	}

    void OnTriggerEnter(Collider c)
    {
        if (!inTransition && !dead)
        {
            if (c.CompareTag("Projectile"))
            {
                ProjectileScript Proj = c.GetComponent<ProjectileScript>();

                if (!Proj.getBeingReturned())
                {
                    playerAnimator.SetBool("Hurt", true);

                    receiveDamage(Proj.Damage);
                    Proj.projectileCrash(1);
                    StartCoroutine(Pain());


                    //if ((!weapon.getPunching() && Proj.blockedByStanding))
                    //{
                    //    SFX.PlaySound(returnPassiveProjectileSound);
                    //    Proj.changeDirection(Proj.rotateRelativelyToHit(transform.position));
                    //}
                    //else
                    //{
                    //    receiveDamage(Proj.Damage);
                    //    Proj.projectileCrash(1);
                    //}

                    //if (Proj.getEffectType() == "poison")
                    //{
                    //    sick = Random.Range(Proj.minSick, Proj.maxSick);
                    //    ptime = Random.Range(minsick, maxsick);
                    //    sick = (int)sick;
                    //    isp = true;
                    //}
                    
                    if (Proj.getEffectType() == "freeze")
                    {
                        movementScript.froze();
                    }
                }
            }

            if (c.CompareTag("Fire"))
            {
                burningDamage = c.GetComponent<fireScript>().damagePerSecond;
                burning = true;
            }
        }
    }

	void Update () {

        if (!dead && !highManager.getPaused())
        {
            //if (isp && ptime > 0)
            //{
            //    poison();
            //}

            if (Input.GetButtonDown("Swing Direction") && canPunch)
            {
                playerAnimator.SetBool("Swinging", true);
                StartCoroutine(punchStuff(-Mathf.RoundToInt(Input.GetAxisRaw("Swing Direction"))));
            }

            if (Input.GetButtonDown("UseRecover"))
            {
                useKit();
            }
        }
	}

    void OnTriggerStay(Collider c)
    {
        if (!dead)
        {
            if (c.CompareTag("Boulder"))
            {
                BoulderScript boulderProperties = c.GetComponent<BoulderScript>();

                receiveDamage(boulderProperties.damage);
                boulderProperties.DestroyBoulder();
            }

            if (c.tag == "Fire")
            {
                Burn();
            }
            else if (c.tag != "Fire" && burnTaim > 0)
            {
                burnTaim -= Time.deltaTime;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Fire") && !dead)
        {
            burning = false;
            burnTaim = 0;
        }
    }

    public void transitionHappening(bool value)
    {
        inTransition = value;
    }

    void Burn()
    {
        burnTaim += Time.deltaTime;

        if (burnTaim >= timeBetweenBurningDamage)
        {
            receiveDamage(burningDamage);
            burnTaim = 0;
        }
    }

    IEnumerator punchStuff(int direction)
    {
        rotateInDirection(direction);
        canPunch = false;
        
        movementScript.SendMessage("changeCanMove", false);
        weapon.SendMessage("changeSwingingVar", true);
        SFX.PlaySound(punchSound);
        yield return new WaitForSeconds(punchingTime);
        playerAnimator.SetBool("Swinging", false);
        weapon.SendMessage("changeSwingingVar", false);
        yield return new WaitForSeconds(punchCooldown);
        weapon.animationHappening = false;
        canPunch = true;
        movementScript.SendMessage("changeCanMove", true);
        rotateInDirection(-direction);
    }

    public int getHealth()
    {
        return currentHealth;
    }

    void receiveDamage(int damage)
    {
        currentHealth -= damage;
        highManager.SendMessage("accumulateDamage", damage);
        SFX.PlaySound(receiveDamageSound);

        //Debug.Log("HP: " + damage);

        if (currentHealth <= 0)
        {
            dead = true;
            
            currentHealth = 0;
            

			if (!godMode) {
				playerAnimator.SetBool("Alive", false);
				StartCoroutine(PlayerisKill());

			}
        }
    }

    private void rotateInDirection(int dir)
    {
        feet.Rotate(new Vector3(0, dir * 90, 0));
        sensors.Rotate(new Vector3(0, -dir * 90, 0));
    }

    public void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.getSFX();
        weapon.manager = manager;
        weapon.SendMessage("changeSoundEffectManager", SFX);
    }

    /*public void freeze(Collider a)
    {
        PlayerMovement plm = a.GetComponent<PlayerMovement>();
        while (freeztart <= freeztme)
        {
            freeztart += Time.deltaTime;
            plm.moveHorizontally(Input.GetAxisRaw("Horizontal") / freezeffct);
            plm.moveVertically((int) (Input.GetAxisRaw("Vertical") / freezeffct));
        }
        plm.moveHorizontally(Input.GetAxisRaw("Horizontal"));
        plm.moveVertically((int)(Input.GetAxisRaw("Vertical")));
    }*/

    /*public void poison()
    {
        while (Etime <= sick)
        {
            Etime += Time.deltaTime;
            currentHealth -= (int)(sick * sick);
        }
    }*/

    //public void poison()
    //{
    //    if (currentHealth <= 100)
    //    {
    //        currentHealth = 0;
    //    }
    //    else if (currentHealth > 100)
    //    {
    //        currentHealth -= (int)(sick * sick);
    //    }
    //    ptime -= (Time.deltaTime * 60);
    //    if (ptime <= 0)
    //    {
    //        isp = false;
    //    }
    //}

    IEnumerator Pain()
    {
        //Debug.Log("Begin");

        yield return new WaitForSeconds(painanimation);
        {
            playerAnimator.SetBool("Hurt", false);
        }
    }
	IEnumerator PlayerisKill()
	{
        movementScript.SendMessage("changeCanMove", false);
        yield return new WaitForSeconds (1);
		manager.GameOver();
		manager.SendMessage ("GameOver");
	}

    public void healPlayer()
    {
        currentHealth = initialHealth;
    }

    public void useKit()
    {
        if (recoverItems > 0)
        {
            recoverItems--;
            PM.spawnParticles(healingParticles, feet.position, healingParticles.duration);
            healPlayer();
            highManager.SendMessage("updateNumberOfItems", recoverItems);
            SFX.PlaySound(healSound);
        }
    }

    public void addKits(int newKits)
    {
        recoverItems += newKits;
        highManager.SendMessage("updateNumberOfItems", recoverItems);
    }
}
