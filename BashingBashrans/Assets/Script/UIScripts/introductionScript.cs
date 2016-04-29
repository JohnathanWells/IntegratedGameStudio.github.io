using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class introductionScript : MonoBehaviour {

    public string nextLevel;
    public float fadingSpeed = 1;
    public float timeBetweenFading = 0.1f;
    public float timeBetweenMessages = 1f;
    public float timeOfMessages = 5f;
    public bool autoText = false;
    public bool showLeaderboard = false;
    public Image fadingTexture;
    public GameObject skipButton;
    [TextArea(4, 20)]
    public string[] textBoxes;
    public Text textDisplay;

    private AsyncOperation async;
    private int currentText;

    void Start()
    {
        skipButton.SetActive(false);
        StartCoroutine(loading(nextLevel));
        textDisplay.text = textBoxes[0];
        currentText = 0;

        StartCoroutine(AUTO(autoText));

        if (showLeaderboard)
        {
            displayLeaderboard();
        }
    }

    IEnumerator loadLevel(string levelName)
    {
        Color temp = fadingTexture.color;

        while (temp.a <= 1)
        {
            temp.a += fadingSpeed;
            fadingTexture.color = temp;
            yield return new WaitForSeconds(timeBetweenFading);
        }

        async.allowSceneActivation = true;
    }

    public void fadeOut()
    {
        StartCoroutine(loadLevel(nextLevel));
    }

    IEnumerator loading(string levelName)
    {
        //StartCoroutine(loadLevel(levelName));

        //yield return new WaitForSeconds(1.3f);

        //SceneManager.LoadScene(levelName);

        async = SceneManager.LoadSceneAsync(levelName);
        async.allowSceneActivation = false;

        //yield return new WaitForSeconds(0.5f);
        //Debug.Log("Loading level " + levelName + ": " + async.progress);

        while(async.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        //yield return async;
        
        Debug.Log("Loaded!");

        skipButton.SetActive(true);
    }

    public void changeText(int direction)
    {
        currentText += direction;
        
        if (currentText < textBoxes.Length)
            textDisplay.text = textBoxes[currentText];
        //Debug.Log("Loading level " +  nextLevel + ": " + async.progress);
    }

    IEnumerator AUTO(bool value)
    {
        if (value)
        {
            int l = textBoxes.Length;
            for (int a = 0; a < l; a++)
            {
                textDisplay.text = "";
                yield return new WaitForSeconds(timeBetweenMessages);
                textDisplay.text = textBoxes[a];
                yield return new WaitForSeconds(timeOfMessages);
            }

            fadeOut();
        }
    }

    void displayLeaderboard()
    {
        SaveLoad.Load();
        Game temp = SaveLoad.savedGame;
        float[] times = temp.bestTimes;
        int[] health = temp.lessDamageReceivedByFloor;
        int l = times.Length;
        string output = " \n";

        for (int a = 0; a < l; a++)
        {
            Debug.Log(output);
            float tempT = health[a];
            output += "Floor " + (a + 1) + ": \nTime Record: " + (Mathf.RoundToInt(tempT / 60)).ToString("00") + ":" + (Mathf.RoundToInt(tempT % 60)).ToString("00") + ":" + (Mathf.RoundToInt((tempT - Mathf.FloorToInt(tempT)) * 100)).ToString("00") + "\nDamage Record: " + health[a] + "\n\n";
        }

        textDisplay.text = output;
    }
}
