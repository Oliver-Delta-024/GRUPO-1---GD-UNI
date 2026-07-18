using UnityEngine;

public class Muro : MonoBehaviour
{
    [Header("Resistencia del muro")]
    public int resistenciaMaxima = 15;

    private int resistenciaActual;

    void Start()
    {
        resistenciaActual = resistenciaMaxima;
    }

    public void RecibirDanio(int dano)
    {
        resistenciaActual -= dano;

        Debug.Log(
            "Muro recibió " +
            dano +
            " de daño. Resistencia restante: " +
            resistenciaActual);

        if (resistenciaActual <= 0)
        {
            DestruirMuro();
        }
    }

    void DestruirMuro()
    {
        Debug.Log("El muro fue destruido.");

        Destroy(gameObject);
    }

    public int ObtenerResistencia()
    {
        return resistenciaActual;
    }

    public void RestaurarResistencia(int resistencia)
    {
        resistenciaActual = resistencia;
    }
}
