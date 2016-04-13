using UnityEngine;
using System.Collections;

public class endLevelSensor : MonoBehaviour {

    public GameObject floorClearedScreen;
    public Animator playerAnimator;
    levelManager highManager;
    CombatScript playerScript;

	void Start () {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
        playerScript = highManager.getPlayerCombatScript();
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            highManager.musicManager.SendMessage("playVictory");
			StartCoroutine(IWin());
        }
    }

	IEnumerator IWin()
	{
        highManager.SendMessage("changePlayerCanMove", false);
        playerAnimator.SetBool("Victory", true);
        yield return new WaitForSeconds (0.5f);
		highManager.SendMessage("floorIsCleared");
		floorClearedScreen.SetActive(true);
		floorClearedScreen.BroadcastMessage("setManager");
	}
}
