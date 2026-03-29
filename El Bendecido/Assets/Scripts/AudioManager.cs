using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Asigna tus AudioSources aquí (En el inspector)")]
    public AudioSource motorSource;
    public AudioSource claxonSource;
    public AudioSource frenoSource;
    public AudioSource vocesSource; // Para el Pavo y Bus Lleno
    public AudioSource sirenaSource; // Audio dedicado de la Policía

    [Header("Música de Ambiente (Playlist)")]
    public AudioSource musicaSource;
    public AudioClip[] playlistAmbiental;

    [Header("Clips sueltos")]
    public AudioClip choqueClip;
    public AudioClip busLlenoClip;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        if (musicaSource != null && playlistAmbiental.Length > 0)
        {
            ReproducirMúsicaRandom();
        }
    }
    void Update()
    {
        // Revisa si la música se acabó de reproducir para poner la siguiente
        if (musicaSource != null && playlistAmbiental.Length > 0)
        {
            if (!musicaSource.isPlaying)
            {
                ReproducirMúsicaRandom();
            }
        }
    }
    private void ReproducirMúsicaRandom()
    {
        if (musicaSource == null || playlistAmbiental.Length == 0) return;
        int randomIdx = Random.Range(0, playlistAmbiental.Length);
        musicaSource.clip = playlistAmbiental[randomIdx];
        musicaSource.Play();
    }
    // NUEVO MÉTODO BASADO EN EVENTOS PARA EL MOTOR
    public void ActualizarMotor(bool enMovimiento, float factorVelocidad)
    {
        if (motorSource == null) return;

        if (enMovimiento)
        {
            // Solo darle Play si no estaba sonando ya (evita tartamudeos)
            if (!motorSource.isPlaying) motorSource.Play();

            // Variar el tono para que suene como un motor revolucionando
            motorSource.pitch = Mathf.Clamp(1f + (factorVelocidad * 1.5f), 1f, 3f);
        }
        else
        {
            // Apaga el motor automáticamente si el bus no se mueve
            if (motorSource.isPlaying) motorSource.Stop();
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
    public void TocarSirenaPolicia()
    {
        if (sirenaSource != null && !sirenaSource.isPlaying) sirenaSource.Play();
    }
    public void DetenerSirenaPolicia()
    {
        if (sirenaSource != null && sirenaSource.isPlaying) sirenaSource.Stop();
    }
}