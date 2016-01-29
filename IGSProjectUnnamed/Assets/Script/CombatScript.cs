using UnityEngine;
using System.Collections;

public class CombatScript : MonoBehaviour {

    public int initialHealth = 100;
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

    

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        originalColor = renderer.color;
        currentHealth = initialHealth;
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Projectile"))
        {
            //Debug.Log("Projectile here");
            ProjectileScript Proj = c.GetComponent<ProjectileScript>();

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

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Health Boost"))
        {
            PowerUp_Script PUP = c.GetComponent<PowerUp_Script>();
            if (currentHealth <= 900)
            {
                currentHealth += 100;
            }
            else
            {
                currentHealth += (initialHealth - currentHealth);
            }
            PUP.DestroyPowerUp();
        }
    }

    void OnTriggerStay2D(Collider2D c)
    {
        
        if(c.CompareTag("Boulder"))
        {
            BoulderScript boulderProperties = c.GetComponent<BoulderScript>();

            if (boulderProperties.harmful && c.transform.position.y > transform.position.y)
            {
                receiveDamage(boulderProperties.damage);
                boulderProperties.DestroyBoulder();
            }
        }

        if(c.CompareTag("Punch Boulder"))
        {
            BoulderScript Boulder = c.GetComponentInParent<BoulderScript>();

            if (Boulder.punchable && punching)
                Boulder.PunchBoulder();
        }
    }

	void Update () {
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
            manager.SendMessage("GameOver");
        }
    }
}
