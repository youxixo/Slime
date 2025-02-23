using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    public PlayerController controller;

    public void ToFire()
    {
        controller.ChangeToFire();
        EventHandler.CallSlimeTypeEnterEvent(SlimeType.Fire);
        controller.transforming = false;
    }

    public void ToGrass()
    {
        controller.ChangeToGrass();
        EventHandler.CallSlimeTypeEnterEvent(SlimeType.Grass);
        controller.transforming = false;
    }

    public void ToWater()
    {
        controller.ChangeToWater();
        EventHandler.CallSlimeTypeEnterEvent(SlimeType.Water);
        controller.transforming = false;
    }
}
