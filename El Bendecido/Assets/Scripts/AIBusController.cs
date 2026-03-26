using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBusController : MonoBehaviour
{
    public List<Transform> waypoints;
    private NavMeshAgent agent;
    private int puntoActual = 0;

    // SIMPLE State Machine
    public enum AIState { Driving, Stopping, Leaving }
    public AIState estadoActual = AIState.Driving;

    private float tiempoEspera = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 19f; // Aprox 70 kmh permitidos en NavMesh
    }

    void Update()
    {
        if (waypoints.Count == 0 || GameManager.Instance.estadoActual != GameManager.GameState.Playing)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        switch (estadoActual)
        {
            case AIState.Driving:
                agent.isStopped = false;
                Transform destino = waypoints[puntoActual];
                agent.SetDestination(destino.position);

                // Si está muy cerca del waypoint
                if (Vector3.Distance(transform.position, destino.position) < 4f)
                {
                    // VERIFICACIÓN FLUIDA: Buscamos si el punto tiene instrucciones especiales de detenerse
                    WaypointInformativo info = destino.GetComponent<WaypointInformativo>();

                    if (info != null && info.esParadaObligatoria)
                    {
                        // Si el punto TIENE el script y está marcado como parada, frena:
                        estadoActual = AIState.Stopping;
                        Debug.Log("Bus Rival: Llegué a una parada. Simulo recoger gente por 3 seg...");
                    }
                    else
                    {
                        // Si NO está marcado (es solo una curva en la calle), pasa de largo a máxima velocidad:
                        estadoActual = AIState.Leaving;
                    }

                    tiempoEspera = 0f;
                }
                break;

            case AIState.Stopping:
                agent.isStopped = true;
                tiempoEspera += Time.deltaTime;

                // Simula que recoge pasajeros por 3 segundos
                if (tiempoEspera >= 3f)
                {
                    estadoActual = AIState.Leaving;
                }
                break;

            case AIState.Leaving:
                puntoActual++;

                if (puntoActual >= waypoints.Count)
                {
                    Debug.Log("🏁 EL BUS RIVAL LLEGÓ A LA TERMINAL. GAME OVER.");
                    GameManager.Instance.ActivarGameOver();
                    enabled = false;
                }
                else
                {
                    // Volver a conducir hacia el siguiente
                    estadoActual = AIState.Driving;
                }
                break;
        }
    }
}
