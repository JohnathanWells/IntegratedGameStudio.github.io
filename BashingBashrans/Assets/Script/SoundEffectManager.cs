using UnityEngine;
using System.Collections;

public class SoundEffectManager : MonoBehaviour {

    //public float Volume;
    public AudioSource source;

    public void PlaySound(AudioClip clip)
    {
        //source.volume = Volume;
        source.PlayOneShot(clip);
    }
}
