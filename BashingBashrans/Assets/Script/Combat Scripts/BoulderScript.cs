using UnityEngine;
using System.Collections;

public class BoulderScript : MonoBehaviour {

    public float fallSpeed = 5f;
    public float desviationSpeed = 7f;
    public ParticleSystem boulderBreak;
    public bool soundWhenCrash = true;
    public AudioClip crashSound;
    public bool punchable = true;
    public int damage = 100;
    public float objectiveHeight;
    public float halflife = 10f;

    private GameManager manager;
    private ParticleManager PM;
    private SoundEffectManager SFX;
    private float horizontalSpeed = 0f;
    private bool beingDesviated = false;
    private float lifetime = 0f;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        PM = manager.PM;
        SFX = manager.SFX;
    }

	void Update () {
        lifetime += Time.deltaTime;

        if (lifetime >= halflife)
        {
            Destroy(gameObject);
        }

        transform.Translate(new Vector2(horizontalSpeed * Time.deltaTime, -fallSpeed * Time.deltaTime));
        
        if (transform.position.y <= objectiveHeight && !beingDesviated)
        {
            DestroyBoulder();
        }
	}

    public void DestroyBoulder()
    {
        SFX.PlaySound(crashSound);
        PM.spawnParticles(boulderBreak, transform.position, boulderBreak.duration);
        Destroy(gameObject);
    }

    public void PunchBoulder(int direction)
    {
        punchable = false;
        fallSpeed = 0;
        horizontalSpeed = desviationSpeed * direction;
        beingDesviated = true;
    }
}
