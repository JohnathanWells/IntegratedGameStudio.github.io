using UnityEngine;
using System.Collections;

public class endLevelSensor : MonoBehaviour {

    public GameObject floorClearedScreen;
    public Animator playerAnimator;
    levelManager highManager;

	void Start () {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            highManager.musicManager.SendMessage("playVictory");
            highManager.SendMessage("floorIsCleared");
            floorClearedScreen.SetActive(true);
            floorClearedScreen.BroadcastMessage("setManager");
            playerAnimator.SetBool("Victory", true);
        }
    }
}
