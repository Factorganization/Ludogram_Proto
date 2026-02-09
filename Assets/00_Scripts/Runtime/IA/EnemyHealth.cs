using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IInteractable //Refacto : enlever monobehaviour et faire instancier par enemybehavior
{
    public int CurrentHealth => currentHealth;
    
    [SerializeField] int maxHealth;
    [SerializeField] private float delayToDestroy;
    [SerializeField] private Material destructionMaterial;

    [SerializeField] private MeshRenderer[] meshRenderers;
    private int currentHealth;
    private float destroyTimer;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            
            var brain = GetComponentInParent<EnemyBehavior>(); // Refacto
            if (brain ) brain.currentSpeed = 0;
            
            LaunchAutoDestruction();
        }
    }

    private void Update()
    {
        if (destroyTimer > 0) // refacto avec un timer manager 
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer < 0)
            {
                AutoDestruction();
            }
        }
    }

    void LaunchAutoDestruction()
    {
        destroyTimer = delayToDestroy;
        if (meshRenderers.Length > 0){
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = destructionMaterial;
            }
        }
}

    void AutoDestruction()
    {
        Destroy(gameObject);
    }

    public Transform GetTransform() => transform;

    public void Interact(IInteractable.InteractAction action, Transform interactor)
    {
        var playerCharacter = interactor.GetComponent<PlayerCharacter>();
        if (playerCharacter != null)
        {
            Interact(playerCharacter);
        }
    }

    public void Interact(PlayerCharacter interactor)
    {
        UpdateHealth(-(maxHealth/3)); // refacto : à changer par une valeur de degat sur l'interactor
    }
}
