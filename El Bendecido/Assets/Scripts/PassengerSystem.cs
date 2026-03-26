using UnityEngine;

// Gestor del "Secretario" o "Pavo" del Diablo Rojo
public class PassengerSystem : MonoBehaviour
{
    public static PassengerSystem Instance;

    [Header("Estadísticas del Viaje")]
    public int capacidadMaximaBus = 45; // Para verificar el Bus Lleno
    public int pasajerosActuales = 0;
    public int dineroActual = 0;
    public int pasajerosEnojados = 0;

    [Header("Metas Aleatorias para Ganar")]
    public int metaPasajeros;
    public int metaDinero;

    [Header("Sistema de Nitro (Chacalito)")]
    public int cargasDeNitro = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Generar metas aleatorias desde el inicio al arrancar el nivel
        metaPasajeros = Random.Range(30, 100);
        metaDinero = metaPasajeros * 25; // Asumimos 25 centavos el pasaje

        Debug.Log($"📋 METAS DE HOY: Llevar {metaPasajeros} pasajeros a la terminal y hacer ${metaDinero} centavos.");
    }

    // Se llama desde StopManager.cs cuando frenas los 5 segundos enteros
    public void SubirPasajerosExitazo()
    {
        // Suben entre 5 a 8 pasajeros normales
        int nuevosPasajeros = Random.Range(5, 9);
        pasajerosActuales += nuevosPasajeros;
        dineroActual += nuevosPasajeros * 25;

        Debug.Log($"✅ ¡Se subieron {nuevosPasajeros} pasajeros! Llevas {pasajerosActuales} cabezas y {dineroActual} centavos.");

        // CONDICIÓN: Si sobrepasamos la capacidad, gritamos
        if (pasajerosActuales >= capacidadMaximaBus)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.ReproducirBusLleno();

            Debug.Log("¡BUS LLENO! 'Móntense, los puestos de la izquierda son de a tres'");
        }

        CheckForPowerUpPassenger();
    }

    // Se llama desde StopManager.cs cuando te pasas de una parada sin frenar bien
    public void PasajerosEnojadosOvershoot()
    {
        // Solo 1 a 3 logran saltar al vuelo
        int subieronCorriendo = Random.Range(1, 4);
        pasajerosActuales += subieronCorriendo;

        // Dan menos plata (Ej. 10 centavos) porque tuviste mal servicio
        dineroActual += subieronCorriendo * 10;
        pasajerosEnojados++;

        Debug.Log($"❌ ¡Te pasaste la parada! Solo saltaron {subieronCorriendo} personas y te pagaron incompleto.");
    }

    private void CheckForPowerUpPassenger()
    {
        // 35% de probabilidad de que suba alguien especial (Power-Up)
        if (Random.Range(0, 100) < 35)
        {
            int tipoEspecial = Random.Range(0, 3); // Escoge entre 0, 1 y 2

            if (tipoEspecial == 0)
            {
                Debug.Log("🍻 SE SUBIÓ UN BORRACHO: Controles Invertidos temporalmente pero te regaló 50 centavos más.");
                dineroActual += 50;
                PowerUpSystem.Instance.ActivarBorracho();
            }
            else if (tipoEspecial == 1)
            {
                Debug.Log("👵 SE SUBIÓ UNA VIEJITA: '¡Mi hijito dale despacio cruzando la calle!'. Velocidad máxima reducida.");
                PowerUpSystem.Instance.ActivarViejita();
            }
            else if (tipoEspecial == 2)
            {
                int nitrosDados = Random.Range(1, 4); // Da 1 a 3 cargas de Nitro
                cargasDeNitro += nitrosDados;
                Debug.Log($"😎 SE SUBIÓ UN CHACALITO: Te regaló {nitrosDados} latas de Nitro. (Total Nitros: {cargasDeNitro})");
            }
        }
    }
}
