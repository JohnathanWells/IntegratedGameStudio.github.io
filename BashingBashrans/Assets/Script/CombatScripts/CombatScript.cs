using UnityEngine;
using System.Collections;

public class CombatScript : MonoBehaviour {

    [Header("Health")]
    public int initialHealth = 100;
    public bool godMode = false;
    private int currentHealth;

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

    [Header("Other Scripts Access")]
    public Animation animation;
    public PlayerMovement movementScript;
    private GameManager manager;
    private SoundEffectManager SFX;

    [Header("Burns")]
    public float timeBetweenBurningDamage = 1;
    private int burningDamage = 0;
    private bool burning = false;
    private float burnTaim = 0;

    private bool inTransition = false;

    [Header("Poison")]
    public float minsick = 2;
    public float maxsick = 4;
    public float sick = 0;
    public float ptime = 0;
    public bool isp = false;

    public PlayerMovement pm;
    public Transform feet;

	void Start () {
        setManager();
        currentHealth = initialHealth;
	}

    void OnTriggerEnter(Collider c)
    {
        if (!inTransition)
        {
            if (c.CompareTag("Projectile"))
            {
                ProjectileScript Proj = c.GetComponent<ProjectileScript>();

                if (!Proj.getBeingReturned())
                {
                    if ((!weapon.getPunching() && Proj.blockedByStanding))
                    {
                        SFX.PlaySound(returnPassiveProjectileSound);
                        Proj.changeDirection(Proj.rotateRelativelyToHit(transform.position));
                    }
                    else
                    {
                        receiveDamage(Proj.Damage);
                        Proj.projectileCrash();
                    }
                    if (Proj.getEffectType() == "poison")
                    {
                        sick = Random.Range(Proj.minSick, Proj.maxSick);
                        ptime = Random.Range(minsick, maxsick);
                        sick = (int)sick;
                        isp = true;
                    }
                    if (Proj.getEffectType() == "freeze")
                    {
                        movementScript.froze();
                    }
                    receiveDamage(Proj.Damage);
                    Proj.projectileCrash();
                }
            }

            if (c.CompareTag("Fire"))
            {
                burningDamage = c.GetComponent<fireScript>().damagePerSecond;
                burning = true;
            }
        }
        else
        {

        }
    }

    public void transitionHappening(bool value)
    {
        inTransition = value;
    }
    
    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Boulder"))
        {
            BoulderScript boulderProperties = c.GetComponent<BoulderScript>();
            
            receiveDamage(boulderProperties.damage);
            boulderProperties.DestroyBoulder();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Fire"))
        {
            burning = false;
            burnTaim = 0;
        }
    }

	void Update () {

        if (isp && ptime > 0)
        {
            poison();
        }
        if (burning)
        {
            Burn();
        }
        else if (!burning && burnTaim > 0)
        {
            burnTaim -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Swing Direction") && canPunch)
        {
            StartCoroutine(punchStuff(-Mathf.RoundToInt(Input.GetAxisRaw("Swing Direction"))));
        }
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
        SFX.PlaySound(receiveDamageSound);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (!godMode)
                manager.SendMessage("GameOver");
        }
    }

    private void rotateInDirection(int dir)
    {
        feet.Rotate(new Vector3(0, dir * 90, 0));
    }

    public void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
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

    public void poison()
    {
        if (currentHealth <= 100)
        {
            currentHealth = 0;
        }
        else if (currentHealth > 100)
        {
            currentHealth -= (int)(sick * sick);
        }
        ptime -= (Time.deltaTime * 60);
        if (ptime <= 0)
        {
            isp = false;
        }
    }
}
