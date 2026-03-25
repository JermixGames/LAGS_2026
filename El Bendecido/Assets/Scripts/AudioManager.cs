using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Asigna tus AudioSources aquí (En el inspector)")]
    public AudioSource motorSource;
    public AudioSource claxonSource;
    public AudioSource frenoSource;
    public AudioSource vocesSource; // Para el Pavo y Bus Lleno

    [Header("Clips sueltos")]
    public AudioClip choqueClip;
    public AudioClip busLlenoClip;

    private PlayerBusController playerBus;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (playerBus == null)
        {
            // Busca al Diablo Rojo automáticamente en la escena
            var p = Object.FindAnyObjectByType<PlayerBusController>();
            if (p != null) playerBus = p;
            return;
        }

        // Variar el "ronroneo" del motor según la velocidad del bus, como en los juegos Arcade
        if (motorSource != null && motorSource.isPlaying)
        {
            float pitch = 1f + (playerBus.velocidadActualKmh / 120f) * 1.5f;
            motorSource.pitch = Mathf.Clamp(pitch, 1f, 3f);
        }
    }

    public void TocarClaxon()
    {
        if (claxonSource != null && !claxonSource.isPlaying) claxonSource.Play();
    }

    public void TocarFreno()
    {
        if (frenoSource != null && !frenoSource.isPlaying) frenoSource.Play();
    }

    public void TocarChoque()
    {
        if (motorSource != null && choqueClip != null)
            motorSource.PlayOneShot(choqueClip);
    }

    public void ReproducirAudioEspecial(AudioClip clip)
    {
        if (vocesSource != null && clip != null)
            vocesSource.PlayOneShot(clip);
    }

    public void ReproducirBusLleno()
    {
        if (vocesSource != null && busLlenoClip != null)
            vocesSource.PlayOneShot(busLlenoClip);
    }
}
