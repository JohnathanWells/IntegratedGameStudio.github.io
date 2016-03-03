using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    private bool punching;
    public SoundEffectManager SFX;
    public AudioClip returnProjectileSound;
    public AudioClip meleeAttackSound;
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
                        
                        Proj.changeDirection(Proj.rotateRelativelyToHit(transform.position));
                    }
                }
            }

            if (c.CompareTag("Enemy") && !animationHappening)
            {
                meleeEnemy(c.GetComponent<EnemyTurretScript>());
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
        SFX.PlaySound(meleeAttackSound);
        enemy.SendMessage("ReceiveDamage", damage);
        animationHappening = true;
    }
}
