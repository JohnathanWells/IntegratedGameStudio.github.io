using UnityEngine;
using System.Collections;

public class fireScript : MonoBehaviour {

    public int damagePerSecond = 10;
    public float burningTime = 10;
    public ParticleSystem fireParticles;

    public float lifetime = 0;

    //void Start()
    //{
    //    GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().getPM().spawnParticles(fireParticles, transform.position, lifetime);
    //}

	void Update () {
        lifetime += Time.deltaTime;

        if (lifetime >= burningTime)
        {
            Destroy(gameObject);
        }
	
	}
}
