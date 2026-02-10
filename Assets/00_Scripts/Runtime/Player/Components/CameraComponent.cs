using UnityEngine;

public class CameraComponent : PlayerComponent
{
    
    public CameraComponent(PlayerCharacter character) : base(character)
    {
    }

    public override void Initialize()
    {
        // Set the player layer to the corresponding layer depending on his PlayerIndex
        character.gameObject.layer = LayerMask.NameToLayer($"Player_{character.PlayerIndex}");
        
        // Then exclude this layer from this player main camera culling mask 
        character.MainCamera.cullingMask = ~(1 << LayerMask.NameToLayer($"Player_{character.PlayerIndex}"));
    }
}