using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // DEFINING GAMEMANGER AS SIGLETON => PREVENTING MULTIPLE LAUNCH
    public static GameManager instance = null;

    // CREATING THE VARIABLE BOARDSCRIPT OF THE TYPE BOARDMANAGER
    public BoardManager boardScript;

    // DEBUG LVL 3 => WHERE ENEMIES FIRST APPEAR
    private int level = 4;

	// Use this for initialization
	void Awake(){

        // CHECK IF INSTANCE == NULL => ASSIGNE TO THIS
        if (instance == null) {
            instance = this;
        }
        // DESTROY INSTANCE => PREVENTING MULTIPLE GAMEMANAGER OBJECTS
        else if(instance != this){
            Destroy(gameObject);
        }

        // DON'T DESTROY ON LOAD
        // WHEN LOADING A NEW SCENEN, NORMALLY ALL GAMEOBJECTS INSIDE THE HIRARCHY ARE DESTROYED
        // THE GAMEMANAGER IS USED TO KEEP TRACK OF THE SCORE AND SO ON BETWEEN THE SCENES
        // => DONTDESTROYONLOAD PRESERVES IT FROM BEING DESTROYED
        DontDestroyOnLoad(gameObject);

        // COMPONENT REFERENCE TO BOARDMANAGER SCRIPT
        boardScript = GetComponent<BoardManager>();
        InitGame();
		
	}

    void InitGame(){
        // CALL THE SETUPSCENE FUNCTION OF THE BOARDSCRIPT VARIABLE => LEVEL AS ARGUMENT
        boardScript.SetupScene(level);
    }
	 
}
