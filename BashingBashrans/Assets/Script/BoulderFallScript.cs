using UnityEngine;
using System.Collections;

public class BoulderFallScript : MonoBehaviour {

    public float roofHeight = 10;
    public Color ColorA;
    public Color ColorB;
    public SpriteRenderer renderer;
    public int numberOfWarnings = 3;
    public float timeOfBlip = 0.5f;
    public Transform Boulder;
    public AudioClip warningSound;
    public bool audibleWarning = false;

    GameManager manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        StartCoroutine(warning());
	}

    IEnumerator warning()
    {
        for (int a = 0; a < numberOfWarnings; a++)
        {
            renderer.color = ColorA;
            yield return new WaitForSeconds(timeOfBlip);
            
            if (audibleWarning)
                manager.SFX.PlaySound(warningSound);
            
            renderer.color = ColorB;
            yield return new WaitForSeconds(timeOfBlip);
        }

        //Note, we may need to change the z axis later because I'm assuming here that the coordinates are always gonna be negative
        Transform boulder = Instantiate(Boulder, new Vector3(transform.position.x, roofHeight, transform.position.z), Quaternion.identity) as Transform;
        boulder.GetComponent<BoulderScript>().objectiveHeight = transform.position.y;
        Destroy(gameObject);
    }
}
