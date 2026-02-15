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
    public bool onlySunGravity = false;
    public float softeningFactor = 1f;

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
            if (body.isStatic) continue;
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

        foreach (CelestialBody otherBody in bodies)
        {
            if (body == otherBody) continue;
            if (onlySunGravity && !otherBody.isSun) continue;
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