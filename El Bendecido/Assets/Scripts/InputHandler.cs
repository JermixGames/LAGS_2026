using UnityEngine;
// Este script simplemente lee los botones que toca el jugador.
// Separar esto hace que tu código sea mucho más limpio y profesional (modular).
public class InputHandler : MonoBehaviour
{
    [Header("Entradas del Jugador")]
    public float aceleracion;
    public float giro;
    public bool frenando;
    public bool empezoAFrenar; // Para activar el sonido de freno solo una vez
    public bool tocandoClaxon;
    public bool usandoNitro;
    void Update()
    {
        // WS o Flechas Arriba/Abajo (-1 a 1)
        aceleracion = Input.GetAxis("Vertical");

        // AD o Flechas Izquierda/Derecha (-1 a 1)
        giro = Input.GetAxis("Horizontal");

        // Espacio para frenar de golpe
        frenando = Input.GetKey(KeyCode.Space);
        empezoAFrenar = Input.GetKeyDown(KeyCode.Space);

        // Claxon en la tecla H (Solo registra el momento exacto en que se presiona)
        tocandoClaxon = Input.GetKeyDown(KeyCode.H);

        // Nitro en el Shift Izquierdo
        usandoNitro = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
