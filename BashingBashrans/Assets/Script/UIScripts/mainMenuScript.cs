using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour {

    public GameObject[] levelButtons;
    public AudioSource musicSource;
    public SoundEffectManager SFX;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    //public AudioClip menuMusic;

    private float MusicVolume;
    private float MasterVolume;

    void Start()
    {
        SaveLoad.Load();
        //levelButtons = new GameObject[SaveLoad.savedGame.returnUnlockedFloors().Length];
        screenshotScript.getScreenshotsInDirectory();
        MusicVolume = SaveLoad.savedGame.MusicVolume;
        MasterVolume = SaveLoad.savedGame.MasterVolume;
        masterSlider.value = MasterVolume;
        musicSlider.value = MusicVolume;
        sfxSlider.value = SFX.getSFXVolume();
        musicSource.Play();
        //disableLockedLevels();
    }

    public void save()
    {
        SaveLoad.Save();
    }

    public void cleanPasswords()
    {
        Game temp = SaveLoad.savedGame;

        temp.newPasswords();
        temp.healthKits = 1;

        SaveLoad.Save();
    }

    public void cleanData()
    {
        SaveLoad.Delete();
    }

    public void loadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    static public void closeGame()
    {
        Application.Quit();
    }

    public void disableLockedLevels()
    {
        bool[] unlockedLevels = SaveLoad.savedGame.returnUnlockedFloors();
        int l = levelButtons.Length;


        for (int a = 0; a < l; a++)
        {
            levelButtons[a].SetActive(unlockedLevels[a]);
        }
    }

    public void saveMusic()
    {
        SaveLoad.savedGame.MusicVolume = MusicVolume;
    }

    public void refreshVolume()
    {
        musicSource.volume = MusicVolume * MasterVolume;
    }

    public void OnMasterChange(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        refreshVolume();
    }

    public void OnMusicChange(float newSFXVolume)
    {
        MusicVolume = newSFXVolume;
        refreshVolume();
    }
}
