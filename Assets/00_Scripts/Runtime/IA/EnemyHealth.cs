using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IInteractable //Refacto : enlever monobehaviour et faire instancier par enemybehavior
{
    public int CurrentHealth => currentHealth;
    
    [SerializeField] int maxHealth;
    [SerializeField] private float delayToDestroy;
    [SerializeField] private Material destructionMaterial;

    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private ParticleSystem explosionFeedback;
    
    [SerializeField] Rigidbody rigidbody;
    
    private int currentHealth;
    private float destroyTimer, explosionTimer;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        GameManager.Instance.enemyCount++;
    }

    public void UpdateHealth(int amount)
    {
        if (destroyTimer > 0)
            return; 
        currentHealth += amount;

        if (currentHealth < 1)
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

        if (explosionTimer > 0)
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer < 0)
            {
                GameManager.Instance.enemyCount--;
                
                Destroy(GetComponentInParent<EnemyBehavior>().gameObject);
            }
        }
    }

    public void LaunchAutoDestruction()
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
        if(explosionFeedback)
            explosionFeedback.Play();

        explosionTimer = 0.5f;
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
        rigidbody.AddForce(Vector3.up*2000, ForceMode.Impulse);
        UpdateHealth(-1); // refacto : à changer par une valeur de degat sur l'interactor
    }
}
