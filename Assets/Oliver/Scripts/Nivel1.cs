using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class Nivel1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Menupanel;
    public GameObject BotonMenu;

    // Tema de Tiempo
    public TextMeshProUGUI tiempoTexto;
    private float tiempo = 0f;

    //Tema de Eventos
    public SpriteRenderer panelOverdrive;
    private float SiguienteEvento = 45f;
    [Header("Overdrive")]
    public float multiplicadorOverdrive = 2f;

    //Variables de Opciones
    public Slider soundSlider;
    public AudioMixer masterMixer;
    public Toggle pantallaCompletaToggle;

    //Variables para Guardar Partida;
    int nivelActual = 1;
    public GameObject panelContinuar;
    bool juegoPausado = false;
    public GeneradorNivel1 generadorDiscos;
    public SistemaEventosNivel1 sistemaEventos;

    //Metodo para Contar el Tiempo
    void Start() 
    {
        bool continuar =
        PlayerPrefs.GetInt("ContinuarPartida", 0) == 1 &&
        PlayerPrefs.GetInt("PartidaGuardada" + nivelActual, 0) == 1;

        if (continuar)
        {
            // Recuperar el tiempo inmediatamente
            tiempo = PlayerPrefs.GetFloat("TiempoGuardado" + nivelActual);

            generadorDiscos.RestaurarDiscos();
            sistemaEventos.RestaurarEstadoEvento();

            if (sistemaEventos.OverdriveActivo())
            {
                generadorDiscos.MostrarOverdriveVisual();
            }

            // Mostrar panel
            panelContinuar.SetActive(true);

            // Calcular el siguiente evento
            SiguienteEvento = Mathf.Floor(tiempo / 45f) * 45f + 45f;

            // Congelar el juego
            juegoPausado = true;

        }
        else
        {
            panelContinuar.SetActive(false);
            // Partida nueva: comenzar a generar discos
            generadorDiscos.IniciarGeneracion();
        }

        Time.timeScale = 1f;

        //Buscamos si hay volumen guardado
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenDelJuego", 1f);

        //Movemos el slider al volumen guardado
        if (soundSlider != null)
        {
            soundSlider.value = volumenGuardado;
        }

        float decibelios = Mathf.Log10(volumenGuardado) * 20;
        masterMixer.SetFloat("MasterVolume", decibelios);

        //Pantalla Completa Inicial
        int pantallaGuardada = PlayerPrefs.GetInt("PantallaGuardada", 0);

        bool esCompleta = (pantallaGuardada == 1);

        if (pantallaCompletaToggle != null)
        {
            pantallaCompletaToggle.isOn = esCompleta;
        }
        Screen.fullScreen = esCompleta;
    }
    
    void Update() 
    {
        if (!juegoPausado)
        {
            tiempo += Time.deltaTime;
        }

        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        tiempoTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (tiempo >= SiguienteEvento)
        {
            int evento = Random.Range(0, 2);
            Debug.Log("Evento elegido = " + evento);

            if (evento == 0)
            {
                Debug.Log("Voy a ejecutar Overdrive");
                sistemaEventos.EjecutarOverdrive();
            }
            else
            {
                Debug.Log("Voy a ejecutar Blackout");
                sistemaEventos.EjecutarBlackout();
            }

            SiguienteEvento += 45f;
        }

    }

    public void CambiarVolumen(float volumen)
    {
        float decibelios = Mathf.Log10(volumen) * 20;
        masterMixer.SetFloat("MasterVolume", decibelios);

        //Guardar Cambio
        PlayerPrefs.SetFloat("VolumenDelJuego", volumen);
        PlayerPrefs.Save();
        Debug.Log("El volumen actual en dB es: " + decibelios);
    }

    public void CambiarPantallaCompleta(bool activado)
    {
        Screen.fullScreen = activado;

        PlayerPrefs.SetInt("PantallaGuardada", activado ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Guardado en memoria. ¿Pantalla completa?: " + activado);
    }


    //Boton para abrir el panel
    public void OpenMenuPanel()
    {
        BotonMenu.SetActive(false);
        Time.timeScale = 0f;
        Menupanel.SetActive(true);
    }

    //Botones de Panel Menu
    public void VolverAlNivel()
    {
        Menupanel.SetActive(false);
        Time.timeScale = 1f;
        BotonMenu.SetActive(true);
    }

    public void TerminarPartida()
    {
        BotonMenu.SetActive(false);
        Menupanel.SetActive(true);

        //Mandar datos de partida
        PlayerPrefs.SetString("UltimoNombre",PlayerPrefs.GetString("NombreJugador"));
        PlayerPrefs.SetFloat("UltimoTiempo",tiempo);
        PlayerPrefs.SetInt("UltimoNivel",nivelActual);

        PlayerPrefs.SetInt("AbrirMenuNiveles", 1);

        PlayerPrefs.DeleteKey("PartidaGuardada" + nivelActual);
        PlayerPrefs.DeleteKey("TiempoGuardado" + nivelActual);

        PlayerPrefs.DeleteKey("Nivel1_CantidadDiscos");

        for (int i = 0; i < 200; i++)
        {
            PlayerPrefs.DeleteKey("Nivel1_Tipo_" + i);
            PlayerPrefs.DeleteKey("Nivel1_PosX_" + i);
            PlayerPrefs.DeleteKey("Nivel1_PosY_" + i);
            PlayerPrefs.DeleteKey("Nivel1_DirX_" + i);
            PlayerPrefs.DeleteKey("Nivel1_DirY_" + i);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void GuardarPartida()
    {
        // Indicar que existe una partida guardada
        PlayerPrefs.SetInt("PartidaGuardada" + nivelActual, 1);

        PlayerPrefs.SetFloat("TiempoGuardado" + nivelActual, tiempo);

        generadorDiscos.GuardarDiscos();
        sistemaEventos.GuardarEstadoEvento();

        // Regresar al menú de niveles
        PlayerPrefs.SetInt("AbrirMenuNiveles", 1);

        SceneManager.LoadScene("MainMenu");
    }

    //Botones para Guardar Partida

    public void NoContinuar()
    {
        PlayerPrefs.DeleteKey(
        "PartidaGuardada" + nivelActual);

        PlayerPrefs.DeleteKey(
            "TiempoGuardado" + nivelActual);
        PlayerPrefs.DeleteKey("Nivel1_CantidadDiscos");
        for (int i = 0; i < 200; i++)
        {
            PlayerPrefs.DeleteKey("Nivel1_Tipo_" + i);
            PlayerPrefs.DeleteKey("Nivel1_PosX_" + i);
            PlayerPrefs.DeleteKey("Nivel1_PosY_" + i);
            PlayerPrefs.DeleteKey("Nivel1_DirX_" + i);
            PlayerPrefs.DeleteKey("Nivel1_DirY_" + i);
        }
        generadorDiscos.LimpiarDiscos();

        juegoPausado = false;
        panelContinuar.SetActive(false);

        sistemaEventos.ReiniciarEvento();
        tiempo = 0f;
        SiguienteEvento = 45f;
        generadorDiscos.IniciarGeneracion();
    }

    public void SeguirPartida()
    {
        if (sistemaEventos.OverdriveActivo())
        {
            generadorDiscos.OcultarOverdriveVisual();
        }

        generadorDiscos.IniciarGeneracion();
        generadorDiscos.ReanudarDiscos();

        sistemaEventos.ContinuarEvento();

        panelContinuar.SetActive(false);
        juegoPausado = false;

    }

    public void VolveryGuardar() 
    {
        PlayerPrefs.SetInt("AbrirMenuNiveles", 1);
        SceneManager.LoadScene("MainMenu");
    }
}
