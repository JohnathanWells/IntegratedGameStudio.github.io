﻿using UnityEngine;
using System.Collections;

public class endLevelSensor : MonoBehaviour {

    public GameObject floorClearedScreen;
    levelManager highManager;

	void Start () {
        highManager = GameObject.FindGameObjectWithTag("High Game Manager").GetComponent<levelManager>();
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            highManager.SendMessage("floorIsCleared");
            floorClearedScreen.SetActive(true);
        }
    }
}