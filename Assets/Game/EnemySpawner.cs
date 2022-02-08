using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float secondsBetweenSpawns;

    void Start()
    {
        StartCoroutine(spawnTargets());
    }
    private IEnumerator spawnTargets()
    {
        while (true)
        {
            float x, y, z;
            x = Random.value * 400 - 200;
            z = Random.value * 400 - 200;
            Vector3 spawnLocation = new Vector3(x, 0, z);
            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);
            GameObject spawned = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity) as GameObject;
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }
}
