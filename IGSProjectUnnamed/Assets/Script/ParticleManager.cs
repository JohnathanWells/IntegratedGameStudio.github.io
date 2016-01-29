using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {


    public void spawnParticles(ParticleSystem particles, Vector3 location, float despawnTime)
    {
        Transform PS = Instantiate(particles, location, Quaternion.identity) as Transform;
        mementoMori(PS, despawnTime);
    }

    IEnumerator mementoMori(Transform system, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(system.gameObject);
    }
}
