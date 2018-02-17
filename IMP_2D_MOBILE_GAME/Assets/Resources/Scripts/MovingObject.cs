using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    // PUBLIC VARIBLES
    public float moveTime = 0.1f; // TIME WHICH TAKES THE OBJECT TO MOVE IN SECONDS
    // PLAYER, ENEMIES AND WALLS ARE PLACED ON THIS LAYER => BLOCKINGLAYER
    public LayerMask blockingLayer; // LAYER ON WHICH COLLISION IS CHECKED => SPACE IS OPEN TO BE MOVE INTO

    // PRIVATE VARIABLES
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D; // STORE A COMPONENT REFERENCE TO THE 2D COMPNENT OF THE MOVING UNIT
    private float inverseMoveTime; // FOR MOVEMENT CALCULATION
     
    // PROTECTED VIRTUAL CLASS => CAN BE OVEWRITTEN BY INHERITING CLASSES
    // => IN CASE INHERITING CLASS NEEDS DIFFERENT IMPLEMENTATION OF START()
	protected virtual void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // STORING THE RECIPROCAL OF MOVETIME
        // => USING IT FOR MULTIPLYING IS MORE EFFICIENT COMPUTATIONALLY THAN USING IT FOR DIVIDING
        inverseMoveTime = 1f / moveTime; 
         
	}

    // FUNCTION TO MOVE AND DETECT A HIT
    // => BOOL RETURN TYPE OF THE FUNCTION
    // => "OUT" FOR ALSO RETURNING RAYCASTHIT2D HIT
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){

        // CURRENT TRANSFORM POSITION 
        Vector2 start = transform.position;

        // CALCULATE THE END POSITION BASED ON THE X/Y PARAMETERS PASSED IN WHEN CALLIN MOVE(..XDIR,..YDIR,..)
        Vector2 end = start + new Vector2(xDir, yDir);

        // DISABLE BOXCOLLIDER => CASTED ARRAY DOES NOT HIT OWN COLLIDER
        boxCollider.enabled = false;

        // CAST A LINE FROM START-POINT TO END-POINT CHECKING COLLISION ON BLOCKING LAYER
        // => LINECAST(VECTOR2 START => START POSITION, VECTOR2 END => END POSITION, INT LAYERMASK => USED BLOCKINGLAYER)
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // REENABLE BOXCOLLIDER
        boxCollider.enabled = true;

        // CHECK IF ANYTHING WAS HIT => HIT.TRANSFORM == NULL
        if(hit.transform == null){
            // IF SPACE CASTED LINE IS ON IS OPEN AND AVAILABLE TO MOVE INTO
            // => STARTCOROUTINE AND PASSING IN THE PARAMETER END
            StartCoroutine(SmoothMovement(end));

            // RETURN TRUE => ABLE TO MOVE
            return true;
        }

        // ELSE RETURN FALSE => NOT SUCCESSFULL
        return false; 
    }


    // FUNCTION FOR MOVING UNITS FROM ONE SPACE TO THE NEXT
    protected IEnumerator SmoothMovement(Vector3 end){

        // THE REMAINING DISTANCE BASED ON THE SQUARE MAGNITUDE
        // => DISTANCE OF THE CURRENT POSITION AND THE END PARAMETER
        // SQRMAGNITUDE IS COMPUTATIONALLY CHEAPER THAN MAGNITUDE (ROOT CALCULATION)
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // WHILE SQRREMAININGDISTANCE IS GREATER THAN FLOAT.EPSILON (ALMOST ZERO)
        // => FLOAT.EPSILON DUE TO DIFFICULTIES WITH PRECISION, COMPARING 2 FLOATS
        while(sqrRemainingDistance > float.Epsilon){

            // NEW POSITION PROPORTIONALLY CLOSER TO THE END BASED ON THE MOVETIME
            // VECTOR.MOVETORWARDS() MOVES A POINT ON A STRAIGHT LINE TO A NEW POINT
            // => MOVETORWARDS(CURRENT POSITION OF THE RIGIDBODY2D, END => POSITION TRYING TO MOVE TO, MAX DISTANCE DELTA);
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            // MOVE TO NEW POSITION
            rb2D.MovePosition(newPosition);

            // RECALCULATE THE REMAINING DISTANCE AFTER THE MOVE
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            // YIELD.RETURN NULL WAITS FOR A FRAME BEFORE RE-EVALUATING THE CONDITION OF THE LOOP
            yield return null;

        }         
    }

    // PROTECTED VIRTUAL FUNCTION
    // => TAKES A GENERIC PARAMETER T AND A PARAMETER COMPONENT OF THE TYPE T 
    // T IS USED TO SPECIFY THE TYPE OF COMPONENT THE UNIT IS EXPECTED TO INTERACT WITH IF BLOCKED
    // => IN CASE OF ENEMEY: PLAYER
    // => IN CASE OF PLAYER: WALLS (PLAYER CAN DESTROY WALLS)
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;

        // SET CAN MOVE TO THE RETURN VALUE OF MOVE()
        // => TRUE: IF MOVE WAS SUCCESSFULL / FALSE: IF MOVE FAILED
        bool canMove = Move(xDir, yDir, out hit);

        // HIT IS AN "OUT" PARAMETER FROM MOVE()
        // => ALLOWS TO CHECK IF THE TRANSFORM HIT IN MOVE IS NULL
        if(hit.transform == null){
            // IF NOTHING WAS HIT BY THE LINECAST IN MOVE() => RETURN AND SKIP THE FOLLOWING CODE
            return;
        }

        // IF SOMETHING WAS HIT
        // => GET A COMPONENT REFERENCE TO THE COMPONENT OF THE TYPE T, ATTACHED TO THE OBJECT THAT WAS HIT
        T hitComponent = hit.transform.GetComponent<T>();

        // IF CANMOVE: FALSE AND HITCOMPONENT NOT EQUAL NULL => MOVING OBJECT IS BLOCKED AND HIT SOMETHING IT CAN INTERACT WITH
        if(!canMove && hitComponent != null){
            // CALL ONCANTMOVE AND PASS HITCOMPONENT AS A PARAMETER
            OnCantMove(hitComponent);
        }

    }

    // PROTECTED ABSTRACT FUNCTION ONCANTMOVE
    // => TAKES A GENERIC PARAMETER T AND A PARAMETER COMPONENT OF THE TYPE T 
    // BE OVERWRITTEN BY FUNCTIONS IN THE INHERITING CLASSES
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
     
}
