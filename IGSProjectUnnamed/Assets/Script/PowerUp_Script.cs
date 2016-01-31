using UnityEngine;
using System.Collections;

public class PowerUp_Script : MonoBehaviour {
    Random rnd = new Random();
    int start = Random.Range(1, 10);
    GameManager manager;
    float countdown = 20;
    float window = 0;

	// Use this for initialization
	void Start ()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
    void Update()
    {
        if (countdown + start >= 0)
        {
            countdown -= Time.deltaTime;
        }
        else if (window <= 30)
        {
            window += Time.deltaTime;
        }
        else if (window >= 30)
        {
            DestroyPowerUp();
        }
    }

    public void DestroyPowerUp()
    {
        Destroy(gameObject);
    }
}
