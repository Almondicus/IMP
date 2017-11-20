using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

	//Damage the player can apply on the wall by chopping
	public int wallDamage = 1;
	//Health points per Flame
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
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0)
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


	private void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			other.gameObject.SetActive (false);
		}
			
	}


	protected override void OnCantMove<T>(T component){
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}


	private void Restart(){
		//On restart load the level again
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
