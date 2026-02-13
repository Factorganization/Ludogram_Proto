using System;
using CarScripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int enemyCount, holesCount;
    
    public CarController playerCar;

    [SerializeField] float moneyDeliveredGoal;
    [SerializeField] private int maxMoneyVan;
    [SerializeField] private int bankMoneyAmount;
    [SerializeField] private bool debug;
    [SerializeField] MoneyAmount moneyPile;
    [SerializeField] TextMeshProUGUI moneyText;

    [SerializeField] private Transform endScreen;
    [SerializeField] TextMeshProUGUI scoreEndScreen;
    
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
        Init();
    }

    private void Init()
    {
        if (!playerCar)
            playerCar = FindFirstObjectByType<CarController>();
        
        if (!moneyPile)
            moneyPile = FindFirstObjectByType<MoneyAmount>();
        
        UpdateMoneyAmount(bankMoneyAmount);
        UpdateMoneyText();
    }

    public void OnBankInteraction(Bank bank)
    {
        if (!bank.delivered)
        {
            moneyDeliveredTotal += currentMoney;
            UpdateMoneyAmount(-currentMoney);
            bankDeliveredCount++;
            bank.delivered = true;
            
            EndGame(); 
        }
        else 
        {
            if(currentMoney == 0)
                UpdateMoneyAmount(bankMoneyAmount);
        }

        UpdateMoneyText();
    }

    private void EndGame()
    {
        if (endScreen)
        {
            endScreen.gameObject.SetActive(true);
            scoreEndScreen.text = moneyDeliveredTotal+"$";
        }
    }

    public void ReloadGame()
    {
        if (endScreen)
        {
            endScreen.gameObject.SetActive(false);
            scoreEndScreen.text = Mathf.RoundToInt(moneyDeliveredTotal).ToString();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        
        moneyPile?.UpdateAmount(currentMoney/(maxMoneyVan*1.5f));
        UpdateMoneyText();
        
        if(debug)
            Debug.Log("New Amount Of Money "+CurrentMoney);
    }

    void UpdateMoneyText()
    {
        if (moneyText)
        {
            moneyText.text = "You have :" + (int)currentMoney+ "$                You delivered " + (int)moneyDeliveredTotal + "$";
        }
    }
}
