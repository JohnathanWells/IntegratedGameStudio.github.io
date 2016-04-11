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
            playerAnimator.SetBool("Victory", true);
			StartCoroutine(IWin());
        }
    }

	IEnumerator IWin()
	{yield return new WaitForSeconds (1);
		highManager.SendMessage("floorIsCleared");
		floorClearedScreen.SetActive(true);
		floorClearedScreen.BroadcastMessage("setManager");
	}
}
