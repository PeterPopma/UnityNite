using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float secondsBetweenSpawns;
    [SerializeField] int maxSquadSize = 5;
    [SerializeField] int maxDistance = 100;
    [SerializeField] float maxSpeed = 6f;
    System.Random random = new System.Random();

    void Start()
    {
        StartCoroutine(spawnTargets());
    }
    private IEnumerator spawnTargets()
    {
        
        while (true)
        {
            float x, z;
            x = player.transform.position.x + (UnityEngine.Random.value * 2 * maxDistance) - maxDistance;
            z = player.transform.position.z + (UnityEngine.Random.value * 2 * maxDistance) - maxDistance;
            float speedX = maxSpeed * 2 * UnityEngine.Random.value - maxSpeed;
            float speedZ = maxSpeed * 2 * UnityEngine.Random.value - maxSpeed;
            float rotation = (float)(Math.Atan2(speedX, speedZ) * 180 / Math.PI);
            int squadSize = random.Next(1, maxSquadSize);
            for (int k = 0; k < squadSize; k++)
            {
                Vector3 spawnLocation = new Vector3(x, 0, z);
                spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);
                GameObject spawned = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity) as GameObject;
                Enemy enemy = spawned.gameObject.GetComponent<Enemy>();
                enemy.SpeedX = speedX;
                enemy.SpeedZ = speedZ;
                x += (float)Math.Sin(rotation) * 3f;
                z += (float)Math.Cos(rotation) * 3f;
            }

            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }
}
