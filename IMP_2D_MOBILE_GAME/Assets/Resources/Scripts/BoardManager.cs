using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    // CLASS FOR COUNT
    [Serializable]
    public class Count {
        public int minimum;
        public int maximum;

        // CONSTRUCTOR MIN AND MAX AMOUNT OF WALLS
        public Count(int min, int max){
            minimum = min;
            maximum = max;
        }
    }

    // NUMBER OF ROWS AND COLUMNS FOR THE GAMEBOARD => PUBLIC FOR RESIZE
    public int columns = 8;
    public int rows = 8;

    // RANDOM RANGE FOR HOW MANY INNER WALLS SPAWNED IN EVERY LEVEL => RANGE BETWEEN MIN AND MAX (5,9)
    public Count wallCount = new Count(5,9); 

    // RANDOM RANGE FOR HOW MANY FOOD ITEMS ARE SPAWNED IN EVERY LEVEL => RANGE BETWEEN MIN AND MAX 
    public Count foodCount = new Count(1,5);

    // GAME OBJECTS => FILLED WITH PREFABS IN THE INSPECTOR
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // TRANSFORM AS BOARDHOLDER => FOR KEEPING THE HIERARCHY CLEAN
    // BECAUSE WE ARE SPAWNING A LOT OF GAMEOBJECTS SO THEY ARE ALL CHILDED TO THE BOARDHOLDER
    // (COLLAPSES IN THE HIERARCHY AND NOT FILLING UP THE HIERARCHY WITH A LOT OF GAMEOBJECTS)
    private Transform boardHolder;

    // TO TRACK ALL THE POSSIBLE POSITIONS ON THE GAMEBOARD
    // AND KEEP TRACK EITHER A GAMEOBJECT HAS BEEN SPAWNED IN THAT POSITION OR NOT
    private List<Vector3> gridPositions = new List<Vector3>();

    // FUNCTION FOR INITIALIZING THE LIST
    void InitialiseList() {
        // CLEARING GRID POSITIONS
        gridPositions.Clear();

        // ITERATE ALL POSSIBLE X/Y-COORDINATES OF THE GAMEBOARD GRID
        // RUNNING FROM 1 AND COLUMNS/ROWS - 1 FOR LEAVING A SPACE OF 1 BETWEEN THE OUTER TILES AND THE OUTERWALL
        // NO OBJECTS SPAWNED IN THE OUTER TILES
        for (int x = 1; x < columns - 1 ; x++) {
            for (int y = 1; y < rows - 1; y++) {
                // FILL THE GRIDPOSITION LIST WITH A VECTOR3 FOR EVERY CELLS
                // LIST OF POSSIBLE POSITIONS FOR WALLS, ENEMIES OR PICKUPS
                gridPositions.Add(new Vector3(x,y,0f));
            }
        } 
    }

    // FUNCTION FOR SETTING UP THE BOARD
    // OUTER WALL AND FLOOR OF THE GAMEBOARD
    void BoardSetup() {
        // BORADHOLDER => HOLDING A NEW GAMEOBJECT "BOARD"
        boardHolder = new GameObject("Board").transform;

        // "LAYING OUT" THE FLOOR AND WALL TILES
        // GOING FROM -1 TO COLUMS/ROWS + 1 TO BUILD AN EDGE AROUND THE ACTIVE "PLAYABLE" PART OF THE GAMEBOARD WITH THE OUTER WALL 
        for(int x = -1; x < columns + 1; x++){
            for(int y = -1; y < rows + 1; y++){
                // CHOOSING A RANDOM FLOORTILE FROM THE FLOORTILES ARRAY => RANDOM.RANGE(0 FOR MIN INDEX, FLOORTILES.LENGTH FOR THE MAX INDEX OF THE ARRAY)
                GameObject toInstatiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // CHECK IF "INSIDE" ONE OF THE OUTER WALL TILES => X OR Y == -1 OR == MAX VALUE OF COLUMNS/ROWS
                if (x == -1 || x == columns || y == -1 ||y == rows){
                    // SETTING UP THE TILE AS AN OUTER WALL TILE FROM THE OUTERWALLTILES ARRAY
                    toInstatiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];                   
                }

                // ACTUALLY INSTATIATING THE CHOSEN TILE TOINSTANTIATE:
                // INSTANTIATE()
                // TOINSTANTIATE => THE CHOSEN PREFAB TO INSTANTIATE,
                // NEW VECTOR3(X,Y,0F) => CURRENT X/Y-COORDINATES AND Z TO 0F (BECAUSE IN 2D)
                // QUATERNION.IDENTITY => FOR NO ROATION
                // AS GAMEOBJECT => CAST AS GAMEOBJECT
                GameObject instance = Instantiate(toInstatiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;

                // SET PARENT OF INSTANCE TO BOARDHOLDER
                instance.transform.SetParent(boardHolder);
                 
            }
        }
    }

    // FUNCTION FOR SETTING UP THE RANDOM OBJECTS WALLS, ENEMIES AND POWERUPS 
    Vector3 RandomPosition(){
        // RANGE BETWEEN 0 AND THE NUMBER OF POSITIONS STORED INSIDE THE GRIDPOSITIONS LIST    
        int randomIndex = Random.Range(0, gridPositions.Count);

        // CREATING A VECTOR3 RANDOMPOSITION AND SETTING THIS EQUAL TO THE POSITION SAVED INSIDE THE GRIDPOSITION LIST AT THE RANDOM INDEX
        Vector3 randomPosition = gridPositions[randomIndex];

        // PREVENT SPAWNING OF 2 OBJECTS AT THE SAME GRID POSITION => REMOVE THIS POSITION FROM THE GRIDPOSITIONS LIST
        gridPositions.RemoveAt(randomIndex);

        // RETURN THE RANDOM POSITION
        return randomPosition;
    }

    // FUNCTION TO ACTUALLY SPAWN THE TILE AT THE RANDOM POSITION
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){

        // OBJECTCOUNT => TO CONTROL HOW MANY OF THE GIVEN OBJECTS ARE SPAWNED
        int objectCount = Random.Range(minimum, maximum + 1);

        // LOOP THROUGH THE NUMBER OF OBJECTS TO SPAWN
        for(int i = 0; i < objectCount; i++){
            // GET RANDOM POSITION => RANDOMPOSITION()
            Vector3 randomPosition = RandomPosition();
            // CHOOSE A RANDOM TILE FROM THE TILEARRAY => RANDOM POSITION WITH RANDOM.RANGE(0 FOR MIN INDEX, TILEARRAY.LENGTH FOR THE MAX INDEX OF THE ARRAY)
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // INSTANTIATE THE CHOSEN GAMEOBJECT => INSTANTIATE(GAMEOBJECT TILECHOICE, POSITION => RANDOMPOSITION, QUATERNION.IDENTITY => NO ROTATION) 
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // PUBLIC FUNCTION SETUPSCENE => TO BE CALLED BY THE GAMEMANAGER TO SET UP THE BOARD/SCENE
    public void SetupScene(int level){

        // SETTING UP THE BOARD
        BoardSetup();
       
        // INITIALIZING THE GRIDPOSITIONS
        InitialiseList();
       
        // LAY OUT WALL TILES
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
       
        // LAY OUT FOOD TILES 
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        
        // ENEMIES 
        // (INT) => MATHF.LOG RETURNS A FLOAT SO CAST IT TO INT
        // LOG => LOGARTIHM OF LEVEL IN BASE 2F
        // 1 ENEMY AT LVL 2, 2 ENEMIES AT LVL 4, 3 ENEMIES AT LVL 8 AND SO ON...
        int enemyCount = (int)Mathf.Log(level, 2f);

        // LAY OUT ENEMIES
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // INSTANTIATE EXIT TILE
        // ALLWAYS IN THE TOP RIGHT POSITION => COLUMNS - 1, ROWS - 1 (RESPONSIVE TO RESIZE OF THE GAMEBOARD)
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);

    }


}
