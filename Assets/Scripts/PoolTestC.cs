using UnityEngine;
using System;
using System.Collections;

public class PoolTestC : MonoBehaviour
{
    public PoolTestA belongedPool;

    void OnEnable()
    {
        // Start the coroutine to destroy the object
        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy this GameObject
        PoolTestB.test.ReturnToPool(this.gameObject);
    }
}
