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
    [SerializeField] private List<Collider> targetWallstest;

    private void Start()
    {
        if (!holePrefab)
        {
            Debug.LogError("No HolePrefab reference found");
        }
    }

    [ContextMenu("Generate Hole")]
    public void TestPlaceHole()
    {
        PlaceHoles(targetWallstest,1);
    }
    
    public void PlaceHoles(List<Collider> targetWalls, int amount)
    {
        if (!holePrefab || targetWalls.Count==0 || amount<=0) return;
        
        Collider targetWall = targetWalls[Random.Range(0, targetWalls.Count-1)];
        Vector3 closestPoint = targetWall.ClosestPointOnBounds(transform.position+ Vector3.up * yBoost);
        
        if (Vector3.Distance(closestPoint, transform.position) > distanceMax) return;
        
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = new Vector3(
                targetWall.bounds.center.x, 
                Random.Range(targetWall.bounds.min.y,targetWall.bounds.max.y), 
                Random.Range(targetWall.bounds.min.z,targetWall.bounds.max.z));

            closestPoint.z = targetWall.bounds.center.z;
            
            Vector3 holePos = Vector3.Lerp(randomPos, closestPoint, aimPrecision);
            
            GameObject hole = Instantiate(holePrefab, holePos, targetWall.transform.rotation);
            hole.transform.parent = targetWall.transform;
        }
    }
}
