using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioClip floorSong;
    public AudioClip bossSong;
    public AudioClip gameOverSong;
    public AudioClip victorySong;
    public AudioSource source;

    private float MusicVolume;
    private float MasterVolume;

	// Use this for initialization
	public void Start () {
        SaveLoad.Load();
        MusicVolume = SaveLoad.savedGame.MusicVolume;
        MasterVolume = SaveLoad.savedGame.MasterVolume;
        refreshVolume();
	}

    public void playFloor()
    {
        source.clip = floorSong;
        source.Play();
    }

    public void playBoss()
    {
        source.clip = bossSong;
        source.Play();
    }

    public void playGameOver()
    {
        source.clip = gameOverSong;
        source.Play();
    }

    public void stopSong()
    {
        source.clip = null;
        source.Stop();
    }

    public void playVictory()
    {
        source.clip = victorySong;
        source.Play();
    }

    public void refreshVolume()
    {
        source.volume = MusicVolume * MasterVolume;
    }

    public void OnMasterChange(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        refreshVolume();
    }

    public void OnMusicChange(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
        refreshVolume();
    }

    public void saveMusic()
    {
        SaveLoad.savedGame.MusicVolume = MusicVolume;
    }
}
