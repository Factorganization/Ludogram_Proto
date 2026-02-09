using System;
using UnityEngine;

public class MoneyAmount : MonoBehaviour
{
    private float fillPercentage = 1;
    private float maxYSize;

    private void Start()
    {
        maxYSize = transform.localScale.y;
    }

    void UpdateAmount(float newPercentage)
    {
        fillPercentage = Mathf.Clamp(newPercentage, 0, 1);
        transform.localScale = new Vector3(transform.localScale.x,newPercentage * maxYSize,transform.localScale.z);
    }

    [ContextMenu("Retire Money")]
    void UpdateAmountTest()
    {
        UpdateAmount(fillPercentage - 0.25f);
    }
}
