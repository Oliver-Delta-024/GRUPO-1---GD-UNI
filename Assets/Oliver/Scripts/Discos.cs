using UnityEngine;
using static Discos;

public class Discos : MonoBehaviour
{
    //==========================================
    // TIPOS DE DISCO
    //==========================================

    public enum TipoDisco
    {
        Normal,
        Rapido,
        Pesado
    }

    [Header("Tipo de Disco")]
    public TipoDisco tipo;

    //==========================================
    // ESTADÍSTICAS
    //==========================================

    [Header("Movimiento")]
    public float velocidadMovimiento;
    public float velocidadGiro;

    //==========================================
    // VARIABLES
    //==========================================

    private Rigidbody2D rb;
    private Vector2 direccion;
    public void InicializarDisco(bool movimientoInicial) 
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        ConfigurarDisco();

        direccion = Random.insideUnitCircle.normalized;

        if (movimientoInicial)
        {
            rb.linearVelocity = direccion * velocidadMovimiento;
        }

        rb.angularVelocity = velocidadGiro;
    }

    private SpriteRenderer sprite;
    private bool overdriveActivo = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //==========================================
    // CONFIGURAR SEGÚN EL TIPO
    //==========================================

    void ConfigurarDisco()
    {
        switch (tipo)
        {
            case TipoDisco.Normal:

                velocidadMovimiento = 4f;
                velocidadGiro = 180f;

                break;

            case TipoDisco.Rapido:

                velocidadMovimiento = 7f;
                velocidadGiro = 360f;

                break;

            case TipoDisco.Pesado:

                velocidadMovimiento = 2.5f;
                velocidadGiro = 90f;

                break;
        }
    }

    //==============================
    // DATOS PARA EL GUARDADO
    //==============================

    // Devuelve el tipo de disco
    public TipoDisco ObtenerTipo()
    {
        return tipo;
    }

    // Devuelve la posición actual
    public Vector2 ObtenerPosicion()
    {
        return transform.position;
    }

    // Devuelve la dirección actual
    public Vector2 ObtenerDireccion()
    {
        return direccion;
    }

    // Restaura la información del disco
    public void RestaurarDisco(Vector2 posicion, Vector2 nuevaDireccion, bool iniciarMovimiento)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        ConfigurarDisco();
        transform.position = posicion;
        direccion = nuevaDireccion;

        if (iniciarMovimiento)
        {
            rb.linearVelocity = direccion * velocidadMovimiento;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void ReanudarMovimiento()
    {
        rb.linearVelocity = direccion * velocidadMovimiento;
    }

    public void ActivarOverdrive(float multiplicador)
    {

        if (overdriveActivo)
            return;

        overdriveActivo = true;

        rb.linearVelocity *= multiplicador;

        Color c = sprite.color;
        c.r = 1f;
        c.g = 0.65f;
        c.b = 0.65f;

        sprite.color = c;
    }

    public void DesactivarOverdrive()
    {
        if (!overdriveActivo)
            return;

        overdriveActivo = false;

        rb.linearVelocity = direccion * velocidadMovimiento;

        sprite.color = Color.white;
    }

    public void MostrarOverdriveVisual()
    {
        Color c = sprite.color;
        c.r = 1f;
        c.g = 0.65f;
        c.b = 0.65f;

        sprite.color = c;
    }

    public void OcultarOverdriveVisual()
    {
        sprite.color = Color.white;
    }
}
