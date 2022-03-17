using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float secondsBetweenSpawns;
    [SerializeField] int maxSquadSize = 5;
    [SerializeField] int minDistance = 10;
    [SerializeField] int maxDistance = 100;
    [SerializeField] float maxSpeed = 6f;
    System.Random random = new System.Random();
    private GameObject player;
    private Transform enemies;

    public GameObject Player { get => player; set => player = value; }

    void Start()
    {
        enemies = GameObject.Find("/Enemies").transform;
        StartCoroutine(spawnTargets());
    }
    private IEnumerator spawnTargets()
    {
        while (true)
        {
            if (GameManager.Instance.gameState.Equals(GameState.Game) && player != null)
            {
                float x, z;
                float distanceX = minDistance + UnityEngine.Random.value * maxDistance;
                if (UnityEngine.Random.value < 0.5)
                {
                    distanceX = -distanceX;
                }
                float distanceY = minDistance + UnityEngine.Random.value * maxDistance;
                if (UnityEngine.Random.value < 0.5)
                {
                    distanceY = -distanceY;
                }
                x = Player.transform.position.x + distanceX;
                z = Player.transform.position.z + distanceY;
                float speedX = maxSpeed * 2 * UnityEngine.Random.value - maxSpeed;
                float speedZ = maxSpeed * 2 * UnityEngine.Random.value - maxSpeed;
                float rotation = (float)(Math.Atan2(speedX, speedZ) * 180 / Math.PI);
                int squadSize = random.Next(1, maxSquadSize);
                for (int k = 0; k < squadSize; k++)
                {
                    Vector3 spawnLocation = new Vector3(x, 0, z);
                    spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);
                    GameObject spawned = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
                    spawned.transform.parent = enemies;
                    Enemy enemy = spawned.gameObject.GetComponent<Enemy>();
                    enemy.SpeedX = speedX;
                    enemy.SpeedZ = speedZ;
                    enemy.Player = Player;
                    x += (float)Math.Sin(rotation) * 3f;
                    z += (float)Math.Cos(rotation) * 3f;
                }
            }

            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }
}
