using UnityEngine;
using System.Collections;

//Curvy and Bouncy still do nothing
public enum typeMovement { Horizontal, Vertical, Curvy, Bouncy }
public enum movementDirection { left, right};
//The effects still do nothing
public enum Effect { fragmented, fire, poison, freeze }
public enum conditionForDestruction { timed, distanceBased }

public class ProjectileScript : MonoBehaviour
{

    #region variables
    [Header("Movement")]
    public typeMovement movement;
    public movementDirection direction;
    public bool canBePunched = true;
    public bool blockedByStanding = false;
    public float frequencyOfCurve;
    public float angleOfBounciness;
    public float angleOfDesviation = 180;
    public float speed = 2;
    public float desviationSpeed = 4;
    private float originalSpeed;
    private bool beingReturned = false;
    public Material desviationMaterial;
    public MeshRenderer renderer;
    private int directionOfProjectile;

    [Header("Damage, Halflife, Trayectory and Explosion")]
    public Effect effectOfProjectile;
    public conditionForDestruction condition;
    public int Damage = 100;
    private int originalDamage;
    public float halflife = 10;
    private float distanceForDestruction = 10;
    private float floordistance = 1;
    private float lifeTime = 0;
    private float distanceTraveled = 0;

    [Header("Particles and Sounds")]
    public ParticleSystem projectileCollision;
    public ParticleSystem muzzleParticles;
    public AudioClip shootingSound;
    public AudioClip destructionSound;
    public AudioClip explosionSound;
    public AudioClip beforeExplosionSound;
    private bool playingSound = false;

    [Header("Other Elements")]
    GameManager manager;
    SoundEffectManager SFX;
    Material originalMaterial;
    #endregion

    void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        originalDamage = Damage;
        originalSpeed = speed;
        originalMaterial = renderer.material;

        if (direction == movementDirection.left)
            directionOfProjectile = -1;
        else
            directionOfProjectile = 1;
	}
	
	void Update () 
    {
        if (condition == conditionForDestruction.timed)
        {
            lifeTime += Time.deltaTime;

            if (lifeTime >= halflife)
                Destroy(gameObject);
        }
        else
        {
            distanceTraveled += Time.deltaTime * speed;

            if (distanceTraveled >= distanceForDestruction)
                Destroy(gameObject);
        }


        projectileMovement();
	}
    
    void projectileMovement()
    {
        Vector2 translation = Vector2.zero;

        if (movement == typeMovement.Horizontal)
        {
            translation = new Vector2(directionOfProjectile * speed * Time.deltaTime, 0);
        }
        else if (movement == typeMovement.Vertical)
        {
            translation = new Vector2(0, directionOfProjectile * speed * Time.deltaTime);
        }

        transform.Translate(translation);
    }

    public void changeDirection(float angleOfReturn)
    {
        transform.Rotate(new Vector3(0, angleOfReturn, 0));
        beingReturned = true;
        canBePunched = false;
        speed = desviationSpeed;        
        //Damage = getNewDamage(pointOfReturn);
        lifeTime = 0;
        switchColors();
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

    public void switchColors()
    {
        if (beingReturned)
        {
            renderer.material = desviationMaterial;
        }
        else
        {
            renderer.material = originalMaterial;
        }
    }

    public void projectileCrash()
    {
        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
        SFX.PlaySound(destructionSound);
        Destroy(gameObject);
    }

    //Get Functions
    public ParticleSystem getMuzzleParticles()
    {
        return muzzleParticles;
    }

    public AudioClip getShootingSound()
    {
        return shootingSound;
    }

    int getOriginalDamage()
    {
        return originalDamage;
    }

    public bool getBeingReturned()
    {
        return beingReturned;
    }

    public float rotateRelativelyToHit(Vector3 hitPos)
    {
        if (direction == movementDirection.left)
        {
            if ((transform.position.x > hitPos.x && (transform.eulerAngles.y > -90 && transform.eulerAngles.y <= 90)) || (hitPos.x > transform.position.x && (transform.eulerAngles.y <= 270 && transform.eulerAngles.y > 90)))
                return angleOfDesviation;
            else
                return 0;
        }
        else if ((hitPos.x < transform.position.x && direction == movementDirection.right && (transform.eulerAngles.z <= -90 && transform.eulerAngles.z > 90)) || (hitPos.x > transform.position.x && direction == movementDirection.right && (transform.eulerAngles.z <= -90 && transform.eulerAngles.z > 90)))
        {
            return angleOfDesviation;
        }
        else
            return 0;
    }

    //private float obtainNewDamage(Vector2 pointOfReturn)
    //{
    //    return Mathf.Abs(Mathf.RoundToInt(Damage * 4 / (pointOfReturn.x - pointOfOrigin.x)));
    //}
}
