using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float spawnTime;
    [SerializeField] private int minToSpawn;
    [SerializeField] private int maxToSpawn;
    [SerializeField] private GameObject prefab;

    [SerializeField] GameObject[] enemies;

    private int spawned;
    private float timer;

    private Transform player;

    private void Start()
    {
        spawned = 0;
        enemies = new GameObject[100];
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        for (int i = 0; i < spawned; i++)
        {
            if (enemies[i].activeInHierarchy == false) continue;
            enemies[i].transform.LookAt(player);
            enemies[i].transform.position += enemies[i].transform.forward * Time.deltaTime * speed;
        }
        timer += Time.deltaTime;
        if (timer > spawnTime)
        {
            timer -= spawnTime;
            int nr = Random.RandomRange(minToSpawn, maxToSpawn + 1);
            for (int i = 0; i < nr; i++)
            {
                Vector3 spawnOffset = Random.insideUnitCircle.normalized * 30;
                spawnOffset.y = 0;
                Vector3 spawnPosition = player.position + spawnOffset;

                GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity);
                enemies[spawned++] = go;
            }
        }
    }
}
