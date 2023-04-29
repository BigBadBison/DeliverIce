using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleFactory : MonoBehaviour
{
    public Transform spawn;
    public Transform recycler;
    public Transform target;
    public Transform obstaclePrefab;
    
    public Vector3 offset;
    public float width;
    public float spawnRate;

    List<Transform> obstacles;
    float nextSpawn;

    void Awake()
    {
        spawn.position = target.position + offset;
        recycler.position = target.position - offset;
        nextSpawn = spawn.position.y + spawnRate;
        obstacles = new List<Transform>();
    }

    void Update()
    {
        spawn.position = target.position + offset;
        recycler.position = target.position - offset;

        if (spawn.position.y > nextSpawn)
        {
            Spawn();
            nextSpawn += spawnRate;
        }
    }

    public void Spawn()
    {
        Transform obstacle;
        if (obstacles.Count > 0)
        {
            obstacle = obstacles[obstacles.Count - 1];
            obstacles.RemoveAt(obstacles.Count - 1);
        }
        else
        {
            obstacle = Instantiate(obstaclePrefab, transform);
        }
        obstacle.gameObject.SetActive(true);
        Vector3 position = spawn.position;
        position.x += Random.Range(-width, width);
        
        obstacle.transform.position = position;
        obstacle.transform.Rotate(Vector3.forward, Random.Range(0, 2 * Mathf.PI));
    }
    
    public void Recycle(Transform obstacle)
    {
        obstacle.gameObject.SetActive(false);
        obstacles.Add(obstacle);
    }    
}
