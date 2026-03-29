using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Proyecto")]
    [Tooltip("El nombre exacto de la escena que debe cargar al presionar Jugar. (Ejemplo: City Scene)")]
    public string nombreEscenaJuego = "LA-CITY";
    [Header("Paneles Principales")]
    public GameObject panelPrincipal;
    public GameObject panelAjustes;
    public GameObject panelCreditos;
    [Header("Sub-Paneles de Ajustes")]
    public GameObject contenidoGeneral;
    public GameObject contenidoControles;
    [Header("Navegacion Control/Teclado")]
    public GameObject botonJugar;      // Primer botón a resaltar
    public GameObject botonGeneral;    // Botón de pestaña sonido/brillo
    public GameObject botonVolverCreditos;
    [Header("Ajustes de Video y Audio")]
    public Slider sliderSonido;
    public Slider sliderBrillo;
    public Image overlayBrillo; // Imagen negra transparente
    [Header("Configuración de Créditos")]
    public RectTransform rectTextoCreditos;
    public float velocidadCreditos = 45f;
    private bool mostrandoCreditos = false;
    private Vector3 posicionInicialCreditos;
    void Start()
    {
        posicionInicialCreditos = rectTextoCreditos.anchoredPosition;
        CargarPreferencias();
        IrAlPanelPrincipal(); // Inicia siempre en la cara principal
    }
    void Update()
    {
        if (mostrandoCreditos)
        {
            rectTextoCreditos.anchoredPosition += Vector2.up * velocidadCreditos * Time.deltaTime;
            // Salir de créditos con Esc o Botón B/Círculo
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")) IrAlPanelPrincipal();
        }
    }
    // --- FUNCIONES DE LOS 5 BOTONES PRINCIPALES ---
    public void FuncionJugar()
    {
        // Carga el nombre de la escena que escribiste manualmente en el Inspector
        if (!string.IsNullOrEmpty(nombreEscenaJuego))
        {
            SceneManager.LoadScene(nombreEscenaJuego);
        }
        else
        {
            Debug.LogError("MainMenuManager: El nombre de la escena de juego está vacío en el Inspector.");
        }
    }
    public void FuncionNuevaPartida()
    {
        // Borra el progreso y arranca de cero la misma escena principal
        PlayerPrefs.DeleteKey("NivelGuardado");
        PlayerPrefs.Save();
        FuncionJugar();
    }
    public void AbrirAjustes()
    {
        panelPrincipal.SetActive(false);
        panelAjustes.SetActive(true);
        AbrirTabGeneral(); // Abre por defecto sonido y brillo
    }
    public void AbrirCreditos()
    {
        panelPrincipal.SetActive(false);
        panelCreditos.SetActive(true);
        rectTextoCreditos.anchoredPosition = posicionInicialCreditos; // Reinicia posición
        mostrandoCreditos = true;
        SeleccionarParaControl(botonVolverCreditos);
    }
    public void SalirDelJuegoTotalmente()
    {
        Debug.Log("Cerrando el Diablo Rojo...");
        Application.Quit();
    }
    // --- NAVEGACIÓN INTERNA ---
    public void IrAlPanelPrincipal()
    {
        mostrandoCreditos = false;
        panelAjustes.SetActive(false);
        panelCreditos.SetActive(false);
        panelPrincipal.SetActive(true);
        SeleccionarParaControl(botonJugar);
    }
    public void AbrirTabGeneral()
    {
        contenidoGeneral.SetActive(true);
        contenidoControles.SetActive(false);
        SeleccionarParaControl(botonGeneral);
    }
    public void AbrirTabControles()
    {
        contenidoGeneral.SetActive(false);
        contenidoControles.SetActive(true);
    }
    // --- LÓGICA DE SLIDERS ---
    public void CambiarSonido(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat("Vol", v);
    }
    public void CambiarBrillo(float v)
    {
        if (overlayBrillo != null)
        {
            Color c = overlayBrillo.color;
            c.a = v; // El slider debe ir de 0 a 0.8
            overlayBrillo.color = c;
        }
        PlayerPrefs.SetFloat("Bri", v);
    }
    private void CargarPreferencias()
    {
        sliderSonido.value = PlayerPrefs.GetFloat("Vol", 0.8f);
        sliderBrillo.value = PlayerPrefs.GetFloat("Bri", 0f);
        CambiarBrillo(sliderBrillo.value);
    }
    private void SeleccionarParaControl(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(obj);
    }
}