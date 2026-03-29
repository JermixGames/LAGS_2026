using UnityEngine;
using TMPro; // TextMeshPro para UI moderna

public class UIManager : MonoBehaviour
{
    [Header("Textos de Interfaz (Asignar en Inspector)")]
    public TextMeshProUGUI txtVelocidad;
    public TextMeshProUGUI txtPasajeros;
    public TextMeshProUGUI txtDinero;
    public TextMeshProUGUI txtNitro;

    [Header("Bus a monitorear")]
    public PlayerBusController playerBus;

    void Update()
    {
        if (playerBus != null && txtVelocidad != null)
        {
            txtVelocidad.text = $"{Mathf.RoundToInt(playerBus.velocidadActualKmh)} KM/H";
        }

        if (PassengerSystem.Instance != null && txtPasajeros != null)
        {
            // Muestra: Pasajeros: 15 / Meta: 30
            txtPasajeros.text = $": {PassengerSystem.Instance.pasajerosActuales} / {PassengerSystem.Instance.metaPasajeros}";

            // Dinero: $5.00 / Meta: $7.50
            txtDinero.text = $": ${PassengerSystem.Instance.dineroActual / 100f:F2} / ${(PassengerSystem.Instance.metaDinero / 100f):F2}";

            txtNitro.text = $": {PassengerSystem.Instance.cargasDeNitro}";
        }
    }
}
