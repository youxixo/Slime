using UnityEngine;

public class SoftBodyScaler : MonoBehaviour
{
    public float scaleMultiplier = 1.5f; // Scale factor to apply
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GetComponent<SoftBodyScaler>().ScaleSoftBody();
        }
    }

    public void ScaleSoftBody()
    {
        // Scale the parent object
        transform.localScale = originalScale * scaleMultiplier;

        // Adjust spring joints and colliders
        foreach (Transform child in transform)
        {
            Debug.Log(child.name);
            // Adjust collider sizes
            var collider = child.GetComponent<Collider2D>(); // Or Collider for 3D
            if (collider != null)
            {
                collider.transform.localScale = originalScale * scaleMultiplier;
            }

            // Adjust spring joint properties
            var springJoints = child.GetComponents<SpringJoint2D>(); // Or SpringJoint for 3D
            foreach (var joint in springJoints)
            {
                if (joint != null)
                {
                    joint.distance *= scaleMultiplier;
                    joint.frequency *= Mathf.Pow(scaleMultiplier, -0.5f); // Adjust based on new mass
                    joint.dampingRatio *= scaleMultiplier;
                }
            }
            /*
            // Adjust rigidbody mass
            var rigidbody = child.GetComponent<Rigidbody2D>(); // Or Rigidbody for 3D
            if (rigidbody != null)
            {
                rigidbody.mass *= Mathf.Pow(scaleMultiplier, 2); // Use 3 for 3D objects
            }
            */
        }
    }
}
