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
        Pesado,
        Explosivo,
        Venenoso
    }

    public enum EstadoExplosivo
    {
        Normal,
        Explosion,
        PostExplosion,
        Destruido
    }

    [Header("Tipo de Disco")]
    public TipoDisco tipo;

    //==========================================
    // ESTAD�STICAS
    //==========================================

    [Header("Movimiento")]
    public float velocidadMovimiento;
    public float velocidadGiro;

    [Header("Combate")]
    public int danoContacto = 1;

    [Header("Explosivo")]
    public float tiempoExplosion = 5f;
    public float duracionExplosion = 0.4f;
    public float duracionPostExplosion = 0.4f;
    public float radioExplosion = 1.5f;

    [Header("Venenoso")]
    public float duracionVeneno = 3f;
    public float multiplicadorVeneno = 0.5f;

    //==========================================
    // VARIABLES
    //==========================================
    
    //Animator Variable
    private Animator animator;

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

    //==========================================
    // ESTADO DEL DISCO EXPLOSIVO
    //==========================================

    private EstadoExplosivo estadoExplosivo = EstadoExplosivo.Normal;

    private float temporizadorExplosion = 0f;
    private float tiempoEstado = 0f;

    private bool explosionDanioAplicado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (tipo == TipoDisco.Explosivo)
        {
            ActualizarExplosivo();
        }
    }

    //==========================================
    // CONFIGURAR SEG�N EL TIPO
    //==========================================

    void ConfigurarDisco()
    {
        switch (tipo)
        {
            case TipoDisco.Normal:

                velocidadMovimiento = 4f;
                velocidadGiro = 180f;

                danoContacto = 1;

                break;

            case TipoDisco.Rapido:

                velocidadMovimiento = 7f;
                velocidadGiro = 360f;

                danoContacto = 1;

                break;

            case TipoDisco.Pesado:

                velocidadMovimiento = 2.5f;
                velocidadGiro = 90f;

                danoContacto = 2;

                break;

            case TipoDisco.Explosivo:

                velocidadMovimiento = 3.5f;
                velocidadGiro = 180f;

                danoContacto = 1;

                break;

            case TipoDisco.Venenoso:

                velocidadMovimiento = 4f;
                velocidadGiro = 180f;

                danoContacto = 1;

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

    // Devuelve la posici�n actual
    public Vector2 ObtenerPosicion()
    {
        return transform.position;
    }

    // Devuelve la direcci�n actual
    public Vector2 ObtenerDireccion()
    {
        return direccion;
    }

    // Restaura la informaci�n del disco
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

        if (tipo == TipoDisco.Explosivo &&
        estadoExplosivo != EstadoExplosivo.Normal)
        {
            return;
        }

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
        if (tipo == TipoDisco.Explosivo &&
        estadoExplosivo != EstadoExplosivo.Normal)
        {
            return;
        }

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

    //==========================================
    // DATOS DEL EXPLOSIVO
    //==========================================

    public EstadoExplosivo ObtenerEstadoExplosivo()
    {
        return estadoExplosivo;
    }

    public float ObtenerTiempoEstado()
    {
        return tiempoEstado;
    }

    public float ObtenerTemporizadorExplosion()
    {
        return temporizadorExplosion;
    }

    public bool ObtenerDanioExplosionAplicado()
    {
        return explosionDanioAplicado;
    }

    public void RestaurarEstadoExplosivo(
    EstadoExplosivo estado,
    float temporizador,
    float tiempoFase,
    bool danoAplicado)
    {
        estadoExplosivo = estado;

        temporizadorExplosion = temporizador;

        tiempoEstado = tiempoFase;

        explosionDanioAplicado = danoAplicado;

        switch (estadoExplosivo)
        {
            //--------------------------
            // ETAPA 1
            //--------------------------

            case EstadoExplosivo.Normal:

                rb.linearVelocity = direccion * velocidadMovimiento;
                rb.angularVelocity = velocidadGiro;

                // Animator
                if (animator != null)
                {
                    animator.Play("Explosivo_Normal");
                }

                break;

            //--------------------------
            // ETAPA 2
            //--------------------------

            case EstadoExplosivo.Explosion:

                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                // Animator
                if (animator != null)
                {
                    animator.Play("Explosivo_Explosion");
                }

                break;

            //--------------------------
            // ETAPA 3
            //--------------------------

            case EstadoExplosivo.PostExplosion:

                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                // Animator
                if (animator != null)
                {
                    animator.Play("Explosivo_PostExplosion");
                }

                break;

            //--------------------------
            // ETAPA 4
            //--------------------------

            case EstadoExplosivo.Destruido:

                Destroy(gameObject);

                break;
        }
    }

    //==========================================
    // COMBATE
    //==========================================

    void InfligirDanio(GameObject jugador)
    {
        if (jugador == null)
            return;

        Player player = jugador.GetComponent<Player>();

        if (player == null)
            return;

        if (player.TieneInmunidad())
            return;

        player.RecibirDanio(danoContacto);
    }

    void InfligirVeneno(GameObject jugador)
    {
        if (jugador == null)
            return;

        Player player = jugador.GetComponent<Player>();

        if (player == null)
            return;

        player.AplicarVeneno(
            duracionVeneno,
            multiplicadorVeneno);
    }

    void AplicarDanioExplosion()
    {
        if (explosionDanioAplicado)
            return;

        explosionDanioAplicado = true;

        Collider2D[] objetos =
            Physics2D.OverlapCircleAll(
                transform.position,
                radioExplosion);

        foreach (Collider2D objeto in objetos)
        {
            if (objeto.CompareTag("Player"))
            {
                InfligirDanio(objeto.gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        switch (tipo)
        {
            case TipoDisco.Normal:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Rapido:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Pesado:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Venenoso:

                InfligirDanio(collision.gameObject);
                InfligirVeneno(collision.gameObject);

                break;

            case TipoDisco.Explosivo:

                // Solo hace da�o por contacto
                // mientras est� en la etapa Normal.

                if (estadoExplosivo == EstadoExplosivo.Normal)
                {
                    InfligirDanio(collision.gameObject);
                }

                break;
        }
    }
    //==========================================
    // EXPLOSIVO
    //==========================================

    void ActualizarExplosivo()
    {
        switch (estadoExplosivo)
        {
            //----------------------------
            // ETAPA 1
            //----------------------------

            case EstadoExplosivo.Normal:

                temporizadorExplosion += Time.deltaTime;

                if (temporizadorExplosion >= tiempoExplosion)
                {
                    IniciarExplosion();
                }

                break;

            //----------------------------
            // ETAPA 2
            //----------------------------

            case EstadoExplosivo.Explosion:

                tiempoEstado += Time.deltaTime;

                AplicarDanioExplosion();

                if (tiempoEstado >= duracionExplosion)
                {
                    estadoExplosivo = EstadoExplosivo.PostExplosion;
                    tiempoEstado = 0f;
                }

                break;

            //----------------------------
            // ETAPA 3
            //----------------------------

            case EstadoExplosivo.PostExplosion:

                tiempoEstado += Time.deltaTime;

                if (tiempoEstado >= duracionPostExplosion)
                {
                    FinalizarExplosion();
                }

                break;

            //----------------------------
            // ETAPA 4
            //----------------------------

            case EstadoExplosivo.Destruido:

                Destroy(gameObject);

                break;
        }
    }

    void IniciarExplosion()
    {
        estadoExplosivo = EstadoExplosivo.Explosion;

        tiempoEstado = 0f;

        explosionDanioAplicado = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Animator.SetTrigger("Explosion");
    }

    void FinalizarExplosion()
    {
        estadoExplosivo = EstadoExplosivo.Destruido;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Animator.SetTrigger("PostExplosion");
    }
}
