using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class PoliceAI : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agent;
    public enum PoliceState { Idle, Chasing, Attacking, LostTarget }
    public PoliceState estadoActual = PoliceState.Idle;
    [Header("Escape System")]
    public float distanciaEscape = 40f;
    public float tiempoParaEscapar = 5f;
    private float temporizadorEscape = 0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 25f; // ~90 km/h
        estadoActual = PoliceState.Chasing; // Inicia persiguiendo de una vez porque el SpeedTrap lo llamó
    }
    void Update()
    {
        if (objetivo == null) return;
        float distancia = Vector3.Distance(transform.position, objetivo.position);
        switch (estadoActual)
        {
            case PoliceState.Idle:
                agent.isStopped = true;
                break;
            case PoliceState.Chasing:
                agent.isStopped = false;
                agent.SetDestination(objetivo.position);

                // Behavior: far -> normal, close -> faster
                if (distancia < 15f) agent.speed = 38f; // ~136 km/h (Mete chola)
                else agent.speed = 25f; // Velocidad Normal
                // Escape logic
                if (distancia > distanciaEscape)
                {
                    temporizadorEscape += Time.deltaTime;
                    if (temporizadorEscape >= tiempoParaEscapar)
                    {
                        Debug.Log("💨 ¡Escapaste de la policía! El Lince abandonó la persecución.");
                        estadoActual = PoliceState.LostTarget;

                        // Apagar sirena de persecución
                        if (AudioManager.Instance != null) AudioManager.Instance.DetenerSirenaPolicia();
                    }
                }
                else
                {
                    temporizadorEscape = 0f; // Reinicia el timer si se vuelve a acercar
                }
                break;
            case PoliceState.Attacking:
                // Se maneja a nivel de colisión física (OnCollisionEnter en el bus)
                break;
            case PoliceState.LostTarget:
                agent.isStopped = true;
                // Despawn despues de escapar
                Destroy(gameObject, 2f);
                break;
        }
    }
}
