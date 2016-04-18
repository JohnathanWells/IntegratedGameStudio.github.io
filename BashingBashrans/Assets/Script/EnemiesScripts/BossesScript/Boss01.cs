using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Boss01 : MonoBehaviour {

    public class Step
    {
        public int[] lane;
        public int[] type;
        public int[] muzzleNumber;

        public Step(int numberOfSimultaneousShots)
        {
            lane = new int[numberOfSimultaneousShots];
            type = new int[numberOfSimultaneousShots];
            muzzleNumber = new int[numberOfSimultaneousShots];
        }
        public void setStep(string text)
        {
            int length = text.Length;

            for (int a = 0; a < length; a += 2)
            {
                //Debug.Log("lane: " + text[a] + "\ntype: " + text[a + 1]);
                lane[a / 2] = (int)text[a] - 49;
                type[a / 2] = (int)text[a + 1] - 49;
                muzzleNumber[a / 2] = lane[a / 2];
            }
        }
    };
    public class Attack
    {
        public Step[] steps;
        public Attack(int numberOfSteps)
        {
            steps = new Step[numberOfSteps];
        }
    }
    [System.Serializable]
    public class phaseAttacks
    {
        public int[] AttacksForPhase;
    }

    //A step is simultaneous attacks. For example if the enemy shoots at lane 0 and 4 at the same time, that's a step.
    //Each step must be expressed between parenthesis. Each bullet has two values: A number and a letter. The number decides the lane it will be shooted at, the letter what type of projectile it is.
    [Header("Health Variables")]
    public int Health = 1000;
    public string Name = "Pedro";
    //public int Phase2StartsAt = 300;
    public List<int> phaseTriggers;
    private int currentHealth;

    [Header("UI")]
    public Text healthText;

    [Header("Attack Variables")]
    [TextArea(4, 20)]
    public string[] PatternsTexts;
    public cannonScript[] muzzles;
    public Attack[] attacks;
    public float[] cooldownsBetweenSteps;
    public phaseAttacks[] PhaseAttacks;
    public int attackToTest = 0;
    public bool testingAttacks = true;
    public float cooldownTime = 1;
    //Make private later
    public float currentCooldown = 0;
    public float currentStepCooldown = 0;
    public bool cooled = true;
    public bool stepCooled = true;
    private int countOfStep = 0;
    private int currentAttack;
    private bool dead = false;
    private bool inTrans = true;
    private int phase = 1;

    [Header("Animation")]
    public Animator animator;

    private GameManager manager;
    private levelManager highManager;

	void Awake () {
        attacks = new Attack[PatternsTexts.Length];
        currentHealth = Health;
        setListOfAttacks(attacks, PatternsTexts);

        //animator = GetComponent<Animator>();

        if (testingAttacks)
        {
            currentAttack = attackToTest;
        }
	}

    void Update()
    {
        if (!inTrans && !dead)
        {
            if (!cooled)
            {
                currentCooldown += Time.deltaTime;

                if (currentCooldown >= cooldownTime)
                {
                    cooled = true;
                    currentCooldown = 0;

                    if (!testingAttacks)
                    {
                        nextAttack();
                    }
                }
            }

            if (!stepCooled)
            {
                currentStepCooldown += Time.deltaTime;

                if (currentStepCooldown >= cooldownsBetweenSteps[currentAttack])
                {
                    stepCooled = true;
                    currentStepCooldown = 0;
                }
            }

            attackShooting(currentAttack);
        }
    }

    void OnGUI()
    {
        healthText.text = (Name +": " + currentHealth + "/" + Health);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Projectile"))
        {
            ProjectileScript proj = c.GetComponent<ProjectileScript>();

            if (proj.getBeingReturned())
            {
                ReceiveDamage(proj.Damage);
            }

            proj.projectileCrash(0);
        }

        if (c.CompareTag("Boulder"))
            boulderReceived(c.transform);
    }

    void nextAttack()
    {
        int nextAttack;
        phaseAttacks temp = PhaseAttacks[phase - 1];
        int lenght = temp.AttacksForPhase.Length;

        nextAttack = temp.AttacksForPhase[Random.Range(0, lenght)];

        if (nextAttack != currentAttack && lenght > 1)
            currentAttack = nextAttack;
        else
        {
            if (nextAttack == 0)
            {
                currentAttack++;
            }
            else
            {
                currentAttack--;
            }
        }
    }
    
    void shoot(int muzzleNum, int projType)
    {
        //Debug.Log("Muzzle: " + muzzleNum + "\nProj: " + projType);
        if (projType >= 0 && muzzleNum >= 0)
        {
            muzzles[muzzleNum].setCurrentAmmo(projType);
            muzzles[muzzleNum].SendMessage("Shoot");
        }
    }

    void attackShooting(int attackSelected)
    {
        if (cooled && stepCooled)
        {
            Step temp = attacks[attackSelected].steps[countOfStep];
            animator.Play("Attack" + (attackSelected + 1));
            //Debug.Log("Step number: " + countOfStep + "/" + attacks[attackToTest].steps.Length + "\nSimProject: " + temp.muzzleNumber.Length);

            for (int a = 0; a < temp.muzzleNumber.Length; a++)
            {
                //Debug.Log("Step: " + countOfStep + ", Bullet: " + (a + 1) + "/" + temp.muzzleNumber.Length + ", Muzzle: " + temp.muzzleNumber[a] + ", Type: " + temp.type[a]);
                shoot(temp.muzzleNumber[a], temp.type[a]);
            }

            countOfStep++;
            //Debug.Log("Step: " + countOfStep);
            stepCooled = false;

            if (countOfStep == attacks[attackSelected].steps.Length)
            {
                countOfStep = 0;
                cooled = false;
            }
        }
    }

    int getNumberOfSteps(string input)
    {
        int count = 0;
        int lenght = input.Length;

        for (int a = 0; a < lenght; a++)
        {
            if (input[a] == '(')
                count++;
        }

        //Debug.Log("Num of Steps: " + count);

        return count;
    }

    Attack setAttack(string text)
    {
        int lenght = getNumberOfSteps(text);
        string [] stTxt = chopText(text, lenght);
        Attack result = new Attack(lenght);

        for (int a = 0; a < lenght; a++)
        {
            //Debug.Log("textNum" + a + ": " + stTxt[a]);
            result.steps[a] = new Step(stTxt[a].Length/2);
            result.steps[a].setStep(stTxt[a]);
            //Debug.Log("attack step" + a + "shoots " + result.steps[a].type.Length + " @same time");
        }

        return result;
    }

    string[] chopText(string fullText, int numberOfSteps)
    {
        int start = 1;
        int end = 0;

        string[] result = new string[numberOfSteps];

        for (int a = 0; a < numberOfSteps; a++)
        {
            //count = 0;

            for (int b = end + 1; b < fullText.Length; b++)
            {
                //Debug.Log(end + "/" + fullText.Length);

                if (fullText[b] == '(' /*&& count == a*/)
                {
                    start = b + 1;
                    //Debug.Log(fullText[b + 1] + " " + fullText[b + 2]);
                }
                else if (fullText[b] == ')' /*&& count == a*/)
                {
                    end = b;
                    break;
                }
                //else if (fullText[b] == '(' && count != a)
                //{
                //    count++;
                //}
            }

            result[a] = fullText.Substring(start, end - start);
            //Debug.Log(result[a]);
        }

        return result;
    }

    void setListOfAttacks(Attack[] list, string[] texts)
    {
        int lenght = list.Length;
        for (int a = 0; a < lenght; a++)
        {
            list[a] = setAttack(texts[a]);
            //showArrayOfAttack(a);
        }
    }

    void showArrayOfAttack(int numberOfAttack)
    {
        Attack attack = attacks[numberOfAttack];
        Debug.Log("Number: " + numberOfAttack + "\nNumber of Steps: " + attack.steps.Length + "\nCooldown Between Steps: " + cooldownsBetweenSteps[numberOfAttack]);
    }

    public void setManager()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        inTrans = false;
        animator.SetInteger("Phase", 1);
    }

    public void ReceiveDamage(int damage)
    {
        //SFX.PlaySound(damageSound);
        animator.Play("Damage");

        currentHealth -= damage;
        
        if (phaseTriggers.Contains(currentHealth))
        {
            phase = phaseTriggers.IndexOf(currentHealth) + 1;
        }

        //if (currentHealth <= Phase2StartsAt)
        //{
        //    phase = 2;
        //}
        //else
        //{
        //    phase = 1;
        //}

        animator.SetInteger("Phase", phase);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            dead = true;

            animator.SetBool("Dead", true);
            Debug.Log("Enemy defeated");
            //DestroyTurret();
        }
    }

    public void inTransition(bool value)
    {
        inTrans = value;
    }

    void boulderReceived(Transform c)
    {
        BoulderScript bould = c.GetComponent<BoulderScript>();

        ReceiveDamage(bould.damage);
        bould.DestroyBoulder();
    }
}
