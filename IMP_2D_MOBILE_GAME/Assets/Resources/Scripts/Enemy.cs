using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITS FROM MOVINGOBJECT CALLS
public class Enemy : MovingObject{

    /* PUBLIC VARIABLES */
    // FOOD POINTS SUBTRACTED WHEN THE ENEMY ATTACKS THE PLAYER
    public int playerDamage;

    /* PRIVATE VARIABLES */
    private Animator animator;

    // STORES THE PLAYERS POSITION AND TELLS THE ENEMY WHERE TO MOVE TOWARDS
    private Transform target;

    // CAUSES THE ENEMY TO MOVE EVERY OTHER TURN
    private bool skipMove;


	// PROTECTED OVERRIDE 
    // => OVERRIDE THE START FUNCTION OF BASE CLASS MOVINGOBJECT
	protected override void Start () {

        // HAVE THE ENEMY ADD ISTSELF TO THE ENEMYLIST IN GAMEMANAGER
        // => GAMEMANAGER CAN NOW CALL THE PUBLIC FUNCTIONS OF ENEMY
        GameManager.instance.AddEnemyToList(this);

        // GET AND STORE COMPONENT REFERENCE 
        animator = GetComponent<Animator>();

        // STORE THE TRANSFORM OF OUR PLAYER AS TARGET
        // => FINDGAMEOBJECTWITHTAG("Player") GETS THE GAMEOBJECT BY IT UNITY TAG
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // CALL THE START FUNCTION IN THE BASE CLASS MOVINGOBJECT
        base.Start();

	}
	 
    // PROTECTED OVERRIDE
    // => OVERRIDES THE ATTEMPTMOVE FUNCTION OF THE BASE CLASS MOVINGOBJECT
    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        // IF SKIPMOVE
        if(skipMove){

            // ENEMY SKIPS TURN
            skipMove = false;
            return;

        }
         
        base.AttemptMove<T>(xDir, yDir); 
        skipMove = true;

    }

    // FUNCTION TO
    // CALLED BY THE GAMEMANAGER WHEN IT ISSUES THE ORDER TO MOVE OF EACH ENEMY IN THE ENEMY LIST
    public void MoveEnemy(){

        int xDir = 0;
        int yDir = 0;

        // CHECK IF THE DIFFERENCE BETWEEN THE X-COORDINATES OF THE TARGETS AND THE TRANSFORMS POSITION IS LESS THAN EPSILON
        // => FLOAT.EPSILON: NUMBER CLOSE TO ZERO
        // => MEANS: ENEMY AND PLAYER ARE IN THE SAME COLUMN
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon){

            // CHECK IF Y-COORDINATE OF TARGET IS BIGGER THAN Y-COORDINATE OF TRANSFORM
            // TRUE: MOVE UP 1
            // FALSE: MOVE DOWN -1
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else{

            // CHECK IF X-COORDINATE OF TARGET IS BIGGER THAN X-COORDINATE OF TRANSFORM
            // TRUE: MOVE RIGHT 1
            // FALSE: MOVE LEFT -1
            xDir = target.position.x > transform.position.x ? 1 : -1;

        }

        // CALLING ATTEMPMOVE WITH PLAYER AS THE GENERIC PARAMETER AND PASSING IN THE XDIR & YDIR AS THE DIRECTION TO MOVE IN
        AttemptMove<Player>(xDir, yDir);
  
    }

    // PROTECTED OVERRIDE
    // => OVERRIDES THE ABSTRACT FUNCTION ATTEMPTMOVE OF THE BASE CLASS MOVINGOBJECT
    // IS CALLED WHEN THE ENEMY MOVES INTO A SPACE THAT IS OCCUPIED BY THE PLAYER
    // => TAKES GENERIC PARAMETER T: THE COMPONENT THE ENEMY ENCOUNTERS - IN THIS CASE: THE PLAYER
    // => TAKES A PARAMETER COMPONENT OF THE TYPE T
    protected override void OnCantMove<T>(T component)
    {
        
        // DECLARE A VARIABLE HITPLAYER OF THE TYPE PLAYER 
        // => SET EQUAL THE COMPONENT PASSED IN AND CAST TO PLAYER
        Player hitPlayer = component as Player;

        // SET THE ENEMYATTACK TRIGGER IN THE ENEMY ANIMATOR CONTROLLER
        animator.SetTrigger("enemyAttack");

        // CALL LOSEFOOD FUNCTION => PLAYERDAMAGE AS POINTS LOSS
        hitPlayer.LoseFood(playerDamage);
         
    }


}
