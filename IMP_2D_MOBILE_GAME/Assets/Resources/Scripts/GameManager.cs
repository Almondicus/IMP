using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class GameManager : MonoBehaviour {

    // PUBLIC VARIABLES
    // DEFINING GAMEMANGER AS SIGLETON => PREVENTING MULTIPLE LAUNCHS
    public static GameManager instance = null;

    // BOARDSCRIPT OF THE TYPE BOARDMANAGER
    public BoardManager boardScript;

    // PLAYER FOOD POINTS
    public int playerFoodPoints = 100;

    // PLAYERS TURN => HIDEININSPECTOR NOT SHOWN IN INSPECTOR
    [HideInInspector] public bool playersTurn = true;

    //PRIVATE VARIABLES
    // DEBUG LVL 4 => WHERE ENEMIES FIRST APPEAR
    private int level = 4;

    // Use this for initialization
    void Awake() {

        // CHECK IF INSTANCE == NULL => ASSIGNE TO THIS
        if (instance == null) {
            instance = this;
        }
        // DESTROY INSTANCE => PREVENTING MULTIPLE GAMEMANAGER OBJECTS
        else if (instance != this) {
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

    // FUNCTION FOR INITIALIZING THE GAME
    void InitGame() {
        // CALL THE SETUPSCENE FUNCTION OF THE BOARDSCRIPT VARIABLE => LEVEL AS ARGUMENT
        boardScript.SetupScene(level);
    }

    // FUNCTION FOR GAME OVER
    public void GameOver(){
        enabled = false;
    }
	 
}
 