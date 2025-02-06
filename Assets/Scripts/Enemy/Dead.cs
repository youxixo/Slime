using UnityEngine;

public class Dead : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    public GameObject reward;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnAnimationEnd()
    {
        if(reward != null)
            Instantiate(reward, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
