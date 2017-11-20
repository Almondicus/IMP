using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
	//Wall makes it possible to destroy walls

	//changing sprites for a damaged wall
	public Sprite dmgSprite;
	//healthPoints of the wall --> hits you need to destroy it
	public int hp = 4;

	private SpriteRenderer spriteRenderer;

	//initialisation of the spriteRenderer
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	//function to damage a wall
	public void DamageWall(int loss){
		//change the sprite
		spriteRenderer.sprite = dmgSprite;
		//decrease the healthPoints
		hp -= loss;

		//deactivate the wall 
		if (hp < 0)
			gameObject.SetActive (false);
	}
}
