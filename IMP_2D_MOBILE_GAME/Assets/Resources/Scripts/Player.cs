using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    /* PUBLIC VARIABLES */
    // DAMAGE PALYER APPLIES TO WALL OBJECTS
    public int wallDamage = 1;

    // POINTS PLAYER RECEIVES FOR COLLECTED ITEMS
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;

    // 
    public float restartLevelDelay = 1f;
    public float restartGameDelay = 5f;

    // FOOD TEXT
    public Text foodText;

    // AUDIOCLIPS
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    /* PRIVATE VARIABLES */
    // ANIMATOR 
    private Animator animator;

    // STORES THE PLAYERS SCORE DURING THE LEVEL
    // BEFORE PASSING IT BACK TO THE GAME MANAGER WHEN CHANGING LEVELS
    private int food;
 
    // PROTECTED OVERRIDE => BECAUSE OF DIFFERENT IMPLEMENTATION IN THE PLAYER CLASS
    protected override void Start(){

        // REFERENCE TO ANIMATOR COMPONENT
        animator = GetComponent<Animator>();

        // SET FOOD TO THE VALUE OF PLAYER FOOD POINTS WHICH IS STORED IN THE GAME MANAGER
        food = GameManager.instance.playerFoodPoints;

        // SET FOOD TEXT TO CURRENT FOOD POINTS
        foodText.text = "Food: " + food;

        // CALL START FUNCTION OF THE BASE CLASS MOVINGOBJECT
        base.Start();
	}

    // FUNCTION 
    private void OnDisable(){

        // STORE THE VALUE OF FOOD IN THE GAMEMANAGER AS LEVEL CHANGES
        GameManager.instance.playerFoodPoints = food;

    }

    // FUNCTION 
    void Update() {

        // CHECK IF IT CURRENTLY IS THE PLAYERS TURN
        // => IF NOT: RETURN AND SKIP THE FOLLOWING CODE
        if (!GameManager.instance.playersTurn) {
            return;
        }

        // STORE MOVEMENT DIRECTION
        int horizontal = 0;
        int vertical = 0;
 
        // GET AXIS MOVEMENT => CAST TO INT AND STORE AS HORIZONTAL/VERTICAL
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        // CHECK IF HORIZONTAL MOVEMENT => SET VERTICAL TO 0 TO PREVENT DIAGONAL MOVEMENT
        if(horizontal != 0){
            vertical = 0;
        }
  
        // IF NON 0 VALUE FOR HORIZONTAL/VERTICAL
        if (horizontal != 0 || vertical != 0){
                // CALL ATTEMPTMOVE => PASSING IN THE GENERIC PARAMETER WALL - PLAYER MAY ENCOUNTER A WALL WHICH HE CAN INTERACT WITH
                AttemptMove<Wall>(horizontal, vertical);
            }
         
    }

    // FUNCTION TO ATTEMPT A MOVE FOR THE PLAYER
    // => <T> TO SPECIFIY THE ENCOUNTER OBJECT OF THE MOVING PLAYER
    protected override void AttemptMove<T>(int xDir, int yDir){
        
        // REDUCE FOOD POINTS -1
        food--;

        // SET FOOD TEXT TO CURRENT FOOD AMOUNT
        foodText.text = "Food: " + food;

        // CALL BASE FUNCTION AND PASSING XDIR/YDIR AS PARAMETERS
        base.AttemptMove<T>(xDir, yDir);

        // REFERENCE THE RESULT OF THE LINECAST 
        RaycastHit2D hit;

        // MOVEMENT SOUND
        // IF ABLE TO MOVE
        if(Move(xDir, yDir, out hit)){

            // PLAY RANDOM MOVESOUND
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            
        }

        // CHECK IF GAMEOVER => ANY FOOD POINTS LEFT
        CheckIfGameOver();

        // SET THE PLAYERSTURN VARIABLE IN THE GAMEMANAGER TO FALSE => PLAYERS TURN HAS ENDED
        GameManager.instance.playersTurn = false;
         
    }

    // FUNCTION TO GIVE THE PLAYER THE ABBILITY TO INTERACT WITH THE OTHER OBJECTS ON THE BOARD
    // => FOOD, EXIT; POTION,...
    private void OnTriggerEnter2D(Collider2D other){

        // IF EXIT TAG
        if (other.tag == "Exit") {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        // IF FOOD
        else if(other.tag == "Food"){
            food += pointsPerFood;

            // CHANGE FOODTEXT
            foodText.text = "+" + pointsPerFood + " Food: " + food;

            // PLAY RANDOM EAT SOUND
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            other.gameObject.SetActive(false);
        }
        // IF POTION
        else if(other.tag == "Soda"){
            food += pointsPerSoda;

            // CHANGE FOODTEXT
            foodText.text = "+" + pointsPerSoda + " Food: " + food;

            // PLAY RANDOM DRINK SOUND
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            other.gameObject.SetActive(false);
        }
         
    }
     
    // IMPLEMENTATION OF ONCANTMOVE FOR THE PLAYER (OVERWRITES THE INHERITED ABSTRACT FUNCTION)
    // FOR PLAYER WALKING INTO A WALL
    protected override void OnCantMove<T>(T component){

        // CASTING AS WALL
        Wall hitWall = component as Wall;

        // CALL DAMAGEWALL() => PASS WALLDAMAGE AS PARAMETER - HOW MUCH DAMAGE PLAYER DOES 
        hitWall.DamageWall(wallDamage);

        // SET TRIGGER FOR PLAYER CHOP ANIMATION
        animator.SetTrigger("playerChop");
         
    }

    // FUNCTION TO RESTART THE GAME
    private void Restart(){        
        // LOAD SCENE 1 
        SceneManager.LoadScene(1);
    }

    // FUNCTION TO RESTART THE GAME
    private void MainRestart()
    {
        // LOAD SCENE 0 
        SceneManager.LoadScene(0); 
        SoundManager.instance.musicSource.Play();
    }


    // FUNCTION TO LOSE FOOD WHEN HIT
    public void LoseFood(int loss){

        // SET TRIGGER FOR PLAYER HIT ANIMATION
        animator.SetTrigger("playerHit");

        // REDUCE FOOD BY LOSS
        food -= loss;

        // SHOW FOOD LOSS
        foodText.text = "-" + loss + " Food: " + food;

        // CALL CHECKIFGAMEOVER
        CheckIfGameOver();

    }

    // FUNCTION TO CHECK IF GAME IS OVER
    private void CheckIfGameOver(){

        if (food <= 0){

            // PLAY GAMEOVER SOUND
            SoundManager.instance.PlaySingle(gameOverSound);

            // STOP GAME MUSIC
            SoundManager.instance.musicSource.Stop();

            // CALL THE GAME OVER FUNCTION OF THE GAME MANAGER
            GameManager.instance.GameOver();

            // RESET PLAYER
            food = 100;
            Invoke("MainRestart", restartGameDelay);

        }

    }
 
}
