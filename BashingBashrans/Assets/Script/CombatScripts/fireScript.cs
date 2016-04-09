using UnityEngine;
using System.Collections;

public class fireScript : MonoBehaviour {

    public int damagePerSecond = 10;
    public float burningTime = 10;
    public ParticleSystem fireParticles;
    public MeshRenderer renderer;
    public BoxCollider fireBox;

    public float lifetime = 0;

    //void Start()
    //{
    //    GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().getPM().spawnParticles(fireParticles, transform.position, lifetime);
    //}

	void Update () {
        lifetime += Time.deltaTime;

        if (lifetime >= burningTime)
        {
            StartCoroutine(fireDecayPart());
        }
	
	}

    IEnumerator fireDecayPart()
    {
        fireBox.enabled = false;
        fireParticles.loop = false;
        renderer.enabled = false;
        yield return new WaitForSeconds(fireParticles.duration);
        Destroy(gameObject);
    }
}
