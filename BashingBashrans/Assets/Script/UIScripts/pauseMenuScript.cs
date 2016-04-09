using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class pauseMenuScript : MonoBehaviour {

    public InputField passwordEnter;
    public Button[] logs;
    public Text messageDisplay;
    public AudioClip wrongMessageSound;
    public AudioClip rightMessageSound;

    private string lastInput;
    private bool checkingPassword = false;

    levelManager highManager;
    SoundEffectManager sfx;

    void Start()
    {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        sfx = highManager.SFXManager;
    }

    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            enterPassword();
    }

    public void enterPassword()
    {
        string password = passwordEnter.text;

        if (password == "cakeisalie")
        {
            cheats(0);
            return;
        }
        else if (password == "mementomori")
        {
            cheats(1);
            return;
        }

        Debug.Log(password);
        SaveLoad.Load();
        if (password != lastInput && !checkingPassword && SaveLoad.savedGame.checkPassword(password))
        {
            sfx.PlaySound(rightMessageSound);
            passwordEnter.text = "LOG UNLOCKED";
            setActiveButtons();
            highManager.SendMessage("healPlayer");
        }
        else
        {
            sfx.PlaySound(wrongMessageSound);
            passwordEnter.text = "ERROR";
            setActiveButtons();
        }
    }

    public void setActiveButtons()
    {
        SaveLoad.Load();
        bool[] temp = SaveLoad.savedGame.unlockedPasswords;
        int l = temp.Length;

        for (int a = 0; a < l; a++)
        {
            logs[a].gameObject.SetActive(temp[a]);

            //Debug.Log("log " + a + "is now " + logs[a].IsActive());
        }
    }

    void cheats(int type)
    {
        if (type == 0)
        {
            int l = SaveLoad.savedGame.unlockedPasswords.Length;

            for (int a = 0; a < l; a++)
            {
                SaveLoad.savedGame.unlockedPasswords[a] = true;
            }
            SaveLoad.Save();
            setActiveButtons();

            passwordEnter.text = "";

            return;
        }
        else if (type == 1)
        {
            Debug.Log("Data erased");
            SaveLoad.Delete();
            setActiveButtons();

            passwordEnter.text = "";

            return;
        }
    }

    public void showMessage(int a)
    {
        messageDisplay.text = SaveLoad.savedGame.returnMessage(a);
    }

    public void quitGame()
    {
        SaveLoad.Save();
        Application.Quit();
    }
}
