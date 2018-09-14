using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count {
        public int minimum;
        public int maximum;
        public Count (int min, int max) {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 6;
    public int rows = 6;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitalizeList() {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++) {
            for (int y = 1; y < rows - 1; y++) {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup() {
        boardHolder = new GameObject("Board").transform;
        // Loop from -1 to n+1 because we are establishing the outerWalls around the playable grid
        for (int x = -1; x < columns + 1; x++) {
            for (int y = -1; y < columns + 1; y++) {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows) {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);

            }
        }
    }

    Vector3 RandomPosition() {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i ++) {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level) {
        BoardSetup();
        InitalizeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        // Calculate enemy count logarithmically by level. so difficulty follows a log curve
        int enemyCount = (int)Mathf.Log(level, 2f);
        // Using enemyCount for both min and max to ensure the number of enemies spawned on each level is the same each time. 
        // Could potentially change this so that every level spawns a range of enemies.
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
