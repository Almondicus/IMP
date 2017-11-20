using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

	//Damage the player can apply on the wall by chopping
	public int wallDamage = 1;
	//Health points per Food and Soda
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;

	public float restartLevelDelay = 1f;

	//Declaring the animator
	private Animator animator;

	//stores the score during the level
	//gives it to GameManager during leveltransition
	private int food;



	// Player has a different Start() than the level
	protected override void Start () {
		//initialize animator and flames
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;
		//start MovingObject
		base.Start();
	}


	//When the player is disabled
	private void OnDisable(){
		//give the flamePoints to the GameManager to store them during a levelchange
		GameManager.instance.playerFoodPoints = food;
	}

	// Update is called once per frame
	void Update () {
		//ignore the input, if it#s not the players turn
		if (!GameManager.instance.playersTurn)
			return;

		//declare variables horizontal and vertical
		int horizontal = 0;
		int vertical = 0;

		//initialize them with the keyboard input
		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		//if you walk horizontally you cannot move vertically 
		if (horizontal != 0)
			vertical = 0;
		//if you move horizontally or vertically
		if (horizontal != 0 || vertical != 0)
			//start AttemptMove and give it Wall as a component
			AttemptMove<Wall> (horizontal, vertical);
	}

	//override the function from MovingObject
	protected override void AttemptMove<T>(int xDir, int yDir){
		//with every move the flame points drop
		food--;
		//try to move
		base.AttemptMove<T> (xDir, yDir);
		RaycastHit2D hit;
		//as flames are lost --> check if the game is over
		CheckIfGameOver ();
		//the players turn is over
		GameManager.instance.playersTurn = false;

	}

	//entering a trigger
	private void OnTriggerEnter2D (Collider2D other){
		//on exit
		if (other.tag == "Exit") {
			//restart the level --> GameManager builds a new one
			Invoke ("Restart", restartLevelDelay);
			//disable the player
			enabled = false;
			//on food
		} else if (other.tag == "Food") {
			//raise the food points by 10
			food += pointsPerFood;
			//disable the food --> vanishes
			other.gameObject.SetActive (false);
			//on soda
		} else if (other.tag == "Soda") {
			//raise the food points by 20
			food += pointsPerSoda;
			//disable the soda --> vanishes
			other.gameObject.SetActive (false);
		}
			
	}


	//if there is an obstacle
	protected override void OnCantMove<T>(T component){
		Wall hitWall = component as Wall;
		//activate the damage wall function
		hitWall.DamageWall (wallDamage);
		//set the animation for chopping the wall down
		animator.SetTrigger ("playerChop");
	}


	private void Restart(){
		//On restart load the level again (automatically new level)
		SceneManager.LoadScene ("Main");

	}


	//when enemy attacks
	public void LoseFlames(int loss){
		//start the fitting animation
		animator.SetTrigger ("playerHit");
		//lose flames
		food -= loss;
		//see if enough flames are left
		CheckIfGameOver ();
	}

	//check if the game is over
	private void CheckIfGameOver(){
		//if flames is 0
		//call the GameOver function of the GameManager
		if (food <= 0)
			GameManager.instance.GameOver ();
	}
		
}
