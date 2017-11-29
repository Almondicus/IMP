using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	//CREATE THE VARIABLE LEVELSTARTDELAY OF TYPE FLOAT
	public float levelStartDely = 2f;

	//CREATING THE VARIABLE TURNDELAY OF TYPE FLOAT
	public float turnDelay = .1f;

	//CREATING THE VARIABLE PLAYERFOODPOINTS OF THE TYPE INT
	public int playerFoodPoints = 100;


    // DEFINING GAMEMANGER AS SIGLETON => PREVENTING MULTIPLE LAUNCH
	private static GameManager instance;


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

	//PREVENTING MULTIPLE GAMEMANAGER OBJECTS
	public static GameManager Instance{
		//get Instance of GameManager
		get{
			//if instance is empty
			if (instance == null) {
				//set it to existing GameManager GameObject 
				instance = GameObject.FindObjectOfType<GameManager> ();
			}
			//return this instance
			return instance;
		}
	}

	// Use this for initialization
	protected virtual void Awake(){

			//if there is an instance AND the instance is not this object
			if(instance != null && instance != this){
				//destroy this object
				Destroy(this.gameObject);
				//and stop the code
				return;
			}
			//else set intance to this
			instance = this;


		//CREATE A LIST WITH ENEMIES
		enemies = new List<Enemy>();

        // COMPONENT REFERENCE TO BOARDMANAGER SCRIPT
        boardScript = GetComponent<BoardManager>();

		//INITIALIZE THE GAME
        InitGame();
	}

	//if the level was loaded
	void OnLevelWasLoaded(int index){
		//set up the level
		level++;
		//initialize the game
		InitGame ();
	}


    void InitGame(){
		//CLEAR THE LIST OF OLD ENEMIES
		enemies.Clear();
        // CALL THE SETUPSCENE FUNCTION OF THE BOARDSCRIPT VARIABLE => LEVEL AS ARGUMENT
        boardScript.SetupScene(level);
    }
		

	void Update(){
		//if it is the enemies turn OR the enemies are allready moving --> return
		//else start moving the enemies
		if (playersTurn || enemiesMoving)
			return;
		StartCoroutine (MoveEnemies ());
	}


	public void AddEnemyToList(Enemy script){
		//add enemies to the list and give the list to the GameManager to handle them
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
