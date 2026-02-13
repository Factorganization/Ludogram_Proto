using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrankItUp : MonoBehaviour, IInteractable
{
    [SerializeField] private float crankDuration = 1f;
    [SerializeField] private float resetMaxDistanceDelay = 1f;
    [SerializeField] private float crankCooldown = 2f;
    private bool isCranking = false;
    private float lastCrankTime = -Mathf.Infinity;
    
    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(IInteractable.InteractAction action, Transform interactor)
    {
        if (action == IInteractable.InteractAction.Primary)
        {
            Interact(interactor.GetComponent<PlayerCharacter>());
        }
    }

    public void Interact(PlayerCharacter interactor)
    {
        Crank();
    }
    
    // A function that get all SpringJoint in the scene, and set the maxDistance to 0 progressively, and then after 1 second, set it back to the original maxDistance
    public void Crank()
    {
        if (isCranking || Time.time < lastCrankTime + crankCooldown) return;
        
        StartCoroutine(CrankCoroutine());
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CrankCoroutine()
    {
        isCranking = true;
        lastCrankTime = Time.time;
        
#pragma warning disable CS0618 // Type or member is obsolete
        SpringJoint[] allRopes = FindObjectsOfType<SpringJoint>();
#pragma warning restore CS0618 // Type or member is obsolete
        float[] originalDistances = new float[allRopes.Length];
        
        for (int i = 0; i < allRopes.Length; i++)
        {
            originalDistances[i] = allRopes[i].maxDistance;
        }
        
        float elapsedTime = 0f;
        while (elapsedTime < crankDuration)
        {
            for (int i = 0; i < allRopes.Length; i++)
            {
                allRopes[i].maxDistance = Mathf.Lerp(originalDistances[i], 0f, elapsedTime / crankDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        for (int i = 0; i < allRopes.Length; i++)
        {
            allRopes[i].maxDistance = 0f;
        }
        
        yield return new WaitForSeconds(resetMaxDistanceDelay);
        
        for (int i = 0; i < allRopes.Length; i++)
        {
            allRopes[i].maxDistance = originalDistances[i];
        }
        
        isCranking = false;
    }
}
