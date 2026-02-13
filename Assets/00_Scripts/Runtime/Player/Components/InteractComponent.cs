using System.Collections.Generic;
using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractComponent : PlayerComponent
{
    private InputAction _interactAction;

    // Layer mask for interactables (must be set so that only "Interactable" layer is included)
    private LayerMask _interactableLayerMask;
    
    // Buffer réutilisé pour le SphereCast non alloc
    private readonly RaycastHit[] _hitBuffer = new RaycastHit[256];
    
    // Stocker l'interactible dans la range pour l'activer lorsque l'input est pressé.
    private IInteractable currentInteractable;

    public InteractComponent(PlayerCharacter character) : base(character)
    {
        if (character == null) return;
    }

    public override void Initialize()
    {
        character.FindInputAction("Interact", out _interactAction);

        // Cache the layer mask for interactables
        _interactableLayerMask = LayerMask.GetMask("Interactable");

        if (_interactAction == null)
        {
            Logs.LogError("[InteractComponent] Interact InputAction not found.");
        }
    }
    
    public void HandleInteractUpdate()
    {
        if (!character || _interactAction == null) return;
        
        if (character.IsStunned) return;

        var (interactable, hitPosition) = FindBestInteractable();
        currentInteractable = interactable;
        
        if (currentInteractable == null)
        {
            character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.NONE);
            return;
        }

        var targetTransform = currentInteractable.GetTransform();
        //Logs.Log($"[InteractComponent] Interacting with {targetTransform.name} at {hitPosition}");
        
        // Update CrosshairUI
        switch (currentInteractable)
        {
            case SteeringWheel:
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.DRIVE);
                break;
            case Ladder : 
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.ELSE);
                break;
            case Hole :
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.REPAIR);
                break;
            case Bank :
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.ELSE);
                break;
            case EnemyHealth :
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.DESTROY);
                break;
            case CrankItUp :
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.ELSE);
                break;
            
            //TODO : Rope crank
            
            default:
                character._uiSwapper.SwapCrosshair(UISwapper.InteractionUI.NONE);
                break;
        }
        

        if (!_interactAction.triggered) return;
        // Nouvelle API d'interaction générique
        currentInteractable.Interact(IInteractable.InteractAction.Primary, character.transform);
    }

    private (IInteractable interactable, Vector3 hitPosition) FindBestInteractable()
    {
        if (character.HeadTransform == null)
        {
            Logs.LogWarning("[InteractComponent] HeadTransform is not assigned on PlayerCharacter.");
            return (null, Vector3.zero);
        }

        var origin = character.HeadTransform.position;
        var direction = character.HeadTransform.forward;
        
        float radius = character.Playerstats.InteractRadius;
        float distance = character.Playerstats.InteractDistance;

        int hitCount = Physics.SphereCastNonAlloc(origin, radius, direction, _hitBuffer, distance, _interactableLayerMask);
        if (hitCount == 0) return (null, Vector3.zero);

        var interactables = new List<IInteractable>();
        var hitPositions = new List<Vector3>();

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hitBuffer[i];
            if (hit.collider == null) continue;

            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != null)
                {
                    interactables.Add(interactable);
                    hitPositions.Add(hit.point);
                }
            }
        }

        if (interactables.Count == 0) return (null, Vector3.zero);

        // Choisir l'interactable le plus proche du joueur
        IInteractable closest = null;
        Vector3 closestHitPos = Vector3.zero;

        for (int i = 0; i < interactables.Count; i++)
        {
            var interactable = interactables[i];

            // Ignore soi-même (transform du collider == transform du joueur)
            if (_hitBuffer[i].transform == character.transform)
                continue;

            var hitPos = hitPositions[i];

            if (closest == null ||
                Vector3.Distance(origin, hitPos) < Vector3.Distance(origin, closestHitPos))
            {
                closest = interactable;
                closestHitPos = hitPos;
            }
        }

        return (closest, closestHitPos);
    }

    public override void Dispose()
    {
        _interactAction = null;
    }

    public override void OnDrawGizmos()
    {
        // Draw the interaction sphere in front of the player for debugging
        if (character == null || character.HeadTransform == null) return;
        
        Gizmos.color = Color.yellow;
        Vector3 origin = character.HeadTransform.position;
        Vector3 direction = character.HeadTransform.forward;
        float radius = character.Playerstats.InteractRadius;
        float distance = character.Playerstats.InteractDistance;
        
        Gizmos.DrawWireSphere(origin + direction * distance, radius);
    }
}

