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

        string [] dir = Directory.GetFiles(screenshotDirectory);
        existingScreenshots = new List<string>(dir);
    }

    public static string saveNewScreenshot()
    {
        string defaultName = "Security_Picture_";
        
        for (int a = 0; a < overflow; a++)
        {
            if (!existingScreenshots.Contains(defaultName + a + ".png"))
            {
                Application.CaptureScreenshot(screenshotDirectory + "/" + defaultName + "" + a + ".png");
                existingScreenshots.Add(defaultName + "" + a + ".png");
                return defaultName + "" + a;
            }
        }

        //if the overflown is met
        return "ERROR";
    }
}
