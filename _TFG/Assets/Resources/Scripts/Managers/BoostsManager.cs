using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boost
{
    public string name;
    public float power;
    public string description;
}

public class BoostsManager : MonoBehaviour
{
    public Boost GetBoostFromSelection(Queue<string> birdSelected)
    {
        if (birdSelected.Count != 3)
            return null;

        string a = birdSelected.ElementAt(0);
        string b = birdSelected.ElementAt(1);
        string c = birdSelected.ElementAt(2);

        // ADG → monedas
        if (a == "A" && b == "D" && c == "G")
            return new Boost { name = "Monedas", power = 1.2f, description = "Bonus de monedas" };

        // BEH → energía
        if (a == "B" && b == "E" && c == "H")
            return new Boost { name = "Energía", power = 1.3f, description = "Bonus de energía" };

        // CFI → ambos
        if (a == "C" && b == "F" && c == "I")
            return new Boost { name = "Monedas + Energía", power = 1.4f, description = "Bonus mixto" };

        return null;
    }
}
