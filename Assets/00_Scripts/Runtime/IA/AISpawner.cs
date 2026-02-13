using System;
using CarScripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private Transform playerCar;
    [SerializeField] private float distanceSpawn = 20;
    [SerializeField] float spawnCooldown;
    [SerializeField] private int maxEnemies;
    [SerializeField] private int maxHoles;
    
    [SerializeField] private GameObject[] enemiesPrefab;
    
    private float spawnTimer;


    private void Update()
    {

        if (spawnTimer < spawnCooldown)
        {
            Debug.Log("spawner : waiting for cooldown");
            spawnTimer += Time.deltaTime;
        }
        else
        {
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        //if (playerCar.GetComponent<CarController>()._drivingInputs == null && playerCar.GetComponent<Rigidbody>().linearVelocity.magnitude > 5) ;
        
        if (gm.enemyCount < maxEnemies &&  gm.holesCount < maxHoles)
        {
            spawnTimer = 0;
            Spawn();
        }
        else
        {
            Debug.Log("spawner : conditions not met");
        }
    }

    private void Spawn()
    {
        Debug.Log("spawner : SPAWN");
        gm.enemyCount++;
        
        int randomIndex = Mathf.RoundToInt(Random.Range(0, 1));
        
        Vector2 randomDir = Random.insideUnitCircle.normalized; 
        Vector3 spawnOffset = new Vector3(randomDir.x, 0, randomDir.y) * distanceSpawn;
        Vector3 spawnPos = playerCar.position + spawnOffset;
        
        Instantiate(enemiesPrefab[randomIndex], spawnPos, Quaternion.identity);
    }
}
