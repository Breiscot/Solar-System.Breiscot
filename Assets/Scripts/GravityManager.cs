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

    void Start()
    {
        // Trova tutti i corpi celesti
        bodies.AddRange(FindObjectsOfType<CelestialBody>());
        Debug.Log($"Trovati " + bodies.Count + "corpi celesti");
    }

    void FixedUpdate()
    {
        float timeStep = Time.fixedDeltaTime * timeScale;

        // Prima calcola tutte le accelerazioni
        foreach (CelestialBody body in bodies)
        {
            Vector3 totalAcceleration = Vector3.zero;
            
            foreach (CelestialBody otherBody in bodies)
            {
                // Non calcolare la gravità su se stesso
                if (body == otherBody) continue;

                // Calcola la forza gravitazionale
                Vector3 direction = otherBody.transform.position - body.transform.position;
                float distance = direction.magnitude;

                // Evita divisione per zero
                if (distance < 0.1f) continue;

                // Formula di Newton: F = G * (m1 * m2) / r²
                // Accelerazione: a = F / m1 = G * m2 / r²
                float accelerationMagnitude = gravitationalConstant * otherBody.mass / (distance * distance);

                // Direzione normalizzata * magnitudine
                totalAcceleration += direction.normalized * accelerationMagnitude;
            }

            // Applica l'accelerazione al corpo
            body.currentVelocity += totalAcceleration * timeStep;
        }

        foreach (CelestialBody body in bodies)
        {
            body.UpdatePosition(timeStep);
        }
    }
}