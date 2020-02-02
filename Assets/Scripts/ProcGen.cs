using UnityEngine;
using System.Collections.Generic;

public class ProcGen : MonoBehaviour
{
    public Transform ship;

    public float minDist;
    public int chunkSize;
    public GameObject[] obstaclePrefabs;
    public MovingObstacle[] movingPrefabs;
    public GameObject fuelPrefab;
    public float percentObstacle;
    public float percentFuel;
    public float percentMovingObstacle;
    public float fuzz;
    public int seed;
    public float maxSizeAdd;


    HashSet<Vector2Int> builtChunks = new HashSet<Vector2Int>();

    void Start()
    {
        minDist = minDist== 0 ? 1 : minDist;
    }

    void Update()
    {
        int x, y;
        x = (int)ship.position.x / chunkSize;
        y = (int)ship.position.y / chunkSize;

        x = ship.position.x < 0 ? x - 1 : x;
        y = ship.position.y < 0 ? y - 1 : y;

        var currChunk = new Vector2Int(x, y);
        //Debug.Log(currChunk);

        for (int i = currChunk.x - 1; i <= currChunk.x + 1; i++) {
            for (int j = currChunk.y - 1; j <= currChunk.y + 1; j++)
            {
                var tChunk = new Vector2Int(i, j);
                if (builtChunks.Contains(tChunk)) continue;

                //Debug.Log($"generate {tChunk}");

                builtChunks.Add(tChunk);
                Generate(tChunk.x, tChunk.y);
            }
        }

    }

    void Generate(int chunkX, int chunkY)
    {
        var hash = GenerateHash(chunkX, chunkY, seed);
        Random.InitState(hash);
        var fuzzRand = new System.Random();

        var startX = chunkX * chunkSize;
        var startY = chunkY * chunkSize;

        for (float i = startX; i < startX + chunkSize; i += minDist)
        {
            for (float j = startY; j < startY + chunkSize; j += minDist)
            {
                var val = Random.value;

                var f =  (float)fuzzRand.NextDouble() * fuzz;
                var pos = new Vector3(i + f, j + f, 0);
                
                var s = (float)fuzzRand.NextDouble() * maxSizeAdd;

                if (val < percentMovingObstacle)
                {
                    MovingObstacle mo = Instantiate(movingPrefabs[Random.Range((int)0, (int)movingPrefabs.Length)], pos, Quaternion.identity);
                    mo.Initialize(ship);
                    mo.transform.localScale = new Vector3(1 + s, 1 + s, 1 + s);
                }
                else if (val < percentFuel)
                {
                    var go = Instantiate(fuelPrefab, pos, Quaternion.identity);
                }
                else if(val < percentObstacle)
                {
                    var go = Instantiate(obstaclePrefabs[Random.Range((int)0, (int)obstaclePrefabs.Length)], pos, Quaternion.identity);
                    go.transform.localScale = new Vector3(1 + s, 1 + s, 1 + s);
                }
                
            }
        }
    }


    int GenerateHash(int x, int y, int seed) {
        int hash = 23;
        hash = hash * 31 + x;
        hash = hash * 31 + y;
        hash = hash * 31 + seed;
        return hash;
    }
}
