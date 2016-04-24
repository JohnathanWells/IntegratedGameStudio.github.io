using UnityEngine;
using System.Collections;

public class ApendageScript : MonoBehaviour {

    public class AnimationsHP
    {
        public int health;
        public string animationName;
    };

    public int damageToBoss = 100;
    public int initialApendageHealth = 100;
    public string deathAnimationTriggerName;
    public Boss01 mainScript;
    public Animator animator;
    public AnimationsHP[] animations;
    public bool meelable = false;
    private int currentHealth;

	// Use this for initialization
	void Start () {
        currentHealth = initialApendageHealth;
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Projectile")
        {
            ProjectileScript temp = c.GetComponent<ProjectileScript>();
            if (temp.getBeingReturned())
            {
                Damage(temp.Damage);
                temp.projectileCrash(0);
            }
        }
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;

        //checkIfHealthTriggerIsAchieved(currentHealth);

        if (currentHealth <= 0)
        {
            mainScript.ReceiveDamage(damageToBoss);
            animator.SetBool(deathAnimationTriggerName, true);
        }
    }

    public bool getIfMeelable()
    {
        return meelable;
    }

    void checkIfHealthTriggerIsAchieved(int num)
    {
        int lenght = animations.Length;

        for (int a = 0; a < lenght; a++)
        {
            if (num == animations[a].health)
            {
                animator.Play(animations[a].animationName);
            }
        }
    }
}
