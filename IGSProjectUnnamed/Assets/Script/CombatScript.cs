using UnityEngine;
using System.Collections;

public class CombatScript : MonoBehaviour {

    public int initialHealth = 100;
    public bool godMode = false;
    private int currentHealth;

    bool punching = false;
    bool canPunch = true;
    
    public float punchingTime = 0.5f;
    public float punchCooldown = 0.2f;
    public Color punchingColor;
    public Color damageColor;
    public Color chargingColor;
    public AudioClip receiveDamageSound;
    public AudioClip returnProjectileSound;
    public AudioClip punchSound;

    Color originalColor;
    public SpriteRenderer renderer;

    private GameManager manager;
    private SoundEffectManager SFX;

    private int burningDamage = 0;
    private bool burning = false;
    private float burnTaim = 0;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        originalColor = renderer.color;
        currentHealth = initialHealth;
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Projectile"))
        {
            ProjectileScript Proj = c.GetComponent<ProjectileScript>();

            if (!Proj.checkIfBeingReturned())
            {
                if ((punching && Proj.canBePunched) || (!punching && Proj.blockedByStanding))
                {
                    SFX.PlaySound(returnProjectileSound);
                    Proj.changeDirection(transform.position);
                }
                else
                {
                    receiveDamage(Proj.Damage);
                    StartCoroutine(damageColorChange());
                    Proj.DestroyProjectile();
                }
            }
        }

        if (c.CompareTag("Fire"))
        {
            burningDamage = c.GetComponent<fireScript>().damagePerSecond;
            burning = true;
        }
    }

    void OnTriggerStay(Collider c)
    {

        if (c.CompareTag("Punch Boulder"))
        {
            BoulderScript Boulder = c.GetComponentInParent<BoulderScript>();

            if (Boulder.punchable && punching)
                Boulder.PunchBoulder();
        }

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

        if (burning)
        {
            burnTaim += Time.deltaTime;

            if (burnTaim >= 1)
            {
                receiveDamage(burningDamage);
                burnTaim = 0;
            }
        }

        if (Input.GetButtonDown("Fire1") && canPunch)
        {
            StartCoroutine(punchStuff());
        }
	}

    IEnumerator punchStuff()
    {
        canPunch = false;
        punching = true;
        renderer.color = punchingColor;
        SFX.PlaySound(punchSound);
        yield return new WaitForSeconds(punchingTime);
        renderer.color = chargingColor;
        punching = false;
        yield return new WaitForSeconds(punchCooldown);
        renderer.color = originalColor;
        canPunch = true;
    }

    IEnumerator damageColorChange()
    {
        renderer.color = damageColor;
        yield return new WaitForSeconds(0.3f);
        
        if (canPunch)
            renderer.color = originalColor;
        else
            renderer.color = chargingColor;
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


}
