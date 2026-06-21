using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

public class Nivel2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Menupanel;
    public GameObject BotonMenu;

    // Tema de Tiempo
    public TextMeshProUGUI tiempoTexto;
    private float tiempo = 0f;

    //Tema de Eventos
    public Image panelOverdrive;
    private float SiguienteEvento = 45f;

    //Variables de Opciones
    public Slider soundSlider;
    public AudioMixer masterMixer;
    public Toggle pantallaCompletaToggle;

    //Variables para Guardar Partida;
    int nivelActual = 2;
    public GameObject panelContinuar;
    bool juegoPausado = false;

    //Metodo para Contar el Tiempo
    void Start()
    {
        if (PlayerPrefs.GetInt("ContinuarPartida", 0) == 1 &&
       PlayerPrefs.GetInt("PartidaGuardada" + nivelActual, 0) == 1)
        {
            // Recuperar el tiempo inmediatamente
            tiempo = PlayerPrefs.GetFloat("TiempoGuardado" + nivelActual);

            // Mostrar panel
            panelContinuar.SetActive(true);

            // Congelar el juego
            juegoPausado = true;

            // Calcular el siguiente evento
            SiguienteEvento = Mathf.Floor(tiempo / 45f) * 45f + 45f;

            // Ya consumimos el mensaje enviado desde MainMenu
            PlayerPrefs.SetInt("ContinuarPartida", 0);
        }
        else
        {
            panelContinuar.SetActive(false);
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

        tiempo += Time.deltaTime;

        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        tiempoTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (tiempo >= SiguienteEvento)
        {
            int evento = Random.Range(0, 2);

            if (evento == 0)
            {
                StartCoroutine(Overdrive());
            }
            else
            {
                StartCoroutine(Blackout());
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

    //Evento Blackout
    IEnumerator Blackout()
    {
        Color colorOriginal = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.8f);

        float duracionTransicion = 1f;
        float t = 0f;

        //Entrada Gradual
        while (t < duracionTransicion)
        {
            t += Time.deltaTime;

            panelOverdrive.color = Color.Lerp(colorOriginal, colorEvento, t / duracionTransicion);

            yield return null;
        }

        //Evento Activo
        yield return new WaitForSeconds(8f);

        //Salida Gradual
        t = 0f;

        while (t < duracionTransicion)
        {
            t += Time.deltaTime;

            panelOverdrive.color = Color.Lerp(colorEvento, colorOriginal, t / duracionTransicion);

            yield return null;
        }

        panelOverdrive.color = colorOriginal;
    }

    //Evento Overdrive
    IEnumerator Overdrive()
    {
        Color colorOriginal = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.3f);

        float duracionTransicion = 1f;
        float t = 0f;

        //Entrada Gradual
        while (t < duracionTransicion)
        {
            t += Time.deltaTime;

            panelOverdrive.color = Color.Lerp(colorOriginal, colorEvento, t / duracionTransicion);

            yield return null;
        }

        //Evento Activo
        yield return new WaitForSeconds(8f);

        //Salida Gradual
        t = 0f;

        while (t < duracionTransicion)
        {
            t += Time.deltaTime;

            panelOverdrive.color = Color.Lerp(colorEvento, colorOriginal, t / duracionTransicion);

            yield return null;
        }

        panelOverdrive.color = colorOriginal;
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
        PlayerPrefs.SetString("UltimoNombre", PlayerPrefs.GetString("NombreJugador"));
        PlayerPrefs.SetFloat("UltimoTiempo", tiempo);
        PlayerPrefs.SetInt("UltimoNivel", nivelActual);

        PlayerPrefs.SetInt("AbrirMenuNiveles", 1);
        SceneManager.LoadScene("MainMenu");
    }

    public void GuardarPartida()
    {
        // Indicar que existe una partida guardada
        PlayerPrefs.SetInt("PartidaGuardada" + nivelActual, 1);

        PlayerPrefs.SetFloat("TiempoGuardado" + nivelActual, tiempo);

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

        juegoPausado = false;
        panelContinuar.SetActive(false);

        tiempo = 0;
    }

    public void SeguirPartida()
    {

        panelContinuar.SetActive(false);
        juegoPausado = false;
    }

    public void VolveryGuardar()
    {
        PlayerPrefs.SetInt("AbrirMenuNiveles", 1);
        SceneManager.LoadScene("MainMenu");
    }
}
