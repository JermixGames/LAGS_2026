using UnityEngine;

public class BusEffectsController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerBusController busController;
    [Tooltip("Arrastra aquí tu Carrocería o Modelo Visual del Bus. Si dejas esto vacío, animará todo el GameObject")]
    public Transform modeloVisual;

    [Header("Efectos de Humo (Braking / Drifting)")]
    [Tooltip("Suma tus Particle Systems de las llantas traseras aquí")]
    public ParticleSystem[] humoLlantas;
    public float velocidadMinimaHumo = 20f; // km/h

    [Header("Juice: Squash & Stretch")]
    public Vector3 squishScale = new Vector3(1.2f, 0.7f, 1.2f); // Bus aplastado, "chato"
    public Vector3 stretchScale = new Vector3(0.9f, 1.15f, 0.9f); // Bus estirado para arriba
    public float velocidadRebote = 12f;
    
    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;

    void Start()
    {
        if (busController == null) busController = GetComponentInParent<PlayerBusController>();
        if (modeloVisual == null) modeloVisual = this.transform;

        escalaOriginal = modeloVisual.localScale;
        escalaObjetivo = escalaOriginal;
        
        // Apagamos los emisores de humo para empezar
        foreach(var humo in humoLlantas) {
            if(humo != null) {
                var em = humo.emission;
                em.enabled = false;
            }
        }
    }

    void Update()
    {
        ControlarHumo();
        ControlarResorteVisual();
    }

    private void ControlarHumo()
    {
        if (busController == null || humoLlantas.Length == 0) return;

        // Está usando la barra espaciadora (freno duro) a alta velocidad
        bool frenandoDuro = busController.inputHandler.frenando && busController.velocidadActualKmh > velocidadMinimaHumo;
        
        // Fuerzas de fricción G lateral = Está derrapando
        Vector3 velLateral = Vector3.Project(busController.rb.linearVelocity, busController.transform.right);
        bool derrapando = velLateral.magnitude > 3f && busController.velocidadActualKmh > velocidadMinimaHumo;

        bool emitiendoHumo = frenandoDuro || derrapando;

        for (int i = 0; i < humoLlantas.Length; i++)
        {
            if (humoLlantas[i] != null)
            {
                var emision = humoLlantas[i].emission;
                emision.enabled = emitiendoHumo;
            }
        }
    }

    private void ControlarResorteVisual()
    {
        // Se mueve lentamente hacia su objetivo actual (stretchScale o normalidad)
        modeloVisual.localScale = Vector3.Lerp(modeloVisual.localScale, escalaObjetivo, Time.deltaTime * velocidadRebote);
        
        // Simula la gravedad del caucho de la "gelatina" regresando a su base original
        if (Vector3.Distance(escalaObjetivo, escalaOriginal) > 0.01f)
        {
            escalaObjetivo = Vector3.Lerp(escalaObjetivo, escalaOriginal, Time.deltaTime * (velocidadRebote * 0.5f));
        }
    }

    // MÁGIA: Lo llamamos desde afuera cada vez que el peso cambia (sube un panameño al bus)
    public void AplicarSquish()
    {
        // Empezamos chatos y ordenamos un rebote estirado al siguiente frame
        modeloVisual.localScale = squishScale;
        escalaObjetivo = stretchScale;
    }
}
