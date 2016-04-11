using UnityEngine;
using System.Collections;

//public enum typeMovement { Horizontal, Vertical, Curvy, Bouncy }
public enum typeOfBomb { horizontalLineBomb, verticalLineBomb, crossBomb, areaBomb }
public enum directionOfLine { Right,Left,Both}

public class bombScript : MonoBehaviour {

    [Header("Movement")]
    public typeMovement movementType;
    public bool canBePunched = false;
    public float angleOfDesviation = 180;
    public Material desviationMaterial;
    public Material preExplosionMaterial;
    public MeshRenderer renderer;
    private bool beingReturned = false;
    private int lane = 0;
    private int numOfLanes;

    [Header("Damage, Halflife, Trayectory and Explosion")]
    public typeOfBomb bombType;
    public directionOfLine lineDirection;
    public conditionForDestruction condition;
    public float speed = 2;
    public float desviationSpeed = 4;
    public int lineRange = 4;
    public float halflife = 10;
    public float distanceCondition = 5;
    public float timeBetweenSubExplosions = 0;
    public Transform subExplosions;
    private Transform projectileFolder;
    private float floordistance = 1;
    private float lifeTime = 0;
    private float distanceTraveled = 0;
    private float originalSpeed;
    //private float distanceBetweenLanes = 1;
    private int switches = 0;
    private int switchesActivated = 0;
    private bool exploded = false;

