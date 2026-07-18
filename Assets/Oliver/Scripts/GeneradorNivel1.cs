using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorNivel1 : MonoBehaviour
{
    [Header("Prefab del disco")]
    public GameObject discoNormalPrefab;

    [Header("Tiempo de generación")]
    public float tiempoMinimo = 0.5f;
    public float tiempoMaximo = 1.0f;

    [Header("Radio de aparición")]
    public float radioGeneracion = 0.5f;

    // Lista donde se almacenarán todos los discos creados
    public List<GameObject> discosActivos = new List<GameObject>();
    
    // Prefijo para diferenciar datos entre niveles
    private string prefijoGuardado = "Nivel1_";
    private Coroutine rutinaGeneracion;

    private bool overdriveActivo = false;
    private float multiplicadorActual = 1f;

    public void IniciarGeneracion()
    {
        if (rutinaGeneracion == null)
        {
            rutinaGeneracion = StartCoroutine(GenerarDiscos());
        }
    }

    public void DetenerGeneracion()
    {
        if (rutinaGeneracion != null)
        {
            StopCoroutine(rutinaGeneracion);
            rutinaGeneracion = null;
        }
    }

    IEnumerator GenerarDiscos()
    {
        while (true)
        {
            float espera = Random.Range(tiempoMinimo, tiempoMaximo);

            yield return new WaitForSeconds(espera);

            GenerarDisco();
        }
    }

    void GenerarDisco()
    {
        Vector2 posicionAleatoria =
            (Vector2)transform.position +
            Random.insideUnitCircle * radioGeneracion;

        GameObject nuevoDisco =
            Instantiate(discoNormalPrefab,
                        posicionAleatoria,
                        Quaternion.identity);

        // Obtener el script del disco
        Discos script = nuevoDisco.GetComponent<Discos>();

        // Inicializar el disco recién generado
        script.InicializarDisco(true);

        // Si el evento está activo, aplicar también al disco nuevo
        if (overdriveActivo)
        {
            script.ActivarOverdrive(multiplicadorActual);
        }

        // Guardarlo en la lista
        discosActivos.Add(nuevoDisco);
    }

    public void GuardarDiscos()
    {
        PlayerPrefs.SetInt(prefijoGuardado + "CantidadDiscos", discosActivos.Count);

        for (int i = 0; i < discosActivos.Count; i++)
        {
            Discos disco = discosActivos[i].GetComponent<Discos>();

            PlayerPrefs.SetInt(
                prefijoGuardado + "Tipo_" + i,
                (int)disco.ObtenerTipo());

            PlayerPrefs.SetFloat(
                prefijoGuardado + "PosX_" + i,
                disco.ObtenerPosicion().x);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "PosY_" + i,
                disco.ObtenerPosicion().y);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "DirX_" + i,
                disco.ObtenerDireccion().x);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "DirY_" + i,
                disco.ObtenerDireccion().y);
        }

        PlayerPrefs.Save();
    }

    public void RestaurarDiscos()
    {
        int cantidad =
            PlayerPrefs.GetInt(prefijoGuardado + "CantidadDiscos", 0);

        for (int i = 0; i < cantidad; i++)
        {
            int tipo =
                PlayerPrefs.GetInt(prefijoGuardado + "Tipo_" + i);

            GameObject prefab = discoNormalPrefab;

            switch ((Discos.TipoDisco)tipo)
            {
                case Discos.TipoDisco.Normal:
                    prefab = discoNormalPrefab;
                    break;
            }

            Vector2 posicion = new Vector2(
                PlayerPrefs.GetFloat(prefijoGuardado + "PosX_" + i),
                PlayerPrefs.GetFloat(prefijoGuardado + "PosY_" + i));

            Vector2 direccion = new Vector2(
                PlayerPrefs.GetFloat(prefijoGuardado + "DirX_" + i),
                PlayerPrefs.GetFloat(prefijoGuardado + "DirY_" + i));

            GameObject nuevoDisco =
                Instantiate(prefab, posicion, Quaternion.identity);

            Discos script = nuevoDisco.GetComponent<Discos>();

            script.RestaurarDisco(posicion, direccion, false);

            discosActivos.Add(nuevoDisco);
        }
    }

    public void ReanudarDiscos()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                Discos script = disco.GetComponent<Discos>();
                script.ReanudarMovimiento();
            }
        }
    }

    public void LimpiarDiscos()
    {
        foreach (GameObject disco in discosActivos)
        {
            Destroy(disco);
        }

        discosActivos.Clear();
    }

    public void ActivarOverdrive(float multiplicador)
    {
        overdriveActivo = true;
        multiplicadorActual = multiplicador;
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().ActivarOverdrive(multiplicador);
            }
        }
    }

    public void DesactivarOverdrive()
    {
        overdriveActivo = false;
        multiplicadorActual = 1f;

        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().DesactivarOverdrive();
            }
        }
    }

    public void MostrarOverdriveVisual()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().MostrarOverdriveVisual();
            }
        }
    }

    public void OcultarOverdriveVisual()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().OcultarOverdriveVisual();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
