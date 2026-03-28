using UnityEngine;

public class DynamicBusCamera : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí tu DiabloRojo (el que tiene el PlayerBusController)")]
    public PlayerBusController busController;
    private Camera cam;

    [Header("Posicionamiento Relativo")]
    [Tooltip("Distancia a espaldas del Bus (Ej: X:0, Y:4, Z:-10)")]
    public Vector3 offset = new Vector3(0, 4.5f, -12f);
    public float smoothSpeed = 10f;
    public float rotationSmoothSpeed = 10f;

    [Header("Sensación de Velocidad (FOV)")]
    [Tooltip("El zoom se ampliará cuando vayas rápido dando sensación de vértigo")]
    public float fovBase = 60f;
    public float fovMaximo = 85f;
    public float velocidadParaFovMaximo = 100f; // km/h

    [Header("Inclinación (Tilt) al Girar")]
    [Tooltip("Al girar, la cámara se ladea como la cabeza del conductor")]
    public float maxTiltAngle = 5f;
    public float tiltSmoothSpeed = 5f;

    [Header("Vibración de Velocidad (Chasis Vibrando)")]
    public float shakeIntensityMax = 0.15f;
    public float velocidadParaShake = 70f; // A partir de esta velocidad, empieza a temblar

    private float currentTilt = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        // Auto-detectar y desparentar: ¡Mágicamente saca la cámara del DiabloRojo si olvidaste hacerlo!
        if (transform.parent != null && transform.parent.GetComponent<PlayerBusController>() != null)
        {
            Debug.Log("DynamicBusCamera: Separando la cámara del DiabloRojo para evitar doble movimiento (Child Rígido)...");
            transform.parent = null; 
        }
    }

    void FixedUpdate()
    {
        if (busController == null) return;

        // 1. Recibir variables rápidas del Diablo Rojo
        float speed = busController.velocidadActualKmh;
        float steerInput = busController.inputHandler != null ? busController.inputHandler.giro : 0f;

        // 2. FOV Dinámico (El viento en la cara de rápido)
        float targetFov = Mathf.Lerp(fovBase, fovMaximo, speed / velocidadParaFovMaximo);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.fixedDeltaTime * 2f);

        // 3. Vibración del Chasis (Solo a altas velocidades, mayor a 70km/h)
        Vector3 shakeOffset = Vector3.zero;
        if (speed > velocidadParaShake)
        {
            float ratioShake = Mathf.InverseLerp(velocidadParaShake, 120f, speed); 
            // 120 es el maxNormal en el busController
            shakeOffset = Random.insideUnitSphere * (ratioShake * shakeIntensityMax);
        }

        // 4. Perseguir al Autobús Suavemente
        Vector3 rotadaAlBus = busController.transform.TransformDirection(offset);
        Vector3 targetPosition = busController.transform.position + rotadaAlBus + shakeOffset;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);

        // 5. Girar e Inclinarse (Tilt de gravedad lateral)
        // Negativo para que, si gira a la derecha (+), la cámara se ladee físicamente a la izquierda (-) un poco
        float targetTilt = steerInput * -maxTiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSmoothSpeed * Time.fixedDeltaTime);
        
        // Mezclamos la rotación actual del autobús con nuestra breve inclinación en Z
        Quaternion currentRot = busController.transform.rotation;
        Quaternion tiltRot = Quaternion.Euler(0, 0, currentTilt);
        Quaternion finalRot = currentRot * tiltRot;

        // Mover la mirada
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRot, rotationSmoothSpeed * Time.fixedDeltaTime);
    }
}
