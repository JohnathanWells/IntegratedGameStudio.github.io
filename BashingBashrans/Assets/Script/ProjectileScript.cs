using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    [Header("Desviation")]
    public bool canBePunched = true;
    public bool blockedByStanding = false;
    public float angleOfDesviation = 180;
    private bool beingReturned = false;
    private Vector2 pointOfOrigin;
    
    [Header("Damage, Halflife, Trayectory and Explosion")]
    public bool upAndDown = false;
    public int Damage = 100;
    private int originalDamage;
    public float speed = 2;
    public float desviationSpeed = 4;
    private float originalSpeed;
    public float halflife = 10;
    private float floordistance = 1;
    private float lifeTime = 0;
    public bool timedExplosion = false;
    public bool isBomb = false;
    public Transform subExplosions;

    [Header("Particles and Sounds")]
    public ParticleSystem projectileCollision;
    public AudioClip destructionSound;
    public AudioClip explosionSound;
    public AudioClip beforeExplosionSound;
    private bool playingSound = false;

    [Header("Other Elements")]
    GameManager manager;
    SoundEffectManager SFX;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        pointOfOrigin = transform.position;
        originalDamage = Damage;
        originalSpeed = speed;
        if (isBomb)
        {
           floordistance = manager.getDistanceBetweenLanes();
        }
	}
	
	void Update () {
        lifeTime += Time.deltaTime;

        if (lifeTime + 1f >= halflife && playingSound)
        {
            SFX.PlaySound(beforeExplosionSound);
            playingSound = true;
        }

        if (lifeTime >= halflife)
        {
            if (isBomb)
            {
                BombExplosion();
            }

            else if (timedExplosion)
            {
                SFX.PlaySound(explosionSound);
                generateExpansiveWave(transform.position);
            }
            
            Destroy(gameObject);
        }

        transform.Translate(new Vector2(-speed * Time.deltaTime, 0));
	}

    public bool checkIfBeingReturned()
    {
        return beingReturned;
    }

    public void changeDirection(Vector2 pointOfReturn)
    {
        transform.Rotate(new Vector3(0, angleOfDesviation, 0));
        beingReturned = true;
        canBePunched = false;
        speed = desviationSpeed;        
        Damage = Mathf.Abs(Mathf.RoundToInt(Damage * 4 / (pointOfReturn.x - pointOfOrigin.x)));
        lifeTime = 0;
    }

    void generateExpansiveWave(Vector2 mainExplosion)
    {
        int b = Mathf.RoundToInt(manager.minPos.y);
        int c = Mathf.RoundToInt(manager.maxPos.y);

        for (int a = b; a <= c; a++)
        {
            Transform exp = Instantiate(subExplosions, new Vector3(mainExplosion.x, transform.position.y, a), Quaternion.Euler(subExplosions.eulerAngles)) as Transform;
        }
    }

    int getOriginalDamage()
    {
        return originalDamage;
    }

    public void EnemyReturnsAttack()
    {
        transform.Rotate(new Vector3(angleOfDesviation, 0, 0));
        beingReturned = false;
        canBePunched = true;

        //Damage = Mathf.Abs(Mathf.RoundToInt( Damage * 3 / Vector2.Distance(pointOfReturn, pointOfOrigin)));
        Damage = originalDamage;
        speed = originalSpeed;
        lifeTime = 0;
    }

    public void DestroyProjectile()
    {
        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        SFX.PlaySound(destructionSound);
        Destroy(gameObject);
    }
    
    public void BombExplosion()
    {
        SFX.PlaySound(explosionSound);
        if (transform.position.x + 1 <= manager.maxPos.x && transform.position.y + 1 <= manager.maxPos.y)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x + 1 <= manager.minPos.x && transform.position.y <= manager.maxPos.y)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x + 1 <= manager.maxPos.x && transform.position.y - 1 >= manager.minPos.y)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y - 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x <= manager.maxPos.x && transform.position.y - 1 >= manager.minPos.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x - 1 >= manager.minPos.x && transform.position.y - 1 >= manager.minPos.y)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y - 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x - 1 >= manager.minPos.x && transform.position.y >= manager.minPos.y)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x - 1 >= manager.minPos.x && transform.position.y + 1 <= manager.maxPos.y)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
        if (transform.position.x <= manager.maxPos.x && transform.position.y + 1 <= manager.maxPos.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        }
    }
}
