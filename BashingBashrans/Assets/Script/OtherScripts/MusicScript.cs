using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioClip floorSong;
    public AudioClip bossSong;
    public AudioClip gameOverSong;
    public AudioClip victorySong;
    public AudioSource source;
    public float fadingSpeed = 0.05f;

    private float MusicVolume;
    private float MasterVolume;

	// Use this for initialization
	public void Start () {
        SaveLoad.Load();
        MusicVolume = SaveLoad.savedGame.MusicVolume;
        MasterVolume = SaveLoad.savedGame.MasterVolume;
        //refreshVolume();
        StartCoroutine(fadeMusic(1));
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

    IEnumerator fadeMusic(int dir)
    {
        if (dir > 1)
        {
            for (float a = 0; a < 1; a += fadingSpeed)
            {
                yield return new WaitForSeconds(0.1f);
                source.volume = MusicVolume * MasterVolume * a;
            }
        }
        else
        {
            for (float a = 1; a > 0; a -= fadingSpeed)
            {
                yield return new WaitForSeconds(0.1f);
                source.volume = MusicVolume * MasterVolume * a;
            }
        }
    }

    public void fadeTheMusic(int dir)
    {
        StartCoroutine(fadeMusic(dir));
    }
}
