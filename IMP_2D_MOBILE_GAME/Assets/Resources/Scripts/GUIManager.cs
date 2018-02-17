using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {
     
    /* LOAD SCENE FUNCTION */
    public void LoadScene(string sceneName){

        SceneManager.LoadScene(sceneName);

    }
    
}
