using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioClip floorSong;
    public AudioClip bossSong;
    public AudioClip gameOverSong;
    public AudioClip victorySong;
    public AudioSource source;
    private float volume;

	// Use this for initialization
	public void Start () {
        SaveLoad.Load();
        volume = SaveLoad.savedGame.MusicVolume;
        source.volume = volume;
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
}
