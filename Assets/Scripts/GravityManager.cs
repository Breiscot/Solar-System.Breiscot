using UnityEngine;
using System.Collections.Generic;

public class GravityManager : MonoBehaviour
{
    [Header("Costanti Fisiche")]
    public float gravitationalConstant = 0.1f;

    [Header("Simulazione")]
    public float timeScale = 1f;

    [Header("Stabilità")]
    [Tooltip("Se attivo, i pianeti sentono solo la gravità del sole")]
    public float softeningFactor = 0.5f;
    public int maxSubSteps = 200;

    private List<CelestialBody> bodies = new List<CelestialBody>();
    private bool initialized = false;

    private float baseTimeStep = 0.005f;

    void Start()
    {
        Invoke("Initialize", 0.5f);
    }

    void Initialize()
    {
        // Trova tutti i corpi celesti
        bodies.AddRange(FindObjectsOfType<CelestialBody>());
        Debug.Log($"Trovati " + bodies.Count + "corpi celesti");

        // Calcola la velocità dei pianeti
        foreach (CelestialBody body in bodies)
        {
            if (body.autoCalculateVelocity && !body.isMoon && !body.isSun)
            {
                body.CalculateOrbitalVelocity(gravitationalConstant);
            }
        }

        // Calcola la velocità della luna
        foreach (CelestialBody body in bodies)
        {
            if (body.autoCalculateVelocity && body.isMoon)
            {
                body.CalculateOrbitalVelocity(gravitationalConstant);
            }
        }

        // Calcola le accelerazioni iniziali
        foreach (CelestialBody body in bodies)
        {
            if (!body.isSun)
            {
                body.currentAcceleration = CalculateAcceleration(body);
            }
        }

        initialized = true;
    }

    void FixedUpdate()
    {
        if (!initialized) return;
        if (timeScale <= 0) return;

        float totalTime = Time.fixedDeltaTime * timeScale;
        int subSteps = Mathf.CeilToInt(totalTime / baseTimeStep);
        subSteps = Mathf.Clamp(subSteps, 1, maxSubSteps);
        float dt = totalTime / subSteps;

        for (int step = 0; step < subSteps; step++)
        {
            DoPhysicsStep(dt);
        }
    }

    void DoPhysicsStep(float dt)
    {
        // Muove il sole con la velocità galattica
        foreach (CelestialBody body in bodies)
        {
            if (body.isSun)
            {
                body.transform.position += body.currentVelocity * dt;
            }
        }

        // Aggiorna posizioni pianeti e luna
        foreach (CelestialBody body in bodies)
        {
            if (body.isSun) continue;
            body.transform.position += body.currentVelocity * dt + 0.5f * body.currentAcceleration * dt * dt; 
        }

        // Calcola le nuove accelerazioni    
        foreach (CelestialBody body in bodies)
        {
            if (body.isSun) continue;
            Vector3 newAcceleration = CalculateAcceleration(body);
            body.currentVelocity += 0.5f * (body.currentAcceleration + newAcceleration) * dt;
            body.currentAcceleration = newAcceleration;
        }  
    }

    Vector3 CalculateAcceleration(CelestialBody body)
    {
        Vector3 totalAcceleration = Vector3.zero;

        if (body.isMoon)
        {
            if (body.orbitAround != null)
            {
                CelestialBody parent = body.orbitAround.GetComponent<CelestialBody>();
                if (parent != null)
                {
                    Vector3 direction = parent.transform.position - body.transform.position;
                    float distanceSqr = direction.sqrMagnitude;

                    if (distanceSqr > 0.01f)
                    {
                        float accel = gravitationalConstant * parent.mass / distanceSqr;
                        totalAcceleration = direction.normalized * accel;
                    }
                }

                // Accelerazione del genitore
                CelestialBody parentBody = body.orbitAround.GetComponent<CelestialBody>();
                if (parentBody != null && !parentBody.isMoon)
                {
                    foreach (CelestialBody otherBody in bodies)
                    {
                        if (otherBody.isSun)
                        {
                            Vector3 dir = otherBody.transform.position - parentBody.transform.position;
                            float dSqr = dir.sqrMagnitude;
                            dSqr += softeningFactor * softeningFactor;
                            float acc = gravitationalConstant * otherBody.mass / dSqr;
                            totalAcceleration += dir.normalized * acc;
                        }
                    }
                }
            }

            return totalAcceleration;
        }

        foreach (CelestialBody otherBody in bodies)
        {
            if (body == otherBody) continue;
            if (!otherBody.isSun) continue;

            Vector3 direction = otherBody.transform.position - body.transform.position;
            float distanceSqr = direction.sqrMagnitude;            
            distanceSqr += softeningFactor * softeningFactor;

            float accelerationMagnitude = gravitationalConstant * otherBody.mass / distanceSqr;
            totalAcceleration += direction.normalized * accelerationMagnitude;
        }
        
        return totalAcceleration;
    }
}