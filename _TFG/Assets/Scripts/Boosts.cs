using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boosts : MonoBehaviour
{
    public class BoostData
    {
        public string name;
        public float power;
        public string description;
    }

    private Dictionary<string[], BoostData> boostCombinations;

    private void Awake()
    {
        boostCombinations = new Dictionary<string[], BoostData>
        {
            { new[] { "A", "B", "C" }, new BoostData { name = "A+B+C", power = 1.5f, description = "Ataque triple"            } },
            { new[] { "D", "E", "F" }, new BoostData { name = "D+E+F", power = 2.0f, description = "Defensa total"            } },
            { new[] { "G", "H", "I" }, new BoostData { name = "G+H+I", power = 1.8f, description = "Velocidad máxima"         } },
            { new[] { "A", "D", "G" }, new BoostData { name = "A+D+G", power = 1.2f, description = "Combo vertical"           } },
            { new[] { "B", "E", "H" }, new BoostData { name = "B+E+H", power = 1.3f, description = "Combo central"            } },
            { new[] { "C", "F", "I" }, new BoostData { name = "C+F+I", power = 1.4f, description = "Combo diagonal derecha"   } },
            { new[] { "A", "E", "I" }, new BoostData { name = "A+E+I", power = 2.5f, description = "Combo diagonal principal" } },
            { new[] { "C", "E", "G" }, new BoostData { name = "C+E+G", power = 2.2f, description = "Combo diagonal inversa"   } }
        };
    }

    public BoostData GetBoostFromSelection(Queue<string> birdSelected)
    {
        var selectedSet = new HashSet<string>(birdSelected);

        foreach (var combo in boostCombinations)
        {
            if (combo.Key.All(selectedSet.Contains))
            {
                return combo.Value;
            }
        }

        return null;
    }
}
