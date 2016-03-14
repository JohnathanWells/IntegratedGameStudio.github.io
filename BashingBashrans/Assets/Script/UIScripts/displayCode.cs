using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum typeOfDisplay {worldScreen, GUIText};

public enum conditionForDisplay { Time, Health, None};

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
    public bool[] requiredPreviouslyUnlockedLogs;
    public displayCode displayWithPriority;

    private string password;

    private levelManager highManager;
    private GameManager manager;

	void Start () {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();

        //if (displayType == typeOfDisplay.GUIText)
        //{
        //    textUI.text = 
        //}
        //else
        //{
        //    textMesh.text = highManager.getPassword(numberOfDisplayInFloor);
        //}
	}

    public bool shouldDisplay()
    {
        bool resultA;
        bool resultB;
        bool resultC;

        if (condition == conditionForDisplay.Time)
        {
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
        else
            resultA = true;

        if (requiresPreviousLogs)
        {
            resultB = checkUnlockedLogs();
        }
        else
            resultB = true;

        if (displayWithPriority != null && displayWithPriority.shouldDisplay())
            resultC = false;
        else
            resultC = true;

        if (resultA && resultB && resultC)
            return true;
        else
            return false;
    }

    bool checkUnlockedLogs()
    {
        SaveLoad.Load();
        bool[] temp = SaveLoad.savedGame.unlockedPasswords;
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
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        password = highManager.getPassword(numberOfDisplayInFloor);

        if (shouldDisplay())
        {
            if (displayType == typeOfDisplay.GUIText)
            {
                textUI.text = password;
            }
            else
            {
                textMesh.text = password;
            }
        }
    }
}
