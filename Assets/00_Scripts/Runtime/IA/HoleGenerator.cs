using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoleGenerator : MonoBehaviour
{
    [SerializeField] private GameObject holePrefab;
    
    [Range(0, 1)]
    [SerializeField] private float aimPrecision;

    [SerializeField] private float distanceMax = 100;
    [SerializeField] private float yBoost = 4;
    
    
    //Test
    public List<Collider> targetWallstest;

    private void Start()
    {
        if (!holePrefab)
        {
            Debug.LogError("No HolePrefab reference found");
        }
    }
    
    
    public void PlaceHoles(List<Collider> targetWalls, int amount)
    {
        if (!holePrefab || targetWalls.Count==0 || amount<=0) return;
        
        Collider targetWall = targetWalls[Random.Range(0, targetWalls.Count-1)];
        Vector3 closestPoint = targetWall.ClosestPoint(transform.position+ Vector3.up * yBoost);
        Debug.DrawLine(transform.position, closestPoint, Color.red,2);
        
        if (Vector3.Distance(closestPoint, transform.position) > distanceMax) return;
        
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(targetWall.bounds.min.x,targetWall.bounds.max.x), 
                Random.Range(targetWall.bounds.min.y,targetWall.bounds.max.y), 
                Random.Range(targetWall.bounds.min.z,targetWall.bounds.max.z));
            
            Vector3 holePos = Vector3.Lerp(randomPos, closestPoint, aimPrecision);

            holePos.x = targetWall.bounds.center.x;
            
            GameObject hole = Instantiate(holePrefab, holePos, targetWall.transform.rotation);
            hole.transform.parent = targetWall.transform;
        }
    }
}
