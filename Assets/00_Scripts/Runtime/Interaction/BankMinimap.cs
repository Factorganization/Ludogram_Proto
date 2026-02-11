using System;
using UnityEngine;

public class BankMinimap : MonoBehaviour
{
    private void Start()
    {
        FindAnyObjectByType<Minimap>().SetBankTransform(transform);
    }
}
