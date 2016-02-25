using UnityEngine;
using System.Collections;

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

    //A step is simultaneous attacks. For example if the enemy shoots at lane 0 and 4 at the same time, that's a step.
    //Each step must be expressed between parenthesis. Each bullet has two values: A number and a letter. The number decides the lane it will be shooted at, the letter what type of projectile it is.
    public string[] PatternsTexts;
    public cannonScript[] muzzles;
    public Attack[] attacks;
    public int attackToTest = 0;
    public float cooldownTime = 1;
    public float cooldownBetweenSteps = 1;
    //Make private later
    public float currentCooldown = 0;
    public float currentStepCooldown = 0;
    public bool cooled = true;
    public bool stepCooled = true;
    private int countOfStep = 0;

	void Awake () {
        attacks = new Attack[PatternsTexts.Length];
        setListOfAttacks(attacks, PatternsTexts);
	}

    void Update()
    {
        if (!cooled)
        {
            currentCooldown += Time.deltaTime;

            if (currentCooldown >= cooldownTime)
            {
                cooled = true;
                currentCooldown = 0;
            }
        }

        if (!stepCooled)
        {
            currentStepCooldown += Time.deltaTime;

            if (currentStepCooldown >= cooldownBetweenSteps)
            {
                stepCooled = true;
                currentStepCooldown = 0;
            }
        }

        testShooting();

        if (countOfStep > 13)
        {
            cooled = false;
            countOfStep = 0;
        }
    }

    void shoot(int muzzleNum, int projType)
    {
        //Debug.Log("Muzzle: " + muzzleNum + "\nProj: " + projType);
        muzzles[muzzleNum].setCurrentAmmo(projType);
        muzzles[muzzleNum].SendMessage("Shoot");
    }

    void testShooting()
    {
        if (cooled && stepCooled)
        {
            Step temp = attacks[attackToTest].steps[countOfStep];
            //Debug.Log("Step number: " + countOfStep + "/" + attacks[attackToTest].steps.Length + "\nSimProject: " + temp.muzzleNumber.Length);

            for (int a = 0; a < temp.muzzleNumber.Length; a++)
            {
                Debug.Log("Step: " + countOfStep + ", Bullet: " + (a + 1) + "/" + temp.muzzleNumber.Length + ", Muzzle: " + temp.muzzleNumber[a] + ", Type: " + temp.type[a]);
                shoot(temp.muzzleNumber[a], temp.type[a]);
            }

            countOfStep++;
            //Debug.Log("Step: " + countOfStep);
            stepCooled = false;
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

        return count;
    }

    Attack setAttack(string text)
    {
        int lenght = getNumberOfSteps(text);
        string [] stTxt = chopText(text, lenght);
        Attack result = new Attack(lenght);

        for (int a = 0; a < lenght; a++)
        {
            Debug.Log("textNum" + a + ": " + stTxt[a]);
            result.steps[a] = new Step(stTxt[a].Length/2);
            result.steps[a].setStep(stTxt[a]);
            //Debug.Log("attack step" + a + "shoots " + result.steps[a].type.Length + " @same time");
            //showArrayOfAttack(result);
        }

        //showArrayOfAttack(result);

        return result;
    }

    string[] chopText(string fullText, int numberOfSteps)
    {
        int start = 1;
        int end = 0;
        int count = 0;

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
        }

        //showArrayOfAttack(list[attackToTest]);
    }

    void showArrayOfAttack(Attack attack)
    {
        for (int a = 0; a < attack.steps.Length; a++)
        {
            Debug.Log("Step: " + a + ", Muzzle: " + attack.steps[a].muzzleNumber + ", Proj: " + attack.steps[a].type);
        }
    }
}
