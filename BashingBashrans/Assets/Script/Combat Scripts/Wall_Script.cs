using UnityEngine;
using System.Collections;

public class Wall_Script : MonoBehaviour {

    GameManager manager;
	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnTriggerEnter(Collider w)
    {
        if (w.CompareTag("Projectile"))
        {
            ProjectileScript pr = w.GetComponent<ProjectileScript>();
            pr.bouncy();
        }
    }
}
