using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {
	
	//This class is written for Player AND enemy
	 

	//time the object needs in seconds
	public float moveTime = 0.1f;
	//checks if the object can move here
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	//reference to the object we want to move
	private Rigidbody2D rb;
	//for efficiency in movement
	private float inverseMoveTime;



	//Start() initializes boxCollider and rb and sets the speed
	// protected so it can be overwritten
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();

		//like this we can use it through multiplication
		inverseMoveTime = 1 / moveTime;
	}


	//Move() checks if we are able to move to the aspired space and returns a bool
	//if we are able to move SmoothMovement() is started
	protected bool Move(int xDir, int yDir, out RaycastHit2D hit){
		//get the current position
		//Vector2 --> no movement in z
		Vector2 start = transform.position;
		//calculate the end position
		Vector2 end = start + new Vector2 (xDir, yDir);
		//disable boxCollider --> no collision with casted ray
		boxCollider.enabled = false;
		//check for collision on blockingLayer between starting position and end
		hit = Physics2D.Linecast (start, end, blockingLayer);
		//reenable boxCollider
		boxCollider.enabled = true;


		//if the layer is free to be moved to
		if(hit.transform == null){
			//moving
			StartCoroutine (SmoothMovement (end));
			//return boolean that we moved
			return true;
		}
		//else return boolean that we could not move
		return false;
	}



	//everytime we attempt to move AttemptMove() checks what to do if we hit something
	//T = component type to interacte with if blocked
	protected virtual void AttemptMove<T> (int xDir, int yDir)
		where T : Component{

		RaycastHit2D hit;
		//canMove = true, if Move() was successful
		bool canMove = Move (xDir, yDir, out hit);

		//out hit --> check IF we hit something and WHAT we hit
		if (hit.transform == null)
			//if we didn't hit anything nothing happens
			return;
		T hitComponent = hit.transform.GetComponent<T>();
		//if canMove == false and we have hit something
		if (!canMove && hitComponent != null)
			//depending on the object (player, enemy) something happens
			OnCantMove (hitComponent);
	}




	//transform the position smoothly
	//end in the middle of the next field
	protected IEnumerator SmoothMovement(Vector3 end){
		//getting the distance to the next destination
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//check the remaining distance until very close to the next field
		//Epsilon = actual difference between the position vectors
		while(sqrRemainingDistance > float.Epsilon){
			// set the new position and move there (from, to, speed)
			Vector3 newPosition = Vector3.MoveTowards (rb.position, end, inverseMoveTime * Time.deltaTime);
			//set the rigidbodys new Position
			rb.MovePosition (newPosition);
			//set the new distance that we still need to go
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//waiting for the next frame before reevaluating the new condition for the loop
			yield return null;
		}
	}


	//will be overwritten
	protected abstract void OnCantMove<T> (T component)
		where T : Component;
	
}
