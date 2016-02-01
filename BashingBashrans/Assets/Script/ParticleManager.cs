using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {


    public void spawnParticles(ParticleSystem particles, Vector3 location, float despawnTime)
    {
        ParticleSystem part = Instantiate(particles, location, Quaternion.identity) as ParticleSystem;
        StartCoroutine(mementoMori(part, despawnTime));
    }

    IEnumerator mementoMori(ParticleSystem particles, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(particles.gameObject);
    }
}
