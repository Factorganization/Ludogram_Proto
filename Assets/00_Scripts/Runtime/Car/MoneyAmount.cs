using System;
using UnityEngine;

public class MoneyAmount : MonoBehaviour
{
    [SerializeField] float fillPercentage = 0;
    [SerializeField] float maxYSize=1;
    [SerializeField] private Transform boxCollider;
    [SerializeField] private SkinnedMeshRenderer moneyPile;
    [SerializeField] private float yOffset=1;



    private void Update()
    {
        UpdateAmount(fillPercentage);
    }

    public void UpdateAmount(float newPercentage)
    {
        fillPercentage = Mathf.Clamp(newPercentage, 0, 1);
        if(boxCollider)
            boxCollider.localScale = new Vector3(transform.localScale.x,newPercentage * maxYSize,transform.localScale.z);
        if (moneyPile)
        {
            moneyPile.transform.localPosition = Vector3.up * fillPercentage * yOffset;
            moneyPile.SetBlendShapeWeight(0,fillPercentage * 100);
            moneyPile.SetBlendShapeWeight(1,fillPercentage * 100);
        }

    }

    [ContextMenu("Retire Money")]
    void UpdateAmountTest()
    {
        UpdateAmount(fillPercentage - 0.25f);
    }
}
