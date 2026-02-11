using System;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Transform bankTransform;

    [SerializeField] private RectTransform minimapTransform;
    [SerializeField] private RectTransform targetTransform;

    [SerializeField] private float minimapRadius = 100f;

    private void Update()
    {
        UpdateTargetOnMinimap();
    }
    
    public void SetBankTransform(Transform bank) { bankTransform = bank; }

    private void UpdateTargetOnMinimap()
    {
        if (bankTransform == null) return;

        Vector3 direction = bankTransform.position - transform.position;

        // Direction relative au joueur
        Vector3 localDir = transform.InverseTransformDirection(direction);

        Vector2 minimapPos = new Vector2(localDir.x, localDir.z);

        // Normalisation
        minimapPos.Normalize();

        // Taille du carré UI
        float halfWidth = minimapTransform.rect.width * 0.5f;
        float halfHeight = minimapTransform.rect.height * 0.5f;

        // Projection sur carré
        float scale = Mathf.Max(Mathf.Abs(minimapPos.x), Mathf.Abs(minimapPos.y));
        minimapPos /= scale;

        minimapPos.x *= halfWidth;
        minimapPos.y *= halfHeight;

        targetTransform.localPosition = minimapPos;
    }

}
