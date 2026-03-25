using UnityEngine;
using System.Collections.Generic;

// Este es el juez principal de la competencia. Conoce todas las paradas que hay de punto A a punto B.
public class StopManager : MonoBehaviour
{
    public static StopManager Instance;

    [Header("Ruta del Bus")]
    [Tooltip("Arrastra aquí todas las paradas en el orden en que el jugador debe recoger pasajeros.")]
    public List<BusStop> listaParadas;

    [Header("Progreso Actual")]
    public int paradaActualIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ParadaExitosa(BusStop parada)
    {
        if (listaParadas.IndexOf(parada) == paradaActualIndex)
        {
            Debug.Log($"[StopManager] Parada {paradaActualIndex + 1} completada con ÉXITO.");

            // Llamamos al pavo para que cobre
            PassengerSystem.Instance.SubirPasajerosExitazo();

            AvanzarSiguiente();
        }
    }

    public void JugadorSePasoDeParada(BusStop parada)
    {
        if (listaParadas.IndexOf(parada) == paradaActualIndex)
        {
            Debug.LogWarning($"[StopManager] ¡Te saltaste la parada {paradaActualIndex + 1}! Pasajeros enojados. Menos Plata.");

            // Obligamos a fallar la parada en su propio script para que no pueda volver atrás.
            parada.ForzarFalla();

            // Pasajeros enojados y penalización
            PassengerSystem.Instance.PasajerosEnojadosOvershoot();

            AvanzarSiguiente();
        }
    }

    private void AvanzarSiguiente()
    {
        paradaActualIndex++;

        // Verifica si hemos completado todo el circuito
        if (paradaActualIndex >= listaParadas.Count)
        {
            Debug.Log("🏁 ¡HAS LLEGADO A LA TERMINAL FINAL! 🏁");
            GameManager.Instance.ActivarVictoria();
        }
        else
        {
            Debug.Log($"[StopManager] Próximo destino: Parada {paradaActualIndex + 1}");
        }
    }
}
