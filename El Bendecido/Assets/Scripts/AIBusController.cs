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

                // Si está muy cerca del waypoint, simula que va a recoger pasajeros
                if (Vector3.Distance(transform.position, destino.position) < 4f)
                {
                    estadoActual = AIState.Stopping;
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
