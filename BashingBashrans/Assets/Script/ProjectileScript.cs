using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    [Header("Desviation")]
    public bool canBePunched = true;
    public bool blockedByStanding = false;
    public float angleOfDesviation = 180;
    private bool beingReturned = false;
    private bool directionChanging = false;
    private Vector2 pointOfOrigin;
    
    [Header("Damage, Halflife, Trayectory and Explosion")]
    public bool upAndDown = false;
    public int Damage = 100;
    private int originalDamage;
    public float speed = 2;
    public float halflife = 10;
    private float lifeTime = 0;
    public bool timedExplosion = false;
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

        if (Mathf.Abs(angleOfDesviation) != 180)
            directionChanging = true;
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
            if (timedExplosion)
            {
                SFX.PlaySound(explosionSound);
                generateExpansiveWave(transform.position);
            }
            
            Destroy(gameObject);
        }

        transform.Translate(new Vector2(-speed * Time.deltaTime, 0));

        if (directionChanging)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
	}

    public bool checkIfBeingReturned()
    {
        return beingReturned;
    }

    public void changeDirection(Vector2 pointOfReturn)
    {
        transform.Rotate(new Vector3(0, 0, angleOfDesviation));
        beingReturned = true;
        
        canBePunched = false;
        
        //Damage = Mathf.Abs(Mathf.RoundToInt( Damage * 3 / Vector2.Distance(pointOfReturn, pointOfOrigin)));
        Damage = Mathf.Abs(Mathf.RoundToInt(Damage * 4 / (pointOfReturn.x - pointOfOrigin.x)));
        lifeTime = 0;
    }

    void generateExpansiveWave(Vector2 mainExplosion)
    {
        int b = Mathf.RoundToInt(manager.minPos.y);
        int c = Mathf.RoundToInt(manager.maxPos.y);

        for (int a = b; a <= c; a++)
        {
            Transform exp = Instantiate(subExplosions, new Vector3(mainExplosion.x, a, a), Quaternion.identity) as Transform;
        }
    }

    int getOriginalDamage()
    {
        return originalDamage;
    }

    public void EnemyReturnsAttack()
    {
        transform.Rotate(new Vector3(0, 0, angleOfDesviation));
        beingReturned = false;

        canBePunched = true;

        //Damage = Mathf.Abs(Mathf.RoundToInt( Damage * 3 / Vector2.Distance(pointOfReturn, pointOfOrigin)));
        Damage = originalDamage;
        lifeTime = 0;
    }

    public void DestroyProjectile()
    {
        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        SFX.PlaySound(destructionSound);
        Destroy(gameObject);
    }
}
