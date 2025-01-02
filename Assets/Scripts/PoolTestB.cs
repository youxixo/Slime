using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolTestB : MonoBehaviour
{
    public GameObject testPrefab;
    public static PoolTestA test;

    public void testConst()
    {
        Debug.Log("1");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Action<Object> enterHandle = (obj) => Debug.Log($"{obj.name} entered the pool.");
        //Action<Object> leaveHandle = (obj) => Debug.Log($"{obj.name} left the pool.");
        test = new PoolTestA(testPrefab, this.transform, null, 0, 20, null, null);
        test.InitPool();
        StartCoroutine(ExecuteEveryHalfSecond());
    }

    private IEnumerator ExecuteEveryHalfSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f); 
            Object o = test.Get();
            o.GetComponent<PoolTestC>().belongedPool = test; 
        }
    }
}
