using UnityEngine;
using System.Collections;

public class fireScript : MonoBehaviour {

    public int damagePerSecond = 10;
    public float burningTime = 10;

    public float lifetime = 0;

	void Update () {
        lifetime += Time.deltaTime;

        if (lifetime >= burningTime)
        {
            Destroy(gameObject);
        }
	
	}
}
