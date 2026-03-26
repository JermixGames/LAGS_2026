using UnityEngine;

// CAPA DE RIESGO ADICIONAL (Overcapacity System)
// Este script es 100% independiente para mantener la modularidad. 
// No rompe ni interfiere con las colisiones, el enojo por pasarse la parada, ni el dinero.
public class OvercapacitySystem : MonoBehaviour
{
    [Header("Configuración de Riesgo Extra")]
    [Tooltip("Pérdida de giro por el peso excesivo del bus lleno")]
    public float penalizacionVelocidadGiro = 30f;

    [Tooltip("Tiempo en que un pasajero se enoja y se tira del bus por exceso de calor/apretón")]
    public float intervaloFugaPasajeros = 6f;

    private PlayerBusController busController;
    private bool sobrecargado = false;
    private float giroOriginal;
    private float temporizadorRiesgo = 0f;

    void Start()
    {
        // Buscamos al Diablo Rojo automáticamente
        busController = Object.FindAnyObjectByType<PlayerBusController>();

        if (busController != null)
        {
            giroOriginal = busController.velocidadGiro;
        }
    }

    void Update()
    {
        // Validamos que existan los sistemas base antes de hacer nada
        if (PassengerSystem.Instance == null || busController == null) return;

        // CONDICIÓN SEPARADA: ¿Superamos la capacidad máxima establecida? (ej. 50)
        if (PassengerSystem.Instance.pasajerosActuales > PassengerSystem.Instance.capacidadMaximaBus)
        {
            if (!sobrecargado)
            {
                sobrecargado = true;

                // Efecto de riesgo 1: El bus se vuelve torpe por el peso
                busController.velocidadGiro -= penalizacionVelocidadGiro;
                Debug.Log("⚠️ ¡SISTEMA DE SOBRECAPACIDAD ACTIVADO! El bus está súper pesado, cuidado al girar.");
            }

            // Efecto de riesgo 2: Se empiezan a bajar corriendo cada X segundos
            temporizadorRiesgo += Time.deltaTime;

            if (temporizadorRiesgo >= intervaloFugaPasajeros)
            {
                temporizadorRiesgo = 0f; // Reiniciamos el reloj

                // Solo se bajan si seguimos estando por encima del límite
                if (PassengerSystem.Instance.pasajerosActuales > PassengerSystem.Instance.capacidadMaximaBus)
                {
                    Debug.Log("🤬 Un pasajero no aguantó el apretujón y se tiró por la puerta trasera.");

                    // Aumentamos los enojados y reducimos los actuales sin tocar el dinero ganado
                    PassengerSystem.Instance.pasajerosActuales--;
                    PassengerSystem.Instance.pasajerosEnojados++;
                }
            }
        }
        else // Si los pasajeros bajan a 50 o menos
        {
            if (sobrecargado)
            {
                sobrecargado = false;

                // Restauramos la física del bus a la normalidad
                busController.velocidadGiro = giroOriginal;
                temporizadorRiesgo = 0f;

                Debug.Log("✅ Niveles de pasajeros estabilizados. Manejo del bus restaurado.");
            }
        }
    }
}
