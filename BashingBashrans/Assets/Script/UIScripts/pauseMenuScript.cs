using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class pauseMenuScript : MonoBehaviour {

    public InputField passwordEnter;
    public Button[] logs;
    public Text messageDisplay;
    public AudioClip wrongMessageSound;
    public AudioClip rightMessageSound;

    [Header("Options")]
    public Slider MasterV;
    public Slider MusicV;
    public Slider SFXV;

    private string lastInput;
    private bool checkingPassword = false;

    levelManager highManager;
    SoundEffectManager sfx;
    MusicScript music;

    void Start()
    {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        sfx = highManager.SFXManager;
        music = highManager.musicManager;
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

    public void saveOptions()
    {
        sfx.SendMessage("saveSFX");
        music.SendMessage("saveMusic");
        SaveLoad.Save();
    }

    void fixSoundSliders()
    {
        SaveLoad.Load();
        Game temp = SaveLoad.savedGame;
        MasterV.value = temp.MasterVolume;
        MusicV.value = temp.MusicVolume;
        SFXV.value = temp.SFXVolume;
    }
}
