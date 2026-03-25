using UnityEngine;
using System.Collections;

public class ObstacleSystem : MonoBehaviour
{
    public enum TipoObstaculo { Bache, Resalto }
    public TipoObstaculo tipo;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var busController = other.GetComponentInParent<PlayerBusController>();
            if (busController != null)
            {
                if (tipo == TipoObstaculo.Bache)
                {
                    Debug.Log("⚠️ ¡Caíste en un cráter de la Transístmica! (-15% de velocidad hasta que recojas pasajeros).");
                    busController.velocidadMaximaNormal *= 0.85f;
                    // Eliminar el bache tras caer o dejarlo para fastidiar
                }
                else if (tipo == TipoObstaculo.Resalto)
                {
                    Debug.Log("⚠️ ¡Saltaste un Policía Muerto muy duro! (-5% de velocidad por 5 seg).");
                    StartCoroutine(PenalizacionResalto(busController));
                }
            }
        }
    }

    private IEnumerator PenalizacionResalto(PlayerBusController bus)
    {
        float originalMax = bus.velocidadMaximaNormal;
        bus.velocidadMaximaNormal *= 0.95f;
        yield return new WaitForSeconds(5f);
        bus.velocidadMaximaNormal = originalMax;
    }
}
