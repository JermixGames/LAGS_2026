using UnityEngine;
using System.Collections.Generic;


// Este script se le pone a los cubos invisibles (Triggers) que actúan como "Parada de Bus".
public class BusStop : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoRecoleccion = 5f; // Según tus instrucciones, 5 segundos para recoger
    public bool paradaCompletada = false;

    [Header("Visuales y Audio (Pavo y Pasajeros)")]
    public GameObject pavoPrefab;
    public GameObject capsulaPasajeroPrefab;
    public Transform puntoSpawnPavo;
    public Transform[] puntosSpawnCapsulas; // Varios puntos donde aparecerán
    public AudioClip audioLlamadoRuta;

    private float temporizador = 0f;
    private bool jugadorEnParada = false;
    private bool procesoIniciado = false;

    private GameObject pavoInstanciado;
    private List<GameObject> capsulasInstanciadas = new List<GameObject>();
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
            // REGLA: Player MUST stop at 0 speed. (Margen de 1km/h)
            if (busPlayer.velocidadActualKmh < 1f)
            {
                // PASO 1 y 2: El jugador frenó, iniciamos el proceso visual
                if (!procesoIniciado)
                {
                    IniciarProcesoVisual();
                }

                // PASO 4: Wait pickup time
                temporizador += Time.deltaTime;

                if (temporizador >= tiempoRecoleccion)
                {
                    paradaCompletada = true;
                    Debug.Log("¡Pasajeros a bordo! Arranca.");

                    StopManager.Instance.ParadaExitosa(this);
                    CambiarColor(Color.green);

                    // PASO 5: Destruir visuales
                    FinalizarProcesoVisual();
                }
            }
            else
            {
                // Si el bus se sigue moviendo o acelera antes de tiempo
                if (procesoIniciado) FinalizarProcesoVisual(); // El Pavo se asusta y se va
                temporizador = 0f;
            }
        }
    }

    private void IniciarProcesoVisual()
    {
        procesoIniciado = true;

        // Spawn Pavo
        if (pavoPrefab != null && puntoSpawnPavo != null)
        {
            pavoInstanciado = Instantiate(pavoPrefab, puntoSpawnPavo.position, puntoSpawnPavo.rotation);
            pavoInstanciado.tag = "Pavo"; // Asignar tag "Pavo" según regla
        }

        // Spawn Capsulas
        if (capsulaPasajeroPrefab != null)
        {
            foreach (Transform t in puntosSpawnCapsulas)
            {
                GameObject capsula = Instantiate(capsulaPasajeroPrefab, t.position, t.rotation);
                capsulasInstanciadas.Add(capsula);
            }
        }

        // Play audio
        if (audioLlamadoRuta != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirAudioEspecial(audioLlamadoRuta);
        }
    }

    private void FinalizarProcesoVisual()
    {
        procesoIniciado = false;

        // Destroy Pavo
        if (pavoInstanciado != null) Destroy(pavoInstanciado);

        // Destroy Passenger capsules
        foreach (var c in capsulasInstanciadas)
        {
            if (c != null) Destroy(c);
        }
        capsulasInstanciadas.Clear();
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
        }
    }
}