using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour { 
   
    /* PUBLIC VARIABLES */
    // SOUND SOURCES FOR MUSIC AND EFX: CONTAIN PLAYABLE CLIPS
    public AudioSource efxSource;
    public AudioSource musicSource;

    // SOUNDMANAGER 
    public static SoundManager instance = null; 

    // SOUND PITCH
    // => TO ADD VARIATION TO THE SOUND EFX (+/- 0,5 % PITCH)
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;
     

    /* FUNCTIONS */
    // AWAKE
    void Awake(){

        // SINGLETON PATTERN AS USED IN GAMEMANAGER
        if (instance == null){
            instance = this;
        }
        else if (instance != this){
            // DESTROY THIS GAMEOBJECT
            Destroy(gameObject);
        }

        // DONT DESTROY THE SOUNDMANAGER ON LOAD
        DontDestroyOnLoad(gameObject);

    }

    // FUNCTION TO PLAY SINGLE AUDIO CLIPS
    // => PUBLIC: TO BE CALLED FROM THE OTHER SCRIPTS, MANAGING THE GAME LOGIC
    // => AUDIOCLIP AS PARAMETER: AUDIOCLIPS ARE ASSETS THAT CONTAIN DIGITAL AUDIO RECORDINGS
    public void PlaySingle(AudioClip clip){

        // GET EFX SOURCE CLIP
        efxSource.clip = clip;

        // PLAY CLIP
        efxSource.Play();

    }

    // FUNCTION TO RANDOMIZED PLAY SOUND EFX
    // => PARAMS AUDIOCLIP[] CLIPS: TAKES AN ARRAY OF AUDIOCLIP AS A PARAMETER
    // => PARAMS ALLOWS TO PASS IN A COMMA SEPARATED LIST OF ARGUMENTS OF THE SAME TYPE, AS SPECIFIED BY THE PARAMETER
    public void RandomizeSfx(params AudioClip[] clips){

        // SET RANDOM NUMBER FOR A RANDOM INDEX
        // => RANDOM.RANGE(MINIMUM NUMBER 0, MAXIMUM NUMBER: LENGTH OF CLIPS ARRAY)
        int randomIndex = Random.Range(0, clips.Length);

        // SET RANDOM PITCH VALUE
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        // SET RAMNDOM PITCH TO THE EFXSOURCE
        efxSource.pitch = randomPitch;

        // SET CLIP TO RANDOM CLIP FRPM THE CLIPS ARRAY
        efxSource.clip = clips[randomIndex];

        // PLAY EFX
        efxSource.Play();

    }
	 
}
