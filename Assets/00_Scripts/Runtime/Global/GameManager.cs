using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager() {}
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    public float CurrentMoney => currentMoney;
    
    private float currentMoney=0;
    private int bankDeliveredCount;
    private float moneyDeliveredTotal;

    [SerializeField] private int maxMoneyVan;
    [SerializeField] private int bankMoneyAmount;
    [SerializeField] private bool debug;
    [SerializeField] MoneyAmount moneyPile;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        UpdateMoneyAmount(maxMoneyVan);
    }

    public void OnBankInteraction(Bank bank)
    {
        if (!bank.delivered)
        {
            moneyDeliveredTotal += currentMoney;
            UpdateMoneyAmount(-currentMoney);
            bankDeliveredCount++;
            bank.delivered = true;
        }
        else 
        {
            if(currentMoney == 0)
                UpdateMoneyAmount(bankMoneyAmount);
        }
    }
    
    public void UpdateMoneyAmount(float addedAmount)
    {
        float newTotal = currentMoney + addedAmount;

        if (newTotal > maxMoneyVan)
        {
            currentMoney = maxMoneyVan;
        }
        else if(newTotal <0)
        {
            currentMoney = 0; //Defeat ?
        }
        else
        {
            currentMoney = newTotal;
        }
        
        moneyPile?.UpdateAmount(currentMoney/maxMoneyVan);
        
        if(debug)
            Debug.Log("New Amount Of Money "+CurrentMoney);
    }
}
