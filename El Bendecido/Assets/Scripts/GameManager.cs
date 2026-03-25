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
        // Si ya perdimos, ignoramos el resto para no repetir el código
        if (estadoActual == GameState.GameOver) return;

        estadoActual = GameState.GameOver;
        Debug.Log("GameManager: ¡Has perdido! Game Over.");

        // TODO: Más adelante aquí activaremos la pantalla de Game Over y pausaremos
    }

    public void ActivarVictoria()
    {
        // Si ya ganamos, ignoramos
        if (estadoActual == GameState.Win) return;

        estadoActual = GameState.Win;
        Debug.Log("GameManager: ¡Has ganado la carrera!");

        // TODO: Más adelante aquí activaremos el panel de Victoria y daremos el resumen
    }

    public void VolverAlMenuPrincipal()
    {
        // Siempre, SIEMPRE descongelar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // El Menú Principal suele ser la Escena 0 en el "Build Settings"
        SceneManager.LoadScene(0);
    }
}
