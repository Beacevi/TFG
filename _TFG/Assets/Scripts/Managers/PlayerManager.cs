using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class Player
{
    [FirestoreProperty] public int maxBalloonLevel { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int sumPrice { get; set; }
    [FirestoreProperty] public int upgradeCost { get; set; }
    [FirestoreProperty] public int VLevel { get; set; }
    [FirestoreProperty] public int SC { get; set; }
    [FirestoreProperty] public int coins { get; set; }
    [FirestoreProperty] public int energy { get; set; }
    [FirestoreProperty] public int gems { get; set; }

    public Player() { }
    
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
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager playerInstance;
    public Dictionary<int, Player> levelTable;

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
