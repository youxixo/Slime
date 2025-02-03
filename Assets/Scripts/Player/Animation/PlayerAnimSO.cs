using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimSO", menuName = "PlayerAnimSO", order = 0)]
public class PlayerAnimSO : ScriptableObject
{
    public AnimationClip transformToFire;
    public AnimationClip transformToWater;
    public AnimationClip transformToGrass;
    public AnimationClip move;
}
