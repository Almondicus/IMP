using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    /* PUBLIC VARIABLES */
    // SPRITE DISPLAYED AFTER PLAYER HIT THE WALL
    public Sprite dmgSprite;
    // HIT POINTS
    public int hp = 4;

    // AUDIOCLIPS
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    /* PRIVATE VARIABLES */
    private SpriteRenderer spriteRenderer;

    // AWAKE FUNCTION
    void Awake () {

        // GET AND STORE COMPONENT REFERENCE TO OUR SPRITERENDERER
        spriteRenderer = GetComponent<SpriteRenderer>(); 

	}

    public void DamageWall(int loss){

        // PLAY CHOPSOUND
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        // CHANGE TO DAMAGED SPRITE
        spriteRenderer.sprite = dmgSprite;

        // SUBSTRACT LOSS FROM WALLS CURRENT HP TOTAL
        hp -= loss;

        // CHECK IF WALL HP <= 0
        if(hp <= 0){
            // DISABLE WALL GAME OBJECT
            gameObject.SetActive(false);
        } 

    }
	 
}
