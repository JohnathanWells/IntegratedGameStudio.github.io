using UnityEngine;
using System.Collections;

public class Wall_Script : MonoBehaviour {

    //float bounceCooldown = 0;
    //const float bc = 0.1f;
    //const float maxbc = 1f;

    //void Update()
    //{
    //    if (bounceCooldown >= maxbc)
    //    {
    //        bounceCooldown += Time.deltaTime;
    //    }
    //}

    public void OnTriggerEnter(Collider w)
    {
        if (w.CompareTag("Projectile"))
        {
            ProjectileScript pr = w.GetComponent<ProjectileScript>();
            if (pr.movement == typeMovement.Bouncy)
            {
            //    if (bounceCooldown >= bc)
            //    {
                    pr.bouncy();
                //    bounceCooldown = 0;
                //}
            }
            else
                pr.SendMessage("projectileCrash", 2);
        }
    }
}
