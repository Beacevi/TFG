/**
 * @file BoostsManager.cs
 * @brief Administra los potenciadores del juego y su efecto sobre economía, energía u otras variables de progreso.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Modelo de datos que describe un beneficio temporal o pasivo aplicado al jugador.
/// </summary>
public class Boost
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar name.
    /// </summary>
    public string name;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar coin multiplier.
    /// </summary>
    public float coinMultiplier;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar energy multiplier.
    /// </summary>
    public float energyMultiplier;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar description.
    /// </summary>
    public string description;
}

/// <summary>
/// Administra los potenciadores del juego y su efecto sobre economía, energía u otras variables de progreso.
/// </summary>
public class BoostsManager : MonoBehaviour
{
    /// <summary>
    /// Colección de boost combinations utilizada por este componente.
    /// </summary>
    private Dictionary<string, Boost> boostCombinations;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        boostCombinations = new Dictionary<string, Boost>
        {
            // Columnas verticales
            { "ADG", new Boost { name = "ADG", coinMultiplier = 1.5f, energyMultiplier = 0f, description = "Bonus de monedas" } },
            { "BEH", new Boost { name = "BEH", coinMultiplier = 0f, energyMultiplier = 1.5f, description = "Bonus de energía" } },
            { "CFI", new Boost { name = "CFI", coinMultiplier = 1.2f, energyMultiplier = 1.2f, description = "Bonus mixto" } },

            // Filas horizontales
            { "ABC", new Boost { name = "ABC", coinMultiplier = 1.5f, energyMultiplier = 0f, description = "Bonus de monedas" } },
            { "DEF", new Boost { name = "DEF", coinMultiplier = 0f, energyMultiplier = 1.5f, description = "Bonus de energía" } },
            { "GHI", new Boost { name = "GHI", coinMultiplier = 1.2f, energyMultiplier = 1.2f, description = "Bonus mixto" } },
        };
    }
    /// <summary>
    /// Obtiene boost from selection a partir del estado actual del sistema.
    /// </summary>
    /// <param name="birdSelected">Parámetro bird selected empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Boost resultante de la operación.</returns>
    public Boost GetBoostFromSelection(Queue<string> birdSelected)
    {
        if (birdSelected.Count != 3)
        {
            return null;
        }

        var combo = string.Concat(birdSelected.OrderBy(x => x));

        if (boostCombinations.TryGetValue(combo, out Boost boost))
        {
            ActivarBuffo(boost.name);
            Debug.Log("Boost seleccionado: "+combo);
            return boost;
        }

        return null;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a activar buffo dentro de BoostsManager.
    /// </summary>
    /// <param name="nombreBuffo">Parámetro nombre buffo empleado por el método.</param>
    private void ActivarBuffo(string nombreBuffo)
    {
        switch (nombreBuffo)
        {
            case "ADG":
            case "ABC":
                GameManager.Instance.buffMoneyActive = true;
                break;
            case "BEH":
            case "DEF":
                GameManager.Instance.buffEnergyActive = true;
                break;
            case "CFI":
            case "GHI":
                GameManager.Instance.buffBothActive = true;
                break;
        }
    }

}
