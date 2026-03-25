using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBusController : MonoBehaviour
{
    [Header("Referencias")]
    public InputHandler inputHandler;
    public Rigidbody rb;

    [Header("Configuración del Motor")]
    public float fuerzaMotor = 3000f;
    public float velocidadMaximaNormal = 120f; // km/h
    public float velocidadMaximaReversa = 40f; // Límite para reversa (¡Soluciona el bus cohete!)
    public float fuerzaFrenado = 6000f; // Le subí la fuerza para que el freno (Espacio) responda mejor

    [Header("Configuración de Dirección")]
    public float velocidadGiro = 80f;
    public float retrasoGiro = 3f;

    [Header("Datos de Lectura (No modificar)")]
    public float velocidadActualKmh;

    private float giroActual;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (inputHandler == null) inputHandler = GetComponent<InputHandler>();

        rb.centerOfMass = new Vector3(0, -1.5f, 0);
        rb.linearDamping = 0.5f;
    }

    void Update()
    {
        velocidadActualKmh = rb.linearVelocity.magnitude * 3.6f;

        if (inputHandler.tocandoClaxon)
        {
            Debug.Log("¡PIII PIIII! ¡Avanza pavo!");
        }
    }

    void FixedUpdate()
    {
        MoverBus();
        GirarBus();
        AplicarFriccionLateralYFrenado();
    }

    private void MoverBus()
    {
        // Solo podemos motorizar el bus si NO estamos pisando el botón de Freno (Barra Espaciadora)
        if (!inputHandler.frenando)
        {
            bool vaHaciaAdelante = inputHandler.aceleracion > 0;
            bool vaHaciaAtras = inputHandler.aceleracion < 0; // Cuando presionas la "S"

            // Avanzar hacia adelante con límite de 120 km/h
            if (vaHaciaAdelante && velocidadActualKmh < velocidadMaximaNormal)
            {
                rb.AddForce(transform.forward * inputHandler.aceleracion * fuerzaMotor);
            }
            // Retroceder hacia atrás con límite seguro de 40 km/h
            else if (vaHaciaAtras && velocidadActualKmh < velocidadMaximaReversa)
            {
                rb.AddForce(transform.forward * inputHandler.aceleracion * fuerzaMotor);
            }
        }
    }

    private void GirarBus()
    {
        if (rb.linearVelocity.magnitude > 1f)
        {
            float giroDeseado = inputHandler.giro * velocidadGiro;
            giroActual = Mathf.Lerp(giroActual, giroDeseado, Time.fixedDeltaTime * retrasoGiro);

            float direccionVelocidad = Vector3.Dot(rb.linearVelocity.normalized, transform.forward);
            float multiplicadorReversa = (direccionVelocidad < -0.1f) ? -1f : 1f;

            Quaternion rotacion = Quaternion.Euler(Vector3.up * giroActual * multiplicadorReversa * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * rotacion);
        }
    }

    private void AplicarFriccionLateralYFrenado()
    {
        // FRENADO CON EL BOTÓN EXCLUSIVO (Barra Espaciadora)
        if (inputHandler.frenando && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.AddForce(-rb.linearVelocity.normalized * fuerzaFrenado);
            Debug.Log("Frenando fuerte...");
        }

        // Evita que derrape
        Vector3 velocidadLateral = Vector3.Project(rb.linearVelocity, transform.right);
        rb.linearVelocity -= velocidadLateral * 0.95f;
    }
}
