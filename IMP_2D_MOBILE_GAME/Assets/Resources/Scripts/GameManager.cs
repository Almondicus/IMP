using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    /* PUBLIC VARIABLES */
    // START DELAY FOR EACH LEVEL
    public float levelStartDelay = 2f;

    // HOW LONG THE GAMES WAITS BETWEEN TURNS
    public float turnDelay = .1f;

    // DEFINING GAMEMANGER AS SIGLETON => PREVENTING MULTIPLE LAUNCHS
    public static GameManager instance = null;
     
    // PLAYER FOOD POINTS
    public int playerFoodPoints = 100;

    // PLAYERS TURN => HIDEININSPECTOR NOT SHOWN IN INSPECTOR
    [HideInInspector] public bool playersTurn = true;

    /* PRIVATE VARIABLES */
    // LEVEL TEXT
    private Text levelText;

    // LEVEL IMAGE
    private GameObject levelImage;

    // BOARDSCRIPT OF THE TYPE BOARDMANAGER
    private BoardManager boardScript;
    
    // INITIAL GAME LEVEL
    private int level = 1;

    // LIST OF ENEMIES TO KEEP TRACK OF THE ENEMIES AND SEND THEM THEIR ORDERS TO MOVE
    private List<Enemy> enemies;

    // VOOLEAN TO CHECK IF ENEMIES ARE MOVING
    private bool enemiesMoving;

    // DOINGSETUP => PREVENT PLAYER FROM MOVING WHILE SETUP
    private bool doingSetup;

    /* FUNCTIONS */
    // Use this for initialization
    void Awake(){

        // CHECK IF INSTANCE == NULL => ASSIGNE TO THIS
        if (instance == null){
            instance = this;
        }
        // DESTROY INSTANCE => PREVENTING MULTIPLE GAMEMANAGER OBJECTS
        else if (instance != this){  
            //
            Destroy(gameObject); 
        }  

        // DON'T DESTROY ON LOAD
        // WHEN LOADING A NEW SCENEN, NORMALLY ALL GAMEOBJECTS INSIDE THE HIRARCHY ARE DESTROYED
        // THE GAMEMANAGER IS USED TO KEEP TRACK OF THE SCORE AND SO ON BETWEEN THE SCENES
        // => DONTDESTROYONLOAD PRESERVES IT FROM BEING DESTROYED
        DontDestroyOnLoad(gameObject);
         
        // COMPONENT REFERENCE TO BOARDMANAGER SCRIPT
        boardScript = GetComponent<BoardManager>();
        
        // SET NEW LIST ENEMIES OF THE TYPE ENEMY
        enemies = new List<Enemy>();

        // CALL INITGAME
        InitGame();        

    }

    // IS CALLED EVERY TIME A SCENE IS LOADED 
    private void OnLevelWasLoaded(int index){

        // INCREASE LEVEL BY 1
        level++;

        // CALL INITGAME FUNCTION
        InitGame();

    }

    // FUNCTION FOR INITIALIZING THE GAME
    void InitGame() {

        // DOING SETUP
        doingSetup = true;

        // GET GAMEOBJECTS BY NAME
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        // SET LEVELTEXT TO DAY + LEVEL NUMBER
        levelText.text = "Day " + level;

        // ACTIVATE THE LEVELIMAGE
        levelImage.SetActive(true);

        // WAIT LEVELSTARTDELAY IN SECONDS BEFORE CALLING HIDELEVELIMAGE FUNCTION
        // => LEVELSTARTDELAY = 2 SECONDS
        Invoke("HideLevelImage", levelStartDelay);

        // CLEAR LIST OF ENEMIES
        // => BECAUSE GAMEMANGER WON'T BE RESET BETWEEN LEVELS
        enemies.Clear();
        
        // CALL THE SETUPSCENE FUNCTION OF THE BOARDSCRIPT VARIABLE => LEVEL AS ARGUMENT
        boardScript.SetupScene(level);

    }

    // FUNCTION TO HIDE THE LEVEL IMAGE
    private void HideLevelImage(){

        // DEACTIVATE LEVELIMAGE 
        levelImage.SetActive(false);

        // SET DOINGSETUP TO FALSE
        doingSetup = false;

    }

    // FUNCTION FOR GAME OVER
    public void GameOver(){

        // SET LEVEL TEXT
        levelText.text = "After " + level + " days, you died.";

        // ACTIVATE LEVEL IMAGE
        levelImage.SetActive(true);

        enabled = false;
    }

    // UPDATE FUNCTION
    void Update(){

        // IF PLAYERS TURN OR ENEMIES ALREADY MOVING
        if(playersTurn || enemiesMoving || doingSetup){

            // DON'T EXECUTE FOLLOWING CODE
            return;
        }

        // ELSE => START COROUTINE AND MOVE ENEMIES
        StartCoroutine(MoveEnemies());
         
    }

    // FUNCTION TO ADD ENEMIES TO ENEMIESLIST
    // => TAKES SCRIPT OF THE TYPE ENEMY AS A PARAMETER
    // => LET ENEMY REGISTER THEMSELF WITH THE GAMEMANAGER, SO THE GAMEMANAGER CAN ISSUE MOVEMENT ORDERS TO THEM
    public void AddEnemyToList(Enemy script){

        // ADD ENEMY TO THE ENEMIES LIST
        enemies.Add(script);

    }

    // COROUTINE TO MOVE ENEMIES
    IEnumerator MoveEnemies(){

        enemiesMoving = true;

        // WAIT AMOUNT OF TURN DELAY
        yield return new WaitForSeconds(turnDelay);

        // IF ENEMIES LIST IS EMPTY
        if(enemies.Count == 0){
            
            // CAUSE PLAYER TO WAIT - EVEN IF THERE'S NO ENEMY TO WAIT FOR
            yield return new WaitForSeconds(turnDelay);

        }

        // LOOP THROUGH ENEMIES LIST
        for(int i = 0; i < enemies.Count; i++){

            // ISSUE EACH ENEMY TO MOVE
            enemies[i].MoveEnemy();

            // WAIT TILL NEXT CALL => PASSING IN MOVETIME VARIABLE OF ENEMIES
            yield return new WaitForSeconds(enemies[i].moveTime);

        }

        // PLAYERS TURN
        playersTurn = true;

        // ENEMIES NOT ALLOWED TO MOVE
        enemiesMoving = false;
         
    }
	 
}
 