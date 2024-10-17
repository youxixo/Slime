using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class testrotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float zRotation = transform.eulerAngles.z + 0.01f; // 1 degree per frame, adjust as needed
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
