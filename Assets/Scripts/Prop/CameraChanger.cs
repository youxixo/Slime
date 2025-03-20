using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    private Collider2D ogCollider;
    [SerializeField]private Transform lockPoint;
    [SerializeField] private CinemachineConfiner2D cine_camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    [SerializeField] AudioClip bossMusic;
    [SerializeField] AudioSource source;


    public static CameraChanger Instance;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    private void ActiveChild()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void DeactiveChild()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Camera.main.transform.position = new Vector3(lockPoint.position.x, lockPoint.position.y, Camera.main.transform.position.z);
            this.CinemachineCamera.Follow = lockPoint;
            source.clip = bossMusic;
            source.Play();
            ActiveChild();
        }
    }
}
