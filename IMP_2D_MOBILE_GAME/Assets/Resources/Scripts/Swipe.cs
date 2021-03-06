﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {

    /* PRIVATE VARIABLES */
    
    private bool tap;
    private bool swipeLeft;
    private bool swipeRight;
    private bool swipeUp;
    private bool swipeDown;

    private bool isDragging = false;

    private Vector2 startTouch, swipeDelta;

    /* PUBLIC VARIABLES */
    public Vector2 SwipeDelta { get { return swipeDelta; } }

    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
     
    /* FUNCTIONS */
    // UPDATE FUNCTION 
    void Update() {

        // INITIAL SET VARIABLES TO FALSE
        tap = false;
        swipeLeft = false;
        swipeRight = false;
        swipeUp = false;
        swipeDown = false;

        #region Standalone Inputs
        // MOUSE SWIPE CONTROL => PC USAGE 
        // MOUSE BUTTON PUSH
        if (Input.GetMouseButtonDown(0)) {

            tap = true;
            isDragging = true;
            startTouch = Input.mousePosition;

        }
        // MOUSE BUTTON RELEASE
        else if (Input.GetMouseButtonUp(0)) {

            isDragging = false;
            Reset();

        }
        #endregion

        #region Mobile Input
        // MOBILE SWIPE CONTROL => MOBILE DEVICES
        // CHECK FOR TOUCHES
        if (Input.touches.Length != 0) {

            // AT LEAST ONE TOUCH AND TOUCH PHASE BEGAN WITHING FRAME
            if (Input.touches[0].phase == TouchPhase.Began) {

                // SET ISDRAGGING TO TRUE
                isDragging = true;
                
                // SET TAP TO TRUE
                tap = true;

                // GET TOUCH POSITION
                startTouch = Input.touches[0].position; 
            }
            // CHECK IF TOUCH ENDED OR CANCELED
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {

                // RESET ISDRAGGING
                isDragging = false;

                // CALL RESET FUNCTION
                Reset(); 
            } 
        }

        #endregion

        // CALCULATE THE SWIPE DISTANCE 
        // RESET SWIPE
        swipeDelta = Vector2.zero;

        // CHECK IF USER IS DRAGGING
        if (isDragging){
           
            // IF TOUCH LENGTH IS > 0
            if (Input.touches.Length > 0) {
                // GET SWIPEDELTA - SWIPE DISTANCE
                swipeDelta = Input.touches[0].position - startTouch; 
            }
            
            // ELSE IF GET MOUSE BUTTON 
            else if(Input.GetMouseButton(0)){
                // GET SWIPEDELTA - MOUSE SWIPE DISTANCE
                swipeDelta = (Vector2)Input.mousePosition - startTouch; 
            }             
        }

        // CROSSED THE DEAD ZONE FOR SWIPE => OTHERWISE NO SWIPE MOVEMENT 
        if (swipeDelta.magnitude > 125){

            // X/Y COORDINATES
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            // LEFT OR RIGHT
            if (Mathf.Abs(x) > Mathf.Abs(y))
            { 
                // LEFT
                if (x < 0)
                {
                    swipeLeft = true;
                }
                // RIGHT
                else{
                    swipeRight = true;
                } 
            }
            // UP OR DOWN
            else { 
                // DOWN
                if (y < 0)
                {
                    swipeDown = true;
                }
                // UP
                else{
                    swipeUp = true;
                } 
            }
            
            // CALL RESET FUNCTION
            Reset(); 
        }
         
    }

    // FUNCTION TO RESET
    // => RESET VARIABLES FOR STARTTOUCH, SWIPEDELTA AND ISDRAGGING
    private void Reset(){

        startTouch = Vector2.zero;
        swipeDelta = Vector2.zero;

        isDragging = false; 
    }

}