    [Header("Particles and Sounds")]
    public ParticleSystem explosionParticle;
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
    ParticleManager PM;
    Material originalMaterial;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.getSFX();
        PM = manager.getPM();
        lane = manager.obtainLane(transform);
        numOfLanes = manager.numberOfLanes;
        floordistance = manager.obtainDistanceBetweenLanes();
        //distanceBetweenLanes = manager.obtainDistanceBetweenLanes();
        projectileFolder = manager.ProjectilesFolder;
        //Debug.Log("Its alive");
	}

	void Update () {

        if (!exploded)
            moveHorizontally();

        if (condition == conditionForDestruction.timed)
        {
            lifeTime += Time.deltaTime;

            if (lifeTime + 1f >= halflife && !playingSound)
            {
                renderer.material = preExplosionMaterial;
                SFX.PlaySound(beforeExplosionSound);
                playingSound = true;
            }

            if (lifeTime >= halflife && !exploded)
            {
                exploded = true;

                if (!playingSound)
                    SFX.PlaySound(explosionSound);

                explodeBomb();
            }
        }
        else if (condition == conditionForDestruction.distanceBased)
        {
            distanceTraveled += Time.deltaTime * speed;

            if (distanceTraveled >= distanceCondition && !exploded)
            {
                exploded = true;
                explodeBomb();
            }
        }

        if (exploded && switchesActivated == switches)
            Destroy(gameObject);
	}

    void explodeBomb()
    {
        //Debug.Log("Boom");
        SFX.PlaySound(explosionSound);
        PM.spawnParticles(explosionParticle, transform.position, explosionParticle.duration);

        if (bombType == typeOfBomb.areaBomb)
        {
            areaExplosion(transform.position);
        }
        else if (bombType == typeOfBomb.horizontalLineBomb)
        {
            horizontalLine();
        }
        else if (bombType == typeOfBomb.verticalLineBomb)
        {
            verticalLine();
        }
        else if (bombType == typeOfBomb.crossBomb)
        {
            horizontalLine();
            verticalLine();
        }
    }

    void horizontalLine()
    {
        Vector3[] direction = new Vector3[2];
        direction[0] = Vector3.zero;
        direction[1] = Vector3.zero;

        if (lineDirection == directionOfLine.Left)
        {
            direction[0] = new Vector3(-1, 0, 0);
            direction[1] = new Vector3(0, 0, 0);
        }
        else if (lineDirection == directionOfLine.Right)
        {
            direction[0] = new Vector3(1, 0, 0);
            direction[1] = new Vector3(0, 0, 0);
        }
        else
        {
            direction[0] = new Vector3(-1, 0, 0);
            direction[1] = new Vector3(1, 0, 0);
        }

        lineExplosion(transform.position, direction);
    }

    void verticalLine()
    {
        Vector3[] direction = new Vector3[2];

        if (lineDirection == directionOfLine.Left)
        {
            direction[0] = new Vector3(0, 0, -1);
            direction[1] = new Vector3(0, 0, 0);
        }
        else if (lineDirection == directionOfLine.Right)
        {
            direction[0] = new Vector3(0, 0, 1);
            direction[1] = new Vector3(0, 0, 0);
        }
        else
        {
            direction[0] = new Vector3(0, 0, -1);
            direction[1] = new Vector3(0, 0, 1);
        }

        lineExplosion(transform.position, direction);
    }

    //public void BombExplosion()
    //{
    //    SFX.PlaySound(explosionSound);
    //    if (transform.position.x + 1 <= manager.maxPos.x && transform.position.y + 1 <= manager.maxPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x + 1 <= manager.minPos.x && transform.position.y <= manager.maxPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x + 1 <= manager.maxPos.x && transform.position.y - 1 >= manager.minPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x + 1, transform.position.y - 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x <= manager.maxPos.x && transform.position.y - 1 >= manager.minPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x - 1 >= manager.minPos.x && transform.position.y - 1 >= manager.minPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x - 1, transform.position.y - 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x - 1 >= manager.minPos.x && transform.position.y >= manager.minPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x - 1 >= manager.minPos.x && transform.position.y + 1 <= manager.maxPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //    if (transform.position.x <= manager.maxPos.x && transform.position.y + 1 <= manager.maxPos.y)
    //    {
    //        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    //        manager.PM.spawnParticles(projectileCollision, transform.position, 1);
    //    }
    //}

    void areaExplosion(Vector3 center)
    {
        Quaternion angleOfSubs = Quaternion.Euler(subExplosions.eulerAngles);
        Transform[] explosions = new Transform[9];

        explosions[0] = Instantiate(subExplosions, center + Vector3.right * floordistance, angleOfSubs) as Transform;
        explosions[1] = Instantiate(subExplosions, center + Vector3.left * floordistance, angleOfSubs) as Transform;

        if (lane + 1 <= manager.numberOfLanes)
        {
            explosions[2] = Instantiate(subExplosions, center + Vector3.forward * floordistance, angleOfSubs) as Transform;
            explosions[3] = Instantiate(subExplosions, center + (Vector3.right + Vector3.forward) * floordistance, angleOfSubs) as Transform;
            explosions[4] = Instantiate(subExplosions, center + (Vector3.left + Vector3.forward) * floordistance, angleOfSubs) as Transform;
        }

        if (lane - 1 >= 0)
        {
            explosions[5] = Instantiate(subExplosions, center + (Vector3.right + Vector3.back) * floordistance, angleOfSubs) as Transform;
            explosions[6] = Instantiate(subExplosions, center + (Vector3.left + Vector3.back) * floordistance, angleOfSubs) as Transform;
            explosions[7] = Instantiate(subExplosions, center + Vector3.back * floordistance, angleOfSubs) as Transform;
        }

        explosions[8] = Instantiate(subExplosions, center, angleOfSubs) as Transform;

        for (int a = 0; a < 9; a++)
        {
            explosions[a].parent = projectileFolder;
        }

        Destroy(gameObject);
    }

    void lineExplosion(Vector3 center, Vector3[] directions)
    {
        int numberOfExplosionsA;

        if (bombType == typeOfBomb.horizontalLineBomb)
            numberOfExplosionsA = lineRange;
        else
            numberOfExplosionsA = numOfLanes - (lane + 1);
            

        int numberOfExplosionsB = lane;
        //Debug.Log(transform.name + "\n" + numOfLanes + " - " + lane++);
        //if (numberOfExplosionsA > lineRange)
        //{
        //    numberOfExplosionsA = lineRange;
        //}
        
        Transform subExplosion = Instantiate(subExplosions, (center), Quaternion.Euler(subExplosions.eulerAngles)) as Transform;
        subExplosion.parent = projectileFolder;

        for (int a = 0; a < 2; a++)
        {

            //Debug.Log(transform.name + ": " + a + "\n" + numberOfExplosionsA + "\n" + numberOfExplosionsB);

            if (directions[a].x > 0  && numberOfExplosionsA > 0)
            {
                switches++;
                StartCoroutine(simultaneousDirections(numberOfExplosionsA, directions[a], center));
            }
            else if (directions[a].x < 0 && numberOfExplosionsA > 0)
            {
                switches++;
                StartCoroutine(simultaneousDirections(numberOfExplosionsA, directions[a], center));
            }

            if (directions[a].z > 0 && numberOfExplosionsA > 0)
            {
                switches++;
                StartCoroutine(simultaneousDirections(numberOfExplosionsA, directions[a], center));
            }
            else if (directions[a].z < 0 && numberOfExplosionsB > 0)
            {
                switches++;
                StartCoroutine(simultaneousDirections(numberOfExplosionsB, directions[a], center));
            }
        }
    }

    IEnumerator simultaneousDirections(int numberOfExplosions, Vector3 direction, Vector3 center)
    {
        //print(direction);
        for (int a = 1; a <= numberOfExplosions; a++)
        {
            Transform subExplosion = Instantiate(subExplosions, (center + (direction * a * floordistance)), Quaternion.Euler(subExplosions.eulerAngles)) as Transform;
            subExplosion.parent = projectileFolder;
            yield return new WaitForSeconds(timeBetweenSubExplosions);
        }

        switchesActivated++;
    }

    public ParticleSystem getMuzzleParticles()
    {
        return muzzleParticles;
    }

    public AudioClip getShootingSounds()
    {
        return shootingSound;
    }

    void moveHorizontally()
    {
        int dir = -1;

        if (movementType == typeMovement.Horizontal)
        {
            transform.Translate(new Vector3(speed * dir * Time.deltaTime, 0, 0));
        }
    }

    public void changeDistanceCondition(float n)
    {
        distanceCondition = n;
    }
}
