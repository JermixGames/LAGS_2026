using UnityEngine;
using System.Collections;

public class PowerUpSystem : MonoBehaviour
{
    public static PowerUpSystem Instance;

    [Header("Referencias OBLIGATORIAS")]
    public PlayerBusController busController;
    public InputHandler inputHandler;

    private float velocidadOriginalNormal;
    private float velocidadOriginalGiro;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Guardamos los valores originales del Bus para poder restaurarlos despues de los power-ups
        if (busController != null)
        {
            velocidadOriginalNormal = busController.velocidadMaximaNormal;
            velocidadOriginalGiro = busController.velocidadGiro;
        }
    }

    void Update()
    {
        // Disparar Nitro con Shift Izquierdo (Leído desde InputHandler)
        if (inputHandler != null && inputHandler.usandoNitro)
        {
            if (PassengerSystem.Instance.cargasDeNitro > 0)
            {
                StartCoroutine(ActivarNitroRutina());
            }
            else
            {
                Debug.Log("⚠️ ¡No tienes latas de nitro! Necesitas recoger un Chacalito.");
            }
        }
    }

    public void ActivarBorracho()
    {
        StartCoroutine(EfectoBorracho());
    }

    public void ActivarViejita()
    {
        StartCoroutine(EfectoViejita());
    }

    private IEnumerator EfectoBorracho()
    {
        if (busController != null) busController.velocidadGiro = -velocidadOriginalGiro; // Derecha es Izquierda
        yield return new WaitForSeconds(8f); // 8 segundos de tortura
        if (busController != null) busController.velocidadGiro = velocidadOriginalGiro;
        Debug.Log("El borrachito se bajó... el timón responde normal de nuevo.");
    }

    private IEnumerator EfectoViejita()
    {
        if (busController != null) busController.velocidadMaximaNormal = 60f; // Limitado a la mitad
        yield return new WaitForSeconds(6f);
        if (busController != null) busController.velocidadMaximaNormal = velocidadOriginalNormal;
        Debug.Log("La viejita se bajó... ya puedes pisar el acelerador.");
    }

    private IEnumerator ActivarNitroRutina()
    {
        PassengerSystem.Instance.cargasDeNitro--;
        Debug.Log($"🔥 ¡NITRO ACTIVADO! Te quedan {PassengerSystem.Instance.cargasDeNitro} botellas.");

        // Te pasas el límite de 120kmh a 200kmh por 3 segundos
        if (busController != null)
        {
            busController.velocidadMaximaNormal = 200f;
            busController.fuerzaMotor *= 2f; // El doble de empuje
        }

        yield return new WaitForSeconds(3f);

        if (busController != null)
        {
            busController.velocidadMaximaNormal = velocidadOriginalNormal;
            busController.fuerzaMotor /= 2f;
        }
        Debug.Log("Nitro completado.");
    }
}
