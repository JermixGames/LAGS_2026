using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject panelPausa;
    public GameObject botonReanudar; // El que dice "Continuar"
    private bool enPausa = false;

    void Update()
    {
        // Detecta Esc o Start del control
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (enPausa) FuncionContinuar(); else ActivarPausa();
        }
    }

    public void ActivarPausa()
    {
        enPausa = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f; // Congela el bus
        EventSystem.current.SetSelectedGameObject(botonReanudar);
    }

    public void FuncionContinuar()
    {
        enPausa = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f; // El bus arranca
    }

    public void FuncionSalirAlMenu()
    {
        Time.timeScale = 1f; // ¡Obligatorio!
        SceneManager.LoadScene(0); // Vuelve al Menú Principal (Escena 0)
    }
}