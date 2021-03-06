﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class floorClearedScreen : MonoBehaviour {

    public int floorNumber = 1;
    public int speedOfRecount = 50;
    public Text titleText;
    public Text timeText;
    public Text damageReceivedText;
    public Text[] otherTexts;
    public string surveyLink;
    private int damageReceived = 0;
    private float timeTaken = 0;

    levelManager highManager;
    int damageCount = 0;
    int recordDamage;
	
    void OnEnable()
    {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        //scaleSizes();
        titleText.text = "Floor " + floorNumber + "\nCleared!";
        getFinalStats();
        SaveLoad.Load();
        recordDamage = SaveLoad.savedGame.lessDamageReceivedByFloor[floorNumber - 1];
        transform.BroadcastMessage("setManager");

        if (timeTaken < SaveLoad.savedGame.bestTimes[floorNumber - 1])
        {
            //Debug.Log(SaveLoad.savedGame.bestTimes[floorNumber - 1]);
            timeText.text = turnIntoTime(timeTaken) + "\nNew Record!";
            SaveLoad.savedGame.setNewTimeRecord(timeTaken, floorNumber - 1);
            SaveLoad.Save();
        }
        else
            timeText.text = turnIntoTime(timeTaken);
    }

	void Update()
    {
        if (damageCount < damageReceived)
        {
            damageCount += Mathf.RoundToInt(speedOfRecount * Time.deltaTime);
            
            if (damageCount > damageReceived)
                damageCount = damageReceived;
        }

        damageReceivedText.text = damageCount + " HP";

        if (damageCount == damageReceived || recordDamage > damageReceived)
        {
            //Debug.Log(recordDamage);
            damageReceivedText.text += "\nNewRecord!";
            SaveLoad.savedGame.setDamageRecord(damageReceived, floorNumber - 1);
            SaveLoad.Save();
        }
        //Debug.Log("Working");
    }

    void getFinalStats()
    {
        damageReceived = highManager.getAccumulatedDamage();
        timeTaken = highManager.getTime();
    }

    string turnIntoTime(float num)
    {
        int minutes = Mathf.FloorToInt(num / 60);
        int seconds = Mathf.FloorToInt(num % 60);
        int miliseconds = Mathf.FloorToInt((num - Mathf.FloorToInt(num)) * 100);
        string result = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds;

        //if (minutes < 10)
        //    result += "0";

        //result += minutes + ":";

        //if (seconds < 10)
        //    result += "0";

        //result += seconds + ":";

        //if (miliseconds < 10)
        //    result += "0";

        //result += miliseconds;

        return result;
    }

    void scaleSizes()
    {
        for (int a = 0; a < otherTexts.Length; a++)
        {
            otherTexts[a].fontSize = otherTexts[a].fontSize * Screen.width / 560;
        }
        timeText.fontSize = timeText.fontSize * Screen.width / 560;
        timeText.rectTransform.position = timeText.rectTransform.position * Screen.width / 560;
        timeText.rectTransform.sizeDelta = timeText.rectTransform.sizeDelta * Screen.width / 560;
        damageReceivedText.fontSize = damageReceivedText.fontSize * Screen.width / 560;
        damageReceivedText.rectTransform.position = damageReceivedText.rectTransform.position * Screen.width / 560;
        damageReceivedText.rectTransform.sizeDelta = damageReceivedText.rectTransform.sizeDelta * Screen.width / 560;
        titleText.fontSize = titleText.fontSize * Screen.width / 560;
        titleText.rectTransform.position = titleText.rectTransform.position * Screen.width / 560;
        titleText.rectTransform.sizeDelta = titleText.rectTransform.sizeDelta * Screen.width / 560;
    }

    public void TakeToSurvey()
    {
        Application.OpenURL(surveyLink);
    }

    public void loadLevel(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
