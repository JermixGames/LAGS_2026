using UnityEngine;

// Este script es el "Interruptor" que diferencia esquinas normales de paradas de bus reales.
public class WaypointInformativo : MonoBehaviour
{
    [Tooltip("Marca esto SOLAMENTE si quieres que el Bus Rival se detenga aquí por 3 segundos. Si está en blanco, el bus seguirá de largo a toda velocidad.")]
    public bool esParadaObligatoria = false;
}