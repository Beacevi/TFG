/**
 * @file BalloonManager.cs
 * @brief Gestiona la tabla de niveles del globo y las operaciones de mejora asociadas al progreso del jugador.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Modelo de datos que define los parámetros de progresión del globo aerostático.
/// </summary>
public class Balloon
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar level.
    /// </summary>
    public int level;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar prices sumatories.
    /// </summary>
    public int pricesSumatories;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar upgrade cost.
    /// </summary>
    public int upgradeCost;
    /// <summary>
    /// Tiempo o duración asociado a time.
    /// </summary>
    public float time;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar total days.
    /// </summary>
    public float totalDays;
}

/// <summary>
/// Gestiona la tabla de niveles del globo y las operaciones de mejora asociadas al progreso del jugador.
/// </summary>
public class BalloonManager : MonoBehaviour
{
    /// <summary>
    /// Colección de balloon level table utilizada por este componente.
    /// </summary>
    private Dictionary< int, Balloon > _balloonLevelTable;
    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        _balloonLevelTable = new Dictionary<int, Balloon>
        {
            {  1, new Balloon { level =  1, pricesSumatories =    100, upgradeCost =    100, time = .25f, totalDays = .25f} },
            {  2, new Balloon { level =  2, pricesSumatories =    350, upgradeCost =    250, time = .25f, totalDays =  .5f} },
            {  3, new Balloon { level =  3, pricesSumatories =    975, upgradeCost =    625, time =  .5f, totalDays =   1f} },
            {  4, new Balloon { level =  4, pricesSumatories =   2340, upgradeCost =   1565, time =   1f, totalDays =   2f} },
            {  5, new Balloon { level =  5, pricesSumatories =   6455, upgradeCost =   3915, time =   1f, totalDays =   3f} },
            {  6, new Balloon { level =  6, pricesSumatories =  16245, upgradeCost =   9790, time =   2f, totalDays =   5f} },
            {  7, new Balloon { level =  7, pricesSumatories =  40720, upgradeCost =  24475, time =   2f, totalDays =   7f} },
            {  8, new Balloon { level =  8, pricesSumatories = 101910, upgradeCost =  61190, time =   2f, totalDays =   9f} },
            {  9, new Balloon { level =  9, pricesSumatories = 254885, upgradeCost = 152975, time =   3f, totalDays =  12f} },
            { 10, new Balloon { level = 10, pricesSumatories = 637325, upgradeCost = 382440, time =   3f, totalDays =  15f} }
        };
    }

    /// <summary>
    /// Obtiene level data a partir del estado actual del sistema.
    /// </summary>
    /// <param name="balloonLevel">Nivel que se utilizará como referencia.</param>
    /// <returns>Instancia o valor de tipo Balloon resultante de la operación.</returns>
    public Balloon GetLevelData(int balloonLevel)
    {
        if (_balloonLevelTable.ContainsKey(balloonLevel))
            return _balloonLevelTable[balloonLevel];
        return null;
    }

    /// <summary>
    /// Establece o actualiza balloon level dentro del sistema.
    /// </summary>
    /// <param name="balloon">Parámetro balloon empleado por el método.</param>
    /// <param name="player">Referencia al jugador afectado por la operación.</param>
    public void SetBalloonLevel(Balloon balloon, Player player)
    {
        if (balloon.level < player.maxBalloonLevel)
        {
            int level = balloon.level++;

            if (_balloonLevelTable.ContainsKey(level))
            {
                int price = _balloonLevelTable[level].upgradeCost;

                if(player.coins >= price)
                {
                    player.coins -= price;

                    balloon.level++;
                }
            }
        }
    }
}
