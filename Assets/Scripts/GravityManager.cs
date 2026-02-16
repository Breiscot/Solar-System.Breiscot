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

    private List<CelestialBody> bodies = new List<CelestialBody>();
    private bool initialized = false;

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

        float timeStep = Time.fixedDeltaTime * timeScale;

        // Muove il sole con la velocità galattica
        foreach (CelestialBody body in bodies)
        {
            if (body.isSun)
            {
                body.transform.position += body.currentVelocity * timeStep;
            }
        }

        // Aggiorna posizioni usando la velocità e accelerazione attuale
        foreach (CelestialBody body in bodies)
        {
            if (body.isSun) continue;
            body.transform.position += body.currentVelocity * timeStep + 0.5f * body.currentAcceleration * timeStep * timeStep; 
        }

        // Calcola le nuove accelerazioni    
        foreach (CelestialBody body in bodies)
        {
            if (body.isStatic) continue;
            Vector3 newAcceleration = CalculateAcceleration(body);
            body.currentVelocity += 0.5f * (body.currentAcceleration + newAcceleration) * timeStep;
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
                    float distance = Mathf.Sqrt(distanceSqr);

                    if (distance > 0.1f)
                    {
                        float accel = gravitationalConstant * parent.mass / distanceSqr;
                        totalAcceleration = direction.normalized * accel;
                    }
                }

                totalAcceleration += CalculateAcceleration(
                    body.orbitAround.GetComponent<CelestialBody>()
                );
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

            float distance = Mathf.Sqrt(distanceSqr);
            float accelerationMagnitude = gravitationalConstant * otherBody.mass / distanceSqr;

            totalAcceleration += direction.normalized * accelerationMagnitude;
        }
        
        return totalAcceleration;
    }
}