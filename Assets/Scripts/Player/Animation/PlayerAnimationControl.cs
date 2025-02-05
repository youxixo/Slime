using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    public PlayerController controller;

    public void ToFire()
    {
        controller.ChangeToFire();
    }

    public void ToGrass()
    {
        controller.ChangeToGrass();
    }

    public void ToWater()
    {
        controller.ChangeToWater();
    }
}
