using UnityEngine;

public class TrafficPoliceCollisionHandler : MonoBehaviour
{
    private int choquesTrafico = 0;
    private int choquesPolicia = 0;
    private PlayerBusController busPlayer;

    void Start() { busPlayer = GetComponent<PlayerBusController>(); }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TrafficCar"))
        {
            choquesTrafico++;
            Debug.Log($"💥 ¡Chocaste un Civil! ({choquesTrafico}/3 choques)");
            AudioManager.Instance?.TocarChoque();

            if (choquesTrafico == 2)
            {
                Debug.Log("📉 ¡Perdiste 5 pasajeros que huyeron de miedo y botaste 2 dólares de ganancia!");
                PassengerSystem.Instance.pasajerosActuales = Mathf.Max(0, PassengerSystem.Instance.pasajerosActuales - 5);
                PassengerSystem.Instance.dineroActual = Mathf.Max(0, PassengerSystem.Instance.dineroActual - 200);
            }
            else if (choquesTrafico >= 3)
            {
                Debug.Log("☠️ ¡DESTROZASTE EL BUS Y HUBO HERIDOS! Game Over.");
                GameManager.Instance.ActivarGameOver();
            }
        }
        else if (collision.gameObject.CompareTag("PoliceCar"))
        {
            choquesPolicia++;
            Debug.Log($"👮 ¡La Policía te embistió! ({choquesPolicia}/2 choques)");
            AudioManager.Instance?.TocarChoque();

            if (choquesPolicia == 1)
            {
                Debug.Log("🚔 ¡El oficial Lince te rompió el motor por persecución! Tu aceleración ha bajado drásticamente.");
                busPlayer.fuerzaMotor *= 0.6f;
            }
            else if (choquesPolicia >= 2)
            {
                Debug.Log("⚖️ ¡ARRESTADO! El Lince y el tránsito te atraparon. Game Over.");
                GameManager.Instance.ActivarGameOver();
            }
        }
    }
}