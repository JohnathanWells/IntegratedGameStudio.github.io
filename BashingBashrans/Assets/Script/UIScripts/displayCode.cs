using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum typeOfDisplay {worldScreen, GUIText};

public enum conditionForDisplay { Time, Health, None, Both};

public class displayCode : MonoBehaviour {

    public int numberOfDisplayInFloor;
    public TextMesh textMesh;
    public Text textUI;
    public typeOfDisplay displayType;
    public conditionForDisplay condition;
    public bool requiresPreviousLogs = false;

    [Header("Conditions")]
    public float timeCondition;
    public int healthCondition;
    public bool[] requiredPreviouslyUnlockedLogs = new bool[11];
    public displayCode displayWithPriority;
    public bool ACTIVE = true;

    private string password;

    private levelManager highManager;
    //private GameManager manager;

    public bool shouldDisplay()
    {
        bool resultA;
        bool resultB;
        bool resultC;

        if (condition == conditionForDisplay.Time)
        {
            Debug.Log(highManager.getTime() + ": " + timeCondition);
            if (timeCondition >= highManager.getTime())
            {
                resultA = true;
            }
            else
                resultA = false;
        }
        else if (condition == conditionForDisplay.Health)
        {
            if (healthCondition <= highManager.getPlayerHealth())
            {
                resultA = true;
            }
            else
                resultA = false;
        }
        else if (condition == conditionForDisplay.Both)
        {
            if (healthCondition <= highManager.getPlayerHealth() && timeCondition >= highManager.getTime())
            {
                resultA = true;
            }
            else
                resultA = false;
        }
        else
            resultA = true;

        if (requiresPreviousLogs)
        {
            resultB = checkUnlockedLogs();
        }
        else
            resultB = true;

        if (displayWithPriority != null && displayWithPriority.shouldDisplay())
        {
            resultC = false;
            displayWithPriority.ACTIVATE();
            //displayWithPriority.setManager();
        }
        else
            resultC = true;

        //Debug.Log("Conditions for " + numberOfDisplayInFloor + " are " + resultA + ", " + resultB + ", " + resultC + ", " + ACTIVE);

        if (resultA && resultB && resultC)
            return true;
        else
            return false;
    }

    bool checkUnlockedLogs()
    {
        SaveLoad.Load();
        bool[] temp = SaveLoad.savedGame.returnUnlockedPasswords();
        int size = temp.Length;

        for (int a = 0; a < size; a++)
        {
            //If that log is not unlocked, check if its required. 
            //If it is, then the condition is not fulfilled. If the array is all checked and there is no conflic, then the requires logs are unlocked
            if (!temp[a])
            {
                if (requiredPreviouslyUnlockedLogs[a])
                    return false;
            }
        }

        return true;
    }

    public void setManager()
    {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        //manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        password = highManager.getPassword(numberOfDisplayInFloor - 1);

        if (ACTIVE && shouldDisplay())
        {
            if (displayType == typeOfDisplay.GUIText)
            {
                textUI.text = passwordOnDisplay(password);
            }
            else
            {
                textMesh.text = passwordOnDisplay(password);
            }
        }
    }

    public void ACTIVATE()
    {
        ACTIVE = true;
        setManager();
    }

    string passwordOnDisplay(string input)
    {
        string result = "";
        int l = input.Length;

        for (int a = 0; a < l; a++)
        {
            if (a != 0 && a % 3 == 0)
                result += "-";

            result += input[a];
        }

        return result;
    }
}
