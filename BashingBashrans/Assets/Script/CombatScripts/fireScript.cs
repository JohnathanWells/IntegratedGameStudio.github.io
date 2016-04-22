using UnityEngine;
using System.Collections;

public class fireScript : MonoBehaviour {

    public int damagePerSecond = 10;
    public float burningTime = 10;
    public float timeBeforeFire = 1;
    public Material foreshadowMaterial;
    public Material damagingMaterial;
    public ParticleSystem fireParticles;
    public MeshRenderer renderer;
    public BoxCollider fireBox;

    public float lifetime = 0;

    private bool readyForFire = false;

    void Start()
    {
        StartCoroutine(foreshadowing());
    }

	void Update () {
        if (readyForFire)
        {
            lifetime += Time.deltaTime;

            if (lifetime >= burningTime)
            {
                StartCoroutine(fireDecayPart());
            }
        }
	}

    IEnumerator fireDecayPart()
    {
        fireBox.enabled = false;
        fireParticles.loop = false;
        renderer.enabled = false;
        yield return new WaitForSeconds(fireParticles.duration - fireParticles.time);
        Destroy(gameObject);
    }

    IEnumerator foreshadowing()
    {
        readyForFire = false;
        fireBox.enabled = false;
        renderer.enabled = true;
        renderer.material = foreshadowMaterial;
        fireParticles.enableEmission = false;
        yield return new WaitForSeconds(timeBeforeFire);
        renderer.material = damagingMaterial;
        readyForFire = true;
        fireBox.enabled = true;
        fireParticles.enableEmission = true;
        fireParticles.time = 0;
    }
}
