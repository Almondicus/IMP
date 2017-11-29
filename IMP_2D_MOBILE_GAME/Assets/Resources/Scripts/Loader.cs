using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    // CHECK IF GAMEMANAGER HAS BEEN INSTANTIATED, IF NOT => INSTANTIATE ONE FROM THE PREFAB

    public GameObject gameManager;
	// Use this for initialization
	void Start () {

        // CHECK THE GAMEMANAGER STATIC VARIABLE FROM THE GAMEMANAGER SCRIPT
        // IF == NULL => INSTANTIATE A NEW GAMEMANAGER => INSTANTIATE(GAMEMANAGER)
        if (GameManager.instance == null){
            Instantiate(gameManager);
        }
	}
	 
}
