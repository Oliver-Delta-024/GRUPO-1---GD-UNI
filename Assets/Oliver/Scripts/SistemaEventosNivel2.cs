using UnityEngine;
using System.Collections;

public class SistemaEventosNivel2 : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer panelOverdrive;
    public GeneradorNivel2 generadorDiscos;
    private Color colorOriginal;

    [Header("Overdrive")]
    public float multiplicadorOverdrive = 2f;
    private string prefijoEvento = "Nivel2_Evento_";

    [Header("Tiempos")]
    public float tiempoEntrada = 1f;
    public float tiempoActivo = 8f;
    public float tiempoSalida = 1f;

    private Coroutine rutinaEvento;

    private bool eventoActivo = false;

    private bool overdriveActivo = false;
    private bool blackoutActivo = false;

    // 0 = Ninguna
    // 1 = Entrada
    // 2 = Activo
    // 3 = Salida
    private int faseEvento = 0;

    // Tiempo transcurrido dentro de la fase actual
    private float tiempoFase = 0f;

    public bool HayEventoActivo()
    {
        return eventoActivo;
    }

    public bool OverdriveActivo()
    {
        return overdriveActivo;
    }

    public bool BlackoutActivo()
    {
        return blackoutActivo;
    }

    public int ObtenerFase()
    {
        return faseEvento;
    }

    public float ObtenerTiempoFase()
    {
        return tiempoFase;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginal = panelOverdrive.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //==========================================
    // CONTROL GENERAL
    //==========================================

    public IEnumerator Overdrive()
    {
        eventoActivo = true;
        overdriveActivo = true;
        blackoutActivo = false;

        faseEvento = 1;
        tiempoFase = 0f;

        Color colorOriginal = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.3f);

        float t = 0f;

        // Entrada
        while (t < tiempoEntrada)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            panelOverdrive.color =
                Color.Lerp(
                    colorOriginal,
                    colorEvento,
                    t / tiempoEntrada);

            yield return null;
        }
        faseEvento = 2;
        tiempoFase = 0f;
        panelOverdrive.color = colorEvento;
        Debug.Log("SistemaEventos -> Activando Overdrive");
        generadorDiscos.ActivarOverdrive(multiplicadorOverdrive);

        // Activo
        t = 0f;

        while (t < tiempoActivo)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            yield return null;
        }

        generadorDiscos.DesactivarOverdrive();

        // Salida
        faseEvento = 3;
        tiempoFase = 0f;
        t = 0f;

        while (t < tiempoSalida)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            panelOverdrive.color =
                Color.Lerp(
                    colorEvento,
                    colorOriginal,
                    t / tiempoSalida);

            yield return null;
        }

        panelOverdrive.color = colorOriginal;

        eventoActivo = false;
        overdriveActivo = false;

        faseEvento = 0;
        tiempoFase = 0f;
        rutinaEvento = null;
    }

    IEnumerator Blackout()
    {
        eventoActivo = true;
        overdriveActivo = false;
        blackoutActivo = true;

        faseEvento = 1;
        tiempoFase = 0f;

        Color colorOriginal = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.8f);

        float t = 0f;

        //-----------------------
        // Entrada
        //-----------------------

        while (t < tiempoEntrada)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            panelOverdrive.color = Color.Lerp(
                colorOriginal,
                colorEvento,
                t / tiempoEntrada);

            yield return null;
        }
        faseEvento = 2;
        tiempoFase = 0f;

        panelOverdrive.color = colorEvento;

        //-----------------------
        // Evento activo
        //-----------------------

        t = 0f;

        while (t < tiempoActivo)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            yield return null;
        }

        //-----------------------
        // Salida
        //-----------------------

        faseEvento = 3;
        tiempoFase = 0f;
        t = 0f;

        while (t < tiempoSalida)
        {
            t += Time.deltaTime;
            tiempoFase = t;

            panelOverdrive.color = Color.Lerp(
                colorEvento,
                colorOriginal,
                t / tiempoSalida);

            yield return null;
        }

        panelOverdrive.color = colorOriginal;

        eventoActivo = false;
        blackoutActivo = false;

        faseEvento = 0;
        tiempoFase = 0f;
        rutinaEvento = null;
    }

    public void EjecutarOverdrive()
    {
        if (eventoActivo)
            return;

        rutinaEvento = StartCoroutine(Overdrive());
    }

    public void EjecutarBlackout()
    {
        if (eventoActivo)
            return;

        rutinaEvento = StartCoroutine(Blackout());
    }

    public void GuardarEstadoEvento()
    {
        PlayerPrefs.SetInt(
            prefijoEvento + "EventoActivo",
            eventoActivo ? 1 : 0);

        PlayerPrefs.SetInt(
            prefijoEvento + "Overdrive",
            overdriveActivo ? 1 : 0);

        PlayerPrefs.SetInt(
            prefijoEvento + "Blackout",
            blackoutActivo ? 1 : 0);

        PlayerPrefs.SetInt(
            prefijoEvento + "Fase",
            faseEvento);

        PlayerPrefs.SetFloat(
            prefijoEvento + "TiempoFase",
            tiempoFase);

        PlayerPrefs.SetFloat(
            prefijoEvento + "ColorR",
            panelOverdrive.color.r);

        PlayerPrefs.SetFloat(
            prefijoEvento + "ColorG",
            panelOverdrive.color.g);

        PlayerPrefs.SetFloat(
            prefijoEvento + "ColorB",
            panelOverdrive.color.b);

        PlayerPrefs.SetFloat(
            prefijoEvento + "ColorA",
            panelOverdrive.color.a);

        Debug.Log("===== GUARDANDO EVENTO =====");
        Debug.Log("EventoActivo: " + eventoActivo);
        Debug.Log("Overdrive: " + overdriveActivo);
        Debug.Log("Blackout: " + blackoutActivo);
        Debug.Log("Fase: " + faseEvento);
        Debug.Log("TiempoFase: " + tiempoFase.ToString("F2"));
        Debug.Log("Color guardado: " + panelOverdrive.color);

        PlayerPrefs.Save();
    }

    public void RestaurarEstadoEvento()
    {
        eventoActivo =
            PlayerPrefs.GetInt(
                prefijoEvento + "EventoActivo", 0) == 1;

        overdriveActivo =
            PlayerPrefs.GetInt(
                prefijoEvento + "Overdrive", 0) == 1;

        blackoutActivo =
            PlayerPrefs.GetInt(
                prefijoEvento + "Blackout", 0) == 1;

        faseEvento =
            PlayerPrefs.GetInt(
                prefijoEvento + "Fase", 0);

        tiempoFase =
            PlayerPrefs.GetFloat(
                prefijoEvento + "TiempoFase", 0f);

        Color colorGuardado = new Color(
            PlayerPrefs.GetFloat(prefijoEvento + "ColorR", 1f),
            PlayerPrefs.GetFloat(prefijoEvento + "ColorG", 1f),
            PlayerPrefs.GetFloat(prefijoEvento + "ColorB", 1f),
            PlayerPrefs.GetFloat(prefijoEvento + "ColorA", 0f)
         );

        panelOverdrive.color = colorGuardado;

        Debug.Log("===== RESTAURANDO EVENTO =====");
        Debug.Log("EventoActivo: " + eventoActivo);
        Debug.Log("Overdrive: " + overdriveActivo);
        Debug.Log("Blackout: " + blackoutActivo);
        Debug.Log("Fase: " + faseEvento);
        Debug.Log("TiempoFase: " + tiempoFase.ToString("F2"));
        Debug.Log("Color restaurado: " + panelOverdrive.color);
    }

    public void ContinuarEvento()
    {
        if (!eventoActivo)
            return;

        if (rutinaEvento != null)
            StopCoroutine(rutinaEvento);

        if (overdriveActivo)
        {
            rutinaEvento = StartCoroutine(ContinuarOverdrive());
        }
        else if (blackoutActivo)
        {
            rutinaEvento = StartCoroutine(ContinuarBlackout());
        }
    }

    IEnumerator ContinuarOverdrive()
    {
        Debug.Log(">>> Continuando Overdrive");

        // El panel YA fue restaurado con su color exacto
        Color colorActual = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.3f);

        //---------------------------------------
        // FASE 1 : ENTRADA
        //---------------------------------------

        if (faseEvento == 1)
        {
            float tiempoRestante = tiempoEntrada - tiempoFase;

            float t = 0f;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                panelOverdrive.color =
                    Color.Lerp(
                        colorActual,
                        colorEvento,
                        t / tiempoRestante);

                yield return null;
            }

            panelOverdrive.color = colorEvento;

            faseEvento = 2;
            tiempoFase = 0f;
        }

        //---------------------------------------
        // FASE 2 : ACTIVO
        //---------------------------------------

        if (faseEvento == 2)
        {
            generadorDiscos.ActivarOverdrive(multiplicadorOverdrive);

            float tiempoRestante = tiempoActivo - tiempoFase;

            float t = 0f;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                yield return null;
            }

            generadorDiscos.DesactivarOverdrive();

            faseEvento = 3;
            tiempoFase = 0f;
            panelOverdrive.color = colorEvento;

            yield return null;
        }

        //---------------------------------------
        // FASE 3 : SALIDA
        //---------------------------------------
        Debug.Log("Entré a FASE 3");
        if (faseEvento == 3)
        {
            float tiempoRestante = tiempoSalida - tiempoFase;
            Debug.Log("Salida " + tiempoFase);
            float t = 0f;

            Color colorActualSalida = panelOverdrive.color;
            Color colorFinal = Color.white;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                panelOverdrive.color =
                    Color.Lerp(
                        colorActualSalida,
                        colorFinal,
                        t / tiempoRestante);

                yield return null;
            }

            panelOverdrive.color = Color.white;
        }

        //---------------------------------------
        // LIMPIEZA
        //---------------------------------------

        eventoActivo = false;
        overdriveActivo = false;

        faseEvento = 0;
        tiempoFase = 0f;

        rutinaEvento = null;

        Debug.Log(">>> Overdrive finalizado");
    }

    IEnumerator ContinuarBlackout()
    {
        Debug.Log(">>> Continuando Blackout");

        // El panel YA fue restaurado con su color exacto
        Color colorActual = panelOverdrive.color;
        Color colorEvento = new Color(1f, 0f, 0f, 0.8f);

        //---------------------------------------
        // FASE 1 : ENTRADA
        //---------------------------------------

        if (faseEvento == 1)
        {
            float tiempoRestante = tiempoEntrada - tiempoFase;

            float t = 0f;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                panelOverdrive.color =
                    Color.Lerp(
                        colorActual,
                        colorEvento,
                        t / tiempoRestante);

                yield return null;
            }

            panelOverdrive.color = colorEvento;

            faseEvento = 2;
            tiempoFase = 0f;
        }

        //---------------------------------------
        // FASE 2 : ACTIVO
        //---------------------------------------

        if (faseEvento == 2)
        {

            float tiempoRestante = tiempoActivo - tiempoFase;

            float t = 0f;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                yield return null;
            }

            faseEvento = 3;
            tiempoFase = 0f;
            panelOverdrive.color = colorEvento;

            yield return null;
        }

        //---------------------------------------
        // FASE 3 : SALIDA
        //---------------------------------------
        Debug.Log("Entré a FASE 3");
        if (faseEvento == 3)
        {
            float tiempoRestante = tiempoSalida - tiempoFase;
            Debug.Log("Salida " + tiempoFase);
            float t = 0f;

            Color colorActualSalida = panelOverdrive.color;
            Color colorFinal = Color.white;

            while (t < tiempoRestante)
            {
                t += Time.deltaTime;
                tiempoFase += Time.deltaTime;

                panelOverdrive.color =
                    Color.Lerp(
                        colorActualSalida,
                        colorFinal,
                        t / tiempoRestante);

                yield return null;
            }

            panelOverdrive.color = Color.white;
        }

        //---------------------------------------
        // LIMPIEZA
        //---------------------------------------

        eventoActivo = false;
        overdriveActivo = false;

        faseEvento = 0;
        tiempoFase = 0f;

        rutinaEvento = null;

        Debug.Log(">>> Blackout finalizado");
    }

    public void ReiniciarEvento()
    {
        //---------------------------------------
        // Detener corrutina si existe
        //---------------------------------------

        if (rutinaEvento != null)
        {
            StopCoroutine(rutinaEvento);
            rutinaEvento = null;
        }

        //---------------------------------------
        // Restaurar panel
        //---------------------------------------

        panelOverdrive.color = Color.white;

        //---------------------------------------
        // Restaurar discos
        //---------------------------------------

        generadorDiscos.DesactivarOverdrive();

        //---------------------------------------
        // Limpiar estados
        //---------------------------------------

        eventoActivo = false;
        overdriveActivo = false;
        blackoutActivo = false;

        faseEvento = 0;
        tiempoFase = 0f;
    }
}
