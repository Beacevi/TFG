using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boost
{
    public string name;
    public float coinMultiplier;
    public float energyMultiplier;
    public string description;
}

public class BoostsManager : MonoBehaviour
{
    private Dictionary<string, Boost> boostCombinations;

    private void Awake()
    {
        boostCombinations = new Dictionary<string, Boost>
        {
            { "ADG", new Boost { name = "ADG", coinMultiplier = 1.5f, energyMultiplier = 0f, description = "Bonus de monedas" } },
            { "BEH", new Boost { name = "BEH", coinMultiplier = 0f, energyMultiplier = 1.5f, description = "Bonus de energía" } },
            { "CFI", new Boost { name = "CFI", coinMultiplier = 1.2f, energyMultiplier = 1.2f, description = "Bonus mixto" } }
        };
    }

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

    private void ActivarBuffo(string nombreBuffo)
    {
        switch (nombreBuffo)
        {
            case "ADG":
                GameManager.Instance.buffMoneyActive = true;
                break;
            case "BEH":
                GameManager.Instance.buffEnergyActive = true;
                break;
            case "CFI":
                GameManager.Instance.buffBothActive = true;
                break;
            default:
            break;
        }
    }

}