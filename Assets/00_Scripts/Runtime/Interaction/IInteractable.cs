using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Transform racine de l'objet interactable (utilisé pour les calculs de distance / debug).
    /// </summary>
    Transform GetTransform();

    /// <summary>
    /// Nouvelle API générique d'interaction, indépendante de PlayerController.
    /// </summary>
    /// <param name="action">Type d'action (clic principal, secondaire, etc.).</param>
    /// <param name="interactor">Transform de l'interactor (joueur, etc.).</param>
    void Interact(InteractAction action, Transform interactor);

    /// <summary>
    /// API legacy utilisée par l'ancien PlayerController.
    /// </summary>
    /// <param name="interactor">Ancien contrôleur joueur.</param>
    void Interact(PlayerCharacter interactor);

    public enum InteractAction
    {
        Primary = 0,
        Secondary = 1
    }
}
