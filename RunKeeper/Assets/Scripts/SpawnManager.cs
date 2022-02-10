using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> obstacles;
    [SerializeField] List<GameObject> boosts;
    [SerializeField] List<GameObject> enemies;
    private float minRange = 0.0f;
    private float maxRange = 2.0f;
    int countInRow = 0;
    int prevObstacle;
    public float spawnTimeMin;
    public float spawnTimeMax;

    public float spawnTimeBoostMin;
    public float spawnTimeBoostMax;

    public float spawnTimeEnemyMin;
    public float spawnTimeEnemyMax;

    private float spawnTime = 5;

    private void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(SpawnBoost());
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator Spawn()
    {
        while (!GameManager.Instance.isGameOver)
        {
            spawnTime = Random.Range(spawnTimeMin, spawnTimeMax);

            yield return new WaitForSeconds(spawnTime);

            int obstacleRandom = Random.Range(0, obstacles.Count);

            if (obstacleRandom == prevObstacle)
            {
                countInRow++;
                if (countInRow >= 3)
                {
                    obstacleRandom = Random.Range(0, 2);
                }
            }
            else
            {
                countInRow = 0;
            }

            Vector3 randomPos = Vector3.zero;

            if (obstacleRandom == 0 || obstacleRandom == 2 || obstacleRandom == 3)
            {
                randomPos = new Vector3(11.0f, Random.Range(minRange, maxRange), 0);
            }
            if (obstacleRandom == 1)
            {
                randomPos = new Vector3(15.0f, 1.33f, 0);
            }
            ObjectPool.Instance.CreateObject(obstacles[obstacleRandom], randomPos);
            prevObstacle = obstacleRandom;
        }
    }

    IEnumerator SpawnBoost()
    {
        while (!GameManager.Instance.isGameOver)
        {
            spawnTime = Random.Range(spawnTimeBoostMin, spawnTimeBoostMax);
            int isAdrenalineSpawn = Random.Range(0, 5);
            yield return new WaitForSeconds(spawnTime);

            Vector3 randomPos = new Vector3(11.0f, Random.Range(minRange, maxRange), 0);
            if (isAdrenalineSpawn == 3)
            {
                ObjectPool.Instance.CreateObject(boosts[1], randomPos);
            }
            else
            {
                ObjectPool.Instance.CreateObject(boosts[0], randomPos);
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (!GameManager.Instance.isGameOver)
        {
            float spawnTimeEnemy = Random.Range(spawnTimeEnemyMin, spawnTimeEnemyMax);
            int randomEnemy = Random.Range(0, enemies.Count);
            yield return new WaitForSeconds(spawnTime);

            Vector3 randomPos = new Vector3(11.0f, Random.Range(minRange, maxRange), 0);
            ObjectPool.Instance.CreateObject(enemies[0], randomPos);

        }
    }
}
