using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Usamos el patrón "Singleton" para que cualquier otro script pueda comunicarse 
    // con el GameManager fácilmente usando: GameManager.Instance.Metodo();
    public static GameManager Instance;
    // Aquí definimos los posibles estados en los que puede estar tu juego.
    public enum GameState
    {
        Init,       // Cuando la escena apenas está cargando
        Playing,    // Cuando estás jugando y manejando el bus
        GameOver,   // Cuando pierdes (ej. chocas mucho oa los pasajeros se enojan)
        Win         // Cuando llegas a la última parada y cumples tus metas
    }

    [Header("Paneles de UI")]
    public GameObject panelVictoria;
    public GameObject panelDerrota;
    [Header("Estado Actual del Juego")]
    public GameState estadoActual;
    void Awake()
    {
        // Configuramos el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Si por algún motivo se duplica el GameManager, destruimos la copia
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Apenas empieza la "City Scene", iniciamos el juego
        IniciarJuego();
    }
    public void IniciarJuego()
    {
        estadoActual = GameState.Playing;

        // Nos aseguramos de que el tiempo corra de forma normal a velocidad 1
        // (Por si acaso quedó congelado tras salir del menú de pausa)
        Time.timeScale = 1f;

        Debug.Log("GameManager: ¡El juego ha comenzado. Arranca el Diablo Rojo!");
    }
    public void ActivarGameOver()
    {
        if (estadoActual == GameState.GameOver || estadoActual == GameState.Win) return;

        estadoActual = GameState.GameOver;
        Debug.Log("🛑 GameManager: ¡HAS PERDIDO! GAME OVER.");
        if (panelDerrota != null) panelDerrota.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ActivarVictoria()
    {
        if (estadoActual == GameState.Win || estadoActual == GameState.GameOver) return;

        // REGLAS: Se evaluarán 3 cosas:
        // - Llegar primero (si esta función es llamada, el jugador llegó primero y no la IA)
        // - Cumplir cuota de Pasajeros
        // - Cumplir cuota de Dinero
        bool victoriaPasajeros = PassengerSystem.Instance.pasajerosActuales >= PassengerSystem.Instance.metaPasajeros;
        bool victoriaDinero = PassengerSystem.Instance.dineroActual >= PassengerSystem.Instance.metaDinero;

        if (victoriaPasajeros && victoriaDinero)
        {
            estadoActual = GameState.Win;
            Debug.Log("🏆 GameManager: ¡VICTORIA ABSOLUTA! Llegaste primero y cumpliste doble cuota.");
            if (panelVictoria != null) panelVictoria.SetActive(true);
        }
        else
        {
            estadoActual = GameState.GameOver; // Empate o Perdió porque no cumplió cuotas
            Debug.Log($"⚠️ GameManager: Llegaste primero pero... Pasajeros: {victoriaPasajeros}, Dinero: {victoriaDinero}. ¡META INCUMPLIDA!");
            if (panelDerrota != null) panelDerrota.SetActive(true);
        }
        Time.timeScale = 0f;
    }
    public void VolverAlMenuPrincipal()
    {
        // Siempre, SIEMPRE descongelar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // El Menú Principal suele ser la Escena 0 en el "Build Settings"
        SceneManager.LoadScene(0);
    }
    public void ReiniciarNivelActual()
    {
        // Descongelar el tiempo para el nuevo juego
        Time.timeScale = 1f;
        // Recargar la escena actual independientemente de su ID
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

