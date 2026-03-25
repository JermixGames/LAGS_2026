using UnityEngine;

// Este script se le pone a los cubos invisibles (Triggers) que actúan como "Parada de Bus".
public class BusStop : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoRecoleccion = 5f; // Según tus instrucciones, 5 segundos para recoger
    public bool paradaCompletada = false;

    private float temporizador = 0f;
    private bool jugadorEnParada = false;
    private PlayerBusController busPlayer;

    void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entró en la luz tiene el Tag "Player" (nuestro bus)
        if (other.CompareTag("Player") && !paradaCompletada)
        {
            jugadorEnParada = true;
            busPlayer = other.GetComponentInParent<PlayerBusController>();

            Debug.Log("Has llegado a la parada. ¡Detente a 0 km/h y espera 5 segundos!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !paradaCompletada)
        {
            jugadorEnParada = false;

            // REGLAS: Si el jugador se pasa de la parada sin completarla al 100%:
            // "Cannot go back, Passengers give less money, Become angry".
            StopManager.Instance.JugadorSePasoDeParada(this);
        }
    }

    void Update()
    {
        if (jugadorEnParada && !paradaCompletada && busPlayer != null)
        {
            // REGLA: Player MUST stop at 0 speed.
            // Usamos < 1 km/h para dar un pequeñísimo margen, en Unity 0 físico es a veces molesto.
            if (busPlayer.velocidadActualKmh < 1f)
            {
                temporizador += Time.deltaTime;

                if (temporizador >= tiempoRecoleccion)
                {
                    paradaCompletada = true;
                    Debug.Log("¡Pasajeros a bordo! Arranca.");

                    // Avisamos al Manager principal que esta parada fue todo un éxito
                    StopManager.Instance.ParadaExitosa(this);

                    // Apagamos el color de esta parada o cambiamos su luz
                    CambiarColor(Color.green);
                }
            }
            else
            {
                // Si el bus se sigue moviendo dentro de la parada, no cuenta el tiempo
                temporizador = 0f;
            }
        }
    }

    public void ForzarFalla()
    {
        paradaCompletada = true;
        CambiarColor(Color.red); // Se pone roja de enojo porque te la saltaste
    }

    private void CambiarColor(Color color)
    {
        var render = GetComponent<Renderer>();
        if (render != null)
        {
            render.material.color = color;
        }
    }
}
