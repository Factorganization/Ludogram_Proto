using System;
using MortierFu.Shared;
using UnityEngine;
using Obi;

public class RopeTensionMonitor : MonoBehaviour
{
    public ObiRope rope;
    public float tensionThreshold = 100f; // Seuil pour considérer la corde "en tension"
    
    private bool isUnderTension = false;
    
    public bool IsUnderTension => isUnderTension;

    private void Awake()
    {
        rope = GetComponent<ObiRope>();
    }

    void FixedUpdate()
    {
        if (!rope || !rope.isActiveAndEnabled)
            return;
            
        float maxForce = GetMaxRopeTension();
        
        // Vérifier si la corde est en tension
        bool wasTensioned = isUnderTension;
        isUnderTension = maxForce > tensionThreshold;
        
        if (isUnderTension && !wasTensioned)
        {
            Logs.Log($"[RopeTensionMonitor] Rope is now under tension! Max force: {maxForce}");
        }
        else if (!isUnderTension && wasTensioned)
        {
            Logs.Log($"[RopeTensionMonitor] Rope is no longer under tension. Max force: {maxForce}");
        }
        
        Logs.Log($"[RopeTensionMonitor] Max rope tension: {maxForce}");
    }
    
    float GetMaxRopeTension()
    {
        float maxForce = 0f;
        float substepTime = Time.fixedDeltaTime / rope.solver.substeps;
        float sqrTime = substepTime * substepTime;
        
        var dc = rope.GetConstraintsByType(Oni.ConstraintType.Distance) as ObiConstraints<ObiDistanceConstraintsBatch>;
        var sc = rope.solver.GetConstraintsByType(Oni.ConstraintType.Distance) as ObiConstraints<ObiDistanceConstraintsBatch>;
        
        if (dc != null && sc != null)
        {
            for (int j = 0; j < rope.solverBatchOffsets[(int)Oni.ConstraintType.Distance].Count; ++j)
            {
                var solverBatch = sc.batches[j] as ObiDistanceConstraintsBatch;
                int offset = rope.solverBatchOffsets[(int)Oni.ConstraintType.Distance][j];
                var batch = dc.GetBatch(j) as ObiDistanceConstraintsBatch;
                
                for (int i = 0; i < batch.activeConstraintCount; i++)
                {
                    float force = Mathf.Abs(solverBatch.lambdas[offset + i] / sqrTime);
                    if (force > maxForce)
                        maxForce = force;
                }
            }
        }
        
        return maxForce;
    }
}