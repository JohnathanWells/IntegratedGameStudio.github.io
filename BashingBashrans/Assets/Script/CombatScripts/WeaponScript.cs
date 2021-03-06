﻿using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    private bool punching;
    public SoundEffectManager SFX;
    public AudioClip returnProjectileSound;
    public AudioClip meleeAttackSound;
    public AudioClip failedMeleeAttackSound;
    public int damage = 100;
    public bool animationHappening = false;

    //private bool swinging = false;
    public GameManager manager;

    void OnTriggerStay(Collider c)
    {
        if (punching)
        {
            if (c.CompareTag("Projectile"))
            {
                //Debug.Log("Nothing will happen");
                ProjectileScript Proj = c.GetComponent<ProjectileScript>();

                if (!Proj.getBeingReturned())
                {
                    if (Proj.canBePunched)
                    {
                        SFX.PlaySound(returnProjectileSound);
                        //Debug.Log(transform.position.x + " vs " + c.transform.position.x);
                        Proj.changeDirection(Proj.rotateRelativelyToHit(transform.position));
                    }
                }
            }

            if (c.CompareTag("Enemy") && !animationHappening)
            {
                EnemyTurretScript temp = c.GetComponent<EnemyTurretScript>();
                ApendageScript Temp = c.GetComponent<ApendageScript>();
                Debug.Log(Temp == null);

                if (temp != null)
                    meleeEnemy(temp);
                
                if (Temp != null)
                    meleeEnemy(Temp);
            }

            if (c.CompareTag("Boulder"))
            {
                BoulderScript Boulder = c.GetComponentInParent<BoulderScript>();

                if (Boulder.punchable)
                    Boulder.PunchBoulder(obtainDirectionHorizontal(c.transform.position));
            }
        }
    }

    int obtainDirectionHorizontal(Vector3 projPos)
    {
        if (projPos.x > transform.parent.position.x)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public void changeSwingingVar(bool val)
    {
        punching = val;
    }

    public bool getPunching()
    {
        return punching;
    }

    void meleeEnemy(EnemyTurretScript enemy)
    {
        if (enemy.getIfMeelable())
        {
            SFX.PlaySound(meleeAttackSound);
            enemy.SendMessage("ReceiveDamage", damage);
            animationHappening = true;
        }
        else
        {
            SFX.PlaySound(failedMeleeAttackSound);
            animationHappening = true;
        }
    }

    void meleeEnemy(ApendageScript enemy)
    {
        if (enemy.getIfMeelable())
        {
            SFX.PlaySound(meleeAttackSound);
            enemy.SendMessage("Damage", damage);
            animationHappening = true;
        }
        else
        {
            SFX.PlaySound(failedMeleeAttackSound);
            animationHappening = true;
        }
    }

    public void changeSoundEffectManager(SoundEffectManager newSFX)
    {
        SFX = newSFX;
    }
}
