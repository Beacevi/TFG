/**
 * @file PlayerManager.cs
 * @brief Administra el estado del jugador y proporciona acceso centralizado a sus datos de progreso.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modelo de datos que almacena las variables principales asociadas al jugador.
/// </summary>
[FirestoreData]
public class Player
{
    /// <summary>
    /// Propiedad que expone o modifica max balloon level.
    /// </summary>
    [FirestoreProperty] public int maxBalloonLevel { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica level.
    /// </summary>
    [FirestoreProperty] public int level { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica sum price.
    /// </summary>
    [FirestoreProperty] public int sumPrice { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica upgrade cost.
    /// </summary>
    [FirestoreProperty] public int upgradeCost { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica v level.
    /// </summary>
    [FirestoreProperty] public int VLevel { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica sc.
    /// </summary>
    [FirestoreProperty] public int SC { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica coins.
    /// </summary>
    [FirestoreProperty] public int coins { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica energy.
    /// </summary>
    [FirestoreProperty] public int energy { get; set; }
    /// <summary>
    /// Propiedad que expone o modifica gems.
    /// </summary>
    [FirestoreProperty] public int gems { get; set; }

    /// <summary>
    /// Reproduce o inicia er.
    /// </summary>
    public Player() { }
    
    /// <summary>
    /// Reproduce o inicia er.
    /// </summary>
    /// <param name="player">Referencia al jugador afectado por la operación.</param>
    public Player(Player player)
    {
        maxBalloonLevel = player.maxBalloonLevel;
        level = player.level;
        sumPrice = player.sumPrice;
        upgradeCost = player.upgradeCost;
        VLevel = player.VLevel;
        SC = player.SC;

        coins = player.coins;
        energy = player.energy;
        gems = player.gems;
    }

}
/// <summary>
/// Administra el estado del jugador y proporciona acceso centralizado a sus datos de progreso.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo PlayerManager utilizado para almacenar o configurar player instance.
    /// </summary>
    public static PlayerManager playerInstance;
    /// <summary>
    /// Colección de level table utilizada por este componente.
    /// </summary>
    public Dictionary<int, Player> levelTable;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        if (playerInstance != null && playerInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        playerInstance = this;
        DontDestroyOnLoad(gameObject);

        inicializarLevelTable();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a inicializar level table dentro de PlayerManager.
    /// </summary>
    private void inicializarLevelTable()
    {
        levelTable = new Dictionary<int, Player>
        {
            {  1, new Player { maxBalloonLevel =  1, level =  1, sumPrice =     25, upgradeCost =     25, VLevel = 1, SC =  100 } },//Cambiar SC a 50 otra vez
            {  2, new Player { maxBalloonLevel =  1, level =  2, sumPrice =     65, upgradeCost =     40, VLevel = 1, SC =  50 } },
            {  3, new Player { maxBalloonLevel =  2, level =  3, sumPrice =    130, upgradeCost =     65, VLevel = 1, SC =  50 } },
            {  4, new Player { maxBalloonLevel =  2, level =  4, sumPrice =    235, upgradeCost =    105, VLevel = 1, SC =  50 } },
            {  5, new Player { maxBalloonLevel =  3, level =  5, sumPrice =    405, upgradeCost =    170, VLevel = 1, SC =  50 } },
            {  6, new Player { maxBalloonLevel =  3, level =  6, sumPrice =    675, upgradeCost =    270, VLevel = 1, SC =  50 } },
            {  7, new Player { maxBalloonLevel =  4, level =  7, sumPrice =   1105, upgradeCost =    430, VLevel = 2, SC = 100 } },
            {  8, new Player { maxBalloonLevel =  4, level =  8, sumPrice =   1795, upgradeCost =    690, VLevel = 2, SC = 100 } },
            {  9, new Player { maxBalloonLevel =  5, level =  9, sumPrice =   2900, upgradeCost =   1105, VLevel = 3, SC = 150 } },
            { 10, new Player { maxBalloonLevel =  5, level = 10, sumPrice =   4670, upgradeCost =   1770, VLevel = 3, SC = 150 } },
            { 11, new Player { maxBalloonLevel =  6, level = 11, sumPrice =   7500, upgradeCost =   2830, VLevel = 4, SC = 200 } },
            { 12, new Player { maxBalloonLevel =  6, level = 12, sumPrice =  12030, upgradeCost =   4530, VLevel = 4, SC = 200 } },
            { 13, new Player { maxBalloonLevel =  7, level = 13, sumPrice =  19280, upgradeCost =   7250, VLevel = 5, SC = 300 } },
            { 14, new Player { maxBalloonLevel =  7, level = 14, sumPrice =  30880, upgradeCost =  11600, VLevel = 5, SC = 300 } },
            { 15, new Player { maxBalloonLevel =  8, level = 15, sumPrice =  49440, upgradeCost =  18560, VLevel = 6, SC = 600 } },
            { 16, new Player { maxBalloonLevel =  8, level = 16, sumPrice =  79135, upgradeCost =  29695, VLevel = 6, SC = 600 } },
            { 17, new Player { maxBalloonLevel =  9, level = 17, sumPrice = 126645, upgradeCost =  47510, VLevel = 7, SC = 900 } },
            { 18, new Player { maxBalloonLevel =  9, level = 18, sumPrice = 202660, upgradeCost =  76015, VLevel = 7, SC = 900 } },
            { 19, new Player { maxBalloonLevel = 10, level = 19, sumPrice = 324285, upgradeCost = 121625, VLevel = 7, SC = 900 } },
            { 20, new Player { maxBalloonLevel = 10, level = 20, sumPrice = 518885, upgradeCost = 194600, VLevel = 7, SC = 900 } },
        };
    }

}
