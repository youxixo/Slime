using UnityEngine;

public class SoftBody : MonoBehaviour
{
    public int gridSize = 5;
    public float springStiffness = 0.5f;
    public float damping = 0.9f;
    public GameObject particlePrefab;

    private Particle[,] particles;

    void Start()
    {
        InitializeParticles();
        CreateSprings();
    }

    void Update()
    {
        SimulateSprings();
    }

    void InitializeParticles()
    {
        particles = new Particle[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject particleObj = Instantiate(particlePrefab, new Vector3(i, j, 0), Quaternion.identity);
                Particle particle = particleObj.AddComponent<Particle>();
                particles[i, j] = particle;
            }
        }
    }

    void CreateSprings()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (i < gridSize - 1)
                {
                    CreateSpring(particles[i, j], particles[i + 1, j]);
                }
                if (j < gridSize - 1)
                {
                    CreateSpring(particles[i, j], particles[i, j + 1]);
                }
            }
        }
    }

    void CreateSpring(Particle p1, Particle p2)
    {
        Spring spring = gameObject.AddComponent<Spring>();
        spring.Initialize(p1, p2, springStiffness, damping);
    }

    void SimulateSprings()
    {
        foreach (Spring spring in GetComponents<Spring>())
        {
            spring.UpdateSpring();
        }
    }
}