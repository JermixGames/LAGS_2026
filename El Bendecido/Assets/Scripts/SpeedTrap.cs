using UnityEngine;

// AJUSTE 1: POLICE SYSTEM (TRIGGER-BASED SPEED CHECK)
public class SpeedTrap : MonoBehaviour
{
    [Header("Configuración del Radar")]
    public float limiteVelocidad = 80f;

    [Header("Referencias Policiales")]
    public GameObject patrullaPrefab;
    public Transform puntoDeSpawn; // Dónde se esconde el policía

    private bool activado = false; // Para evitar que se spawneen 100 policías

    void OnTriggerEnter(Collider other)
    {
        // Solo verificamos si no hemos activado la trampa ya
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            PlayerBusController bus = other.GetComponentInParent<PlayerBusController>();

            // Check current speed
            if (bus != null && bus.velocidadActualKmh > limiteVelocidad)
            {
                activado = true;
                Debug.Log($"🚨 ¡EXCESO DE VELOCIDAD! Ibas a {bus.velocidadActualKmh} km/h en zona de {limiteVelocidad}.");

                // Spawn police car
                if (patrullaPrefab != null && puntoDeSpawn != null)
                {
                    GameObject patrulla = Instantiate(patrullaPrefab, puntoDeSpawn.position, puntoDeSpawn.rotation);
                    patrulla.tag = "PoliceCar";

                    // Asegurarnos de que tenga el script de persecución NavMesh
                    var lince = patrulla.GetComponent<PoliceAI>();
                    if (lince == null) lince = patrulla.AddComponent<PoliceAI>();

                    // Asignar objetivo
                    lince.objetivo = bus.transform;
                }
            }
        }
    }
}