using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	//on interaction with player substract 
	public int playerDamage;

	private Animator animator;
	//players position
	private Transform target;
	//ships a move every other turn
	private bool skipMove;



	// override Start() from base
	protected override void Start () {
		//add selfe to list in GameManager
		GameManager.instance.AddEnemyToList(this);
		//initialize the animator
		animator = GetComponent<Animator> ();
		//find the position of the player
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		//start in MovingObject
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	protected override void AttemptMove <T>(int xDir, int yDir){
		//only move when it's the enemys turn
		if (skipMove) {
			skipMove = false;
			return;
		}
		//T = Player <-- the enemy interacts with him
		//enemy moves
		base.AttemptMove<T> (xDir, yDir);
		skipMove = true;
	}

	//GameManager get's it to organize the order of the enemies moving
	public void MoveEnemy(){
		int xDir = 0;
		int yDir = 0;

		//where to move?
		//if the x coordinates are the same (same column)
		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			//check if y of the player is higher then our y
			//true: move up
			//false: move down
			yDir = target.position.y > transform.position.y ? 1 : -1;}
		//else if the xs are not the same
		else{
			//move on x nearer to the player, depending on where he is
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}
		//move the enemy
		//Player as component to interact with
		AttemptMove<Player>(xDir, yDir);
	}

	//if the enemy meets the player
	protected override void OnCantMove<T> (T component)
	{
		//get the component
		Player hitPlayer = component as Player;
		//set the right animation
		animator.SetTrigger ("enemyAttack");
		//make damage
		hitPlayer.LoseFood (playerDamage);
	}
}
