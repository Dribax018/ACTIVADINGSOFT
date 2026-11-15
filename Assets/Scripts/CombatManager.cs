using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Asegúrate de tener TextMeshPro importado (viene por defecto en Unity)

[System.Serializable]
public class Combatiente
{
    public string nombre;
    public int hpMax;
    [HideInInspector] public int hpActual;
    public int ataqueMin;
    public int ataqueMax;

    public void Inicializar()
    {
        hpActual = hpMax;
    }

    public bool EstaVivo()
    {
        return hpActual > 0;
    }

    public void RecibirDano(int cantidad)
    {
        hpActual = Mathf.Max(0, hpActual - cantidad);
    }

    public int CalcularDano()
    {
        return Random.Range(ataqueMin, ataqueMax + 1);
    }

    // Genera una barra de vida en texto: [████░░░░] 40/100
    public string ObtenerBarraVida(int longitud = 10)
    {
        float porcentaje = (float)hpActual / hpMax;
        int llenos = Mathf.RoundToInt(longitud * porcentaje);
        int vacios = longitud - llenos;

        string barra = new string('█', llenos) + new string('░', vacios);
        return $"[{barra}] {hpActual}/{hpMax}";
    }
}

public class CombatePorTurnos : MonoBehaviour
{
    [Header("Configuración de Combatientes")]
    public Combatiente player = new Combatiente { nombre = "Héroe", hpMax = 100, ataqueMin = 15, ataqueMax = 25 };
    public List<Combatiente> listaInicialEnemigos = new List<Combatiente>
    {
        new Combatiente { nombre = "Goblin", hpMax = 40, ataqueMin = 5, ataqueMax = 10 },
        new Combatiente { nombre = "Orco", hpMax = 65, ataqueMin = 8, ataqueMax = 14 },
        new Combatiente { nombre = "Esqueleto", hpMax = 35, ataqueMin = 6, ataqueMax = 12 }
    };

    [Header("Referencias de UI (TextMeshPro)")]
    public TextMeshProUGUI textoPlayer;
    public TextMeshProUGUI textoColaEnemigos;
    public TextMeshProUGUI textoLogBatalla;
    public Button botonAtacar;

    private Queue<Combatiente> colaEnemigos = new Queue<Combatiente>();
    private bool turnoJugador = false;

    void Start()
    {
        IniciarCombate();
    }

    void IniciarCombate()
    {
        player.Inicializar();
        colaEnemigos.Clear();

        // Llenar la cola con los enemigos configurados en el Inspector
        foreach (Combatiente enemigo in listaInicialEnemigos)
        {
            enemigo.Inicializar();
            colaEnemigos.Enqueue(enemigo);
        }

        botonAtacar.onClick.AddListener(AlPresionarAtacar);

        ActualizarUI("¡Comienza el combate! Es tu turno.");
        turnoJugador = true;
        botonAtacar.interactable = true;
    }

    // Se ejecuta al hacer clic en el botón de UI
    public void AlPresionarAtacar()
    {
        if (!turnoJugador || colaEnemigos.Count == 0 || !player.EstaVivo()) return;

        StartCoroutine(SecuenciaTurnoCompleto());
    }

    IEnumerator SecuenciaTurnoCompleto()
    {
        // Bloquear botón mientras se resuelve el turno
        turnoJugador = false;
        botonAtacar.interactable = false;

        // --- 1. TURNO DEL JUGADOR ---
        // Peek() mira al primer enemigo de la cola sin sacarlo
        Combatiente objetivo = colaEnemigos.Peek();
        int danoPlayer = player.CalcularDano();
        objetivo.RecibirDano(danoPlayer);

        string mensaje = $"¡{player.nombre} golpea a {objetivo.nombre} por {danoPlayer} de daño!";

        if (!objetivo.EstaVivo())
        {
            mensaje += $"\n¡{objetivo.nombre} ha caído! El siguiente avanza a primera línea.";
            colaEnemigos.Dequeue(); // Eliminamos al enemigo derrotado
        }

        ActualizarUI(mensaje);
        yield return new WaitForSeconds(1.5f);

        // Comprobar victoria
        if (colaEnemigos.Count == 0)
        {
            ActualizarUI("¡VICTORIA! Has derrotado a todos los enemigos.");
            yield break;
        }

        // --- 2. TURNO DEL ENEMIGO EN PRIMERA LÍNEA ---
        Combatiente atacante = colaEnemigos.Peek();
        int danoEnemigo = atacante.CalcularDano();
        player.RecibirDano(danoEnemigo);

        ActualizarUI($"¡{atacante.nombre} ataca desde la primera línea por {danoEnemigo} de daño!");
        yield return new WaitForSeconds(1.5f);

        // Comprobar derrota
        if (!player.EstaVivo())
        {
            ActualizarUI("DERROTA... El héroe ha caído en combate.");
            yield break;
        }

        // --- 3. ROTACIÓN DE LA COLA (QUEUE) ---
        if (colaEnemigos.Count > 1)
        {
            // Sacamos al que acaba de atacar y lo mandamos al final de la fila
            Combatiente enemigoRotado = colaEnemigos.Dequeue();
            colaEnemigos.Enqueue(enemigoRotado);

            Combatiente nuevoFrente = colaEnemigos.Peek();
            ActualizarUI($"{enemigoRotado.nombre} retrocede al final. ¡{nuevoFrente.nombre} pasa a primera línea!");
            yield return new WaitForSeconds(1.2f);
        }

        // Devolver el control al jugador
        ActualizarUI($"Tu turno. {colaEnemigos.Peek().nombre} está frente a ti.");
        turnoJugador = true;
        botonAtacar.interactable = true;
    }

    void ActualizarUI(string eventoActual)
    {
        // Dibujar Jugador (Izquierda)
        textoPlayer.text = $"{player.ObtenerBarraVida()}\n<b>{player.nombre}</b>\n[ JUGADOR ]";

        // Dibujar Cola de Enemigos en horizontal (Derecha)
        string lineasBarras = "";
        string lineasNombres = "";
        string lineasSprites = "";

        int posicion = 0;
        foreach (Combatiente enemigo in colaEnemigos)
        {
            string indicador = (posicion == 0) ? ">> " : "   ";

            // Añadimos espacios para separarlos horizontalmente
            lineasBarras += $"{enemigo.ObtenerBarraVida()}    ";
            lineasNombres += $"{indicador}<b>{enemigo.nombre}</b>          ";
            lineasSprites += $"[ ENEMIGO ]        ";

            posicion++;
        }

        textoColaEnemigos.text = $"{lineasBarras}\n{lineasNombres}\n{lineasSprites}";
        textoLogBatalla.text = eventoActual;
    }
}
