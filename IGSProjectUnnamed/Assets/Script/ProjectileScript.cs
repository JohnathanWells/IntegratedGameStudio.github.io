﻿using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    public bool canBePunched = true;
    public bool blockedByStanding = false;
    public bool timedExplosion = false;
    public float angleOfDesviation = 180;
    public int Damage = 100;
    public float speed = 2;
    public float halflife = 10;
    public Transform subExplosions;
    public int ExpWavePointA = 0;
    public int ExpWavePointB = 5;
    public ParticleSystem projectileCollision;
    public AudioClip destructionSound;
    public AudioClip explosionSound;
    public AudioClip beforeExplosionSound;

    private float lifeTime = 0;
    private bool beingReturned = false;
    private Vector2 pointOfOrigin;
    private int originalDamage;
    private bool playingSound = false;

    GameManager manager;
    SoundEffectManager SFX;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        pointOfOrigin = transform.position;
        originalDamage = Damage;
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
        for (int a = ExpWavePointA; a <= ExpWavePointB; a++)
        {
            Transform exp = Instantiate(subExplosions, new Vector2(mainExplosion.x, a), Quaternion.identity) as Transform;
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
