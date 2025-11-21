using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Balloon
{
    public int level;
    public int pricesSumatories;
    public int upgradeCost;
    public float time;
    public float totalDays;
}

public class BalloonManager : MonoBehaviour
{
    private Dictionary< int, Balloon > _balloonLevelTable;
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

    public Balloon GetLevelData(int balloonLevel)
    {
        if (_balloonLevelTable.ContainsKey(balloonLevel))
            return _balloonLevelTable[balloonLevel];
        return null;
    }

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
