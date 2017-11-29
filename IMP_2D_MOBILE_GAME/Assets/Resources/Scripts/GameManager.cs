using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public float levelStartDely = 2f;

	//CREATING THE VARIABLE TURNDELAY OF TYPE FLOAT
	public float turnDelay = .1f;

	//CREATING THE VARIABLE PLAYERFOODPOINTS OF THE TYPE INT
	public int playerFoodPoints = 100;


    // DEFINING GAMEMANGER AS SIGLETON => PREVENTING MULTIPLE LAUNCH
    public static GameManager instance = null;


	//CREATING THE VARIABLE PLAYERSTURN AND INITIALIZE IT TO TRUE
	//DOES NOT SHOW IN EDITOR; EVEN THOUGH IT'S PUBLIC
	[HideInInspector] public bool playersTurn = true;

    // CREATING THE VARIABLE BOARDSCRIPT OF THE TYPE BOARDMANAGER
    private BoardManager boardScript;

    // DEBUG LVL 3 => WHERE ENEMIES FIRST APPEAR
    private int level = 4;

	//CREAE A LIST FOR THE ENEMYS
	private List<Enemy> enemies;

	//DECLARE A VARIABLE CALLED ENEMIESMOVING OF TYPE BOOLEAN
	private bool enemiesMoving;


	// Use this for initialization
	void Awake(){

        // CHECK IF INSTANCE == NULL => ASSIGNE TO THIS
        if (instance == null) {
            instance = this;
			// DON'T DESTROY ON LOAD
			// WHEN LOADING A NEW SCENEN, NORMALLY ALL GAMEOBJECTS INSIDE THE HIRARCHY ARE DESTROYED
			// THE GAMEMANAGER IS USED TO KEEP TRACK OF THE SCORE AND SO ON BETWEEN THE SCENES
			// => DONTDESTROYONLOAD PRESERVES IT FROM BEING DESTROYED
			DontDestroyOnLoad(this);
        }
        // DESTROY INSTANCE => PREVENTING MULTIPLE GAMEMANAGER OBJECTS
        else if(this != instance){
            Destroy(this.gameObject);
        }


	
      


		//CREATE A LIST WITH ENEMIES
		enemies = new List<Enemy>();

        // COMPONENT REFERENCE TO BOARDMANAGER SCRIPT
        boardScript = GetComponent<BoardManager>();

        InitGame();
	}

	void OnLevelWasLoaded(int index){
		level++;
		InitGame ();
	}


    void InitGame(){
		//CLEAR THE LIST OF OLD ENEMIES
		enemies.Clear();
        // CALL THE SETUPSCENE FUNCTION OF THE BOARDSCRIPT VARIABLE => LEVEL AS ARGUMENT
        boardScript.SetupScene(level);
    }
		

	void Update(){
		//start MoveEnemies, if it's their turn and they are not allready moving
		if (playersTurn || enemiesMoving)
			return;

		StartCoroutine (MoveEnemies ());
	}


	public void AddEnemyToList(Enemy script){
		//add enemies to the list and give them to the GameManager to handle them
		enemies.Add (script);
	}



	public void GameOver(){
		
		//DISABLES THE GAMEMANAGER WHEN GAMEOVER
		enabled = false;
	}


	//moves the enemies one at a time
	IEnumerator MoveEnemies(){
		//set enemiesMoving to true
		enemiesMoving = true;
		//wait so everything happens orderly
		yield return new WaitForSeconds (turnDelay);

		//if there are no enemies
		if (enemies.Count == 0) {
			//let the player wait
			yield return new WaitForSeconds (turnDelay);		
		}
		for (int i = 0; i < enemies.Count; i++) {
			//every enemy is moved
			enemies [i].MoveEnemy ();
			//wait after every moved enemy
			yield return new WaitForSeconds (enemies[i].moveTime);
		}
		//set the players Turn to true
		playersTurn = true;
		//set the enemiesMoving to false
		enemiesMoving = false;

	}

}
