using UnityEngine;
using System.Collections.Generic;

public class GravityManager : MonoBehaviour
{
    [Header("Costanti Fisiche")]
    public float gravitationalConstant = 0.1f;

    [Header("Simulazione")]
    public float timeScale = 1f;

    // Lista di tutti i corpi celesti
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

        foreach (CelestialBody body in bodies)
        {
            body.currentAcceleration = CalculateAcceleration(body);
        }

        initialized = true;
    }

    void FixedUpdate()
    {
        if (!initialized) return;
        
        float timeStep = Time.fixedDeltaTime * timeScale;

        // Aggiorna posizioni usando la velocità e accelerazione attuale
        foreach (CelestialBody body in bodies)
        {
            body.transform.position += body.currentVelocity * timeStep + 0.5f * body.currentAcceleration * timeStep * timeStep;
        }

        // Calcola le nuove accelerazioni    
        foreach (CelestialBody body in bodies)
        {
            Vector3 newAcceleration = CalculateAcceleration(body);

            // Aggiorna velocità usando media delle accelerazioni
            body.currentVelocity += 0.5f * (body.currentAcceleration + newAcceleration) * timeStep;

            body.currentAcceleration = newAcceleration;
        }  

        foreach (CelestialBody body in bodies)
        {
            body.UpdateTrail();
        }
    }

    Vector3 CalculateAcceleration(CelestialBody body)
    {
        Vector3 totalAcceleration = Vector3.zero;

        foreach (CelestialBody otherBody in bodies)
        {
            if (body == otherBody) continue;
            Vector3 direction = otherBody.transform.position - body.transform.position;
            float distance = direction.magnitude;
            
            if (distance < 0.5f) continue;

            float accelerationMagnitude = gravitationalConstant * otherBody.mass / (distance * distance);
            totalAcceleration += direction.normalized * accelerationMagnitude;
        }
        
        return totalAcceleration;
    }
}