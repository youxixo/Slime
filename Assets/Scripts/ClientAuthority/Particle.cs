using UnityEngine;

public class Particle : MonoBehaviour
{
    public Vector3 velocity;
    public float mass = 1f;

    void Update()
    {
        // Update position based on velocity
        transform.position += velocity * Time.deltaTime;
    }

    public void ApplyForce(Vector3 force)
    {
        // F = ma -> a = F/m
        velocity += force / mass * Time.deltaTime;
    }
}

public class Spring : MonoBehaviour
{
    private Particle p1, p2;
    private float restLength;
    private float stiffness;
    private float damping;

    public void Initialize(Particle particle1, Particle particle2, float springStiffness, float dampingFactor)
    {
        p1 = particle1;
        p2 = particle2;
        restLength = Vector3.Distance(p1.transform.position, p2.transform.position);
        stiffness = springStiffness;
        damping = dampingFactor;
    }

    public void UpdateSpring()
    {
        Vector3 springVector = p2.transform.position - p1.transform.position;
        float currentLength = springVector.magnitude;
        Vector3 force = stiffness * (currentLength - restLength) * springVector.normalized;

        p1.ApplyForce(force);
        p2.ApplyForce(-force);

        // Apply damping
        p1.velocity *= damping;
        p2.velocity *= damping;
    }
}