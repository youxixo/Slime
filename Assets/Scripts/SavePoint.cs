using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Vector2 savePos;

    private void Start()
    {
        savePos = transform.position;
    }
}
