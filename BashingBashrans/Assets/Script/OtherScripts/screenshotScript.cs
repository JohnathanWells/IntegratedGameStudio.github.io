using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class screenshotScript{

    static List<string> existingScreenshots;
    static string screenshotDirectory = Application.dataPath + "/Screenshots";
    static int overflow = 1000;

    //void Start()
    //{
    //    getScreenshotsInDirectory();
    //    Debug.Log(existingScreenshots);
    //}

    public static void getScreenshotsInDirectory()
    {
        if (!Directory.Exists(screenshotDirectory))
            Directory.CreateDirectory(screenshotDirectory);

        existingScreenshots = new List<string>(Directory.GetFiles(screenshotDirectory));
        //Debug.Log(existingScreenshots[0]);
    }

    public static string saveNewScreenshot()
    {
        string defaultName = "Security_Picture_";
        string tempName;
        
        for (int a = 0; a < overflow; a++)
        {
            tempName = screenshotDirectory + "\\" + defaultName + a + ".png";
            //Debug.Log(tempName);
            if (!existingScreenshots.Contains(tempName))
            {
                Application.CaptureScreenshot(tempName);
                existingScreenshots.Add(tempName);
                return defaultName + "" + a;
            }
        }

        //if the overflown is met
        return "ERROR";
    }
}
