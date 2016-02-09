using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    public bool punching;
    public SoundEffectManager SFX;
    public AudioClip returnProjectileSound;

    //void OnTriggerEnter(Collider c)
    //{
    //    if (c.CompareTag("Projectile"))
    //    {
    //        ProjectileScript Proj = c.GetComponent<ProjectileScript>();

    //        if (!Proj.checkIfBeingReturned())
    //        {
    //            if ((punching && Proj.canBePunched))
    //            {
    //                SFX.PlaySound(returnProjectileSound);
    //                Proj.changeDirection(transform.position);
    //            }
    //        }
    //    }

    //}

    void OnTriggerStay(Collider c)
    {
        if (c.CompareTag("Projectile"))
        {
            ProjectileScript Proj = c.GetComponent<ProjectileScript>();

            if (!Proj.checkIfBeingReturned())
            {
                if ((punching && Proj.canBePunched))
                {
                    SFX.PlaySound(returnProjectileSound);
                    Proj.changeDirection(transform.position);
                }
            }
        }

        if (c.CompareTag("Boulder"))
        {
            BoulderScript Boulder = c.GetComponentInParent<BoulderScript>();

            if (Boulder.punchable && punching)
                Boulder.PunchBoulder();
        }
    }

}
