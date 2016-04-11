using UnityEngine;
using System.Collections;

public class SoundEffectManager : MonoBehaviour {

    //public float Volume;
    public AudioSource source;

    private float MasterVolume = 1.0f;
    private float SFXVolume = 1.0f;

    void Start()
    {
        SaveLoad.Load();
        SFXVolume = SaveLoad.savedGame.SFXVolume;
        MasterVolume = SaveLoad.savedGame.MasterVolume;
        refreshVolume();
    }

    public void PlaySound(AudioClip clip)
    {
        //source.volume = Volume;
        source.PlayOneShot(clip);
    }

    public void refreshVolume()
    {
        source.volume = SFXVolume * MasterVolume;
    }

    public void OnMasterChange(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        refreshVolume();
    }

    public void OnSFXChange(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;
        refreshVolume();
    }

    public void saveSFX()
    {
        SaveLoad.savedGame.SFXVolume = SFXVolume;
        SaveLoad.savedGame.MasterVolume = MasterVolume;
    }
}
