/**
 * @file EquippedBirdsData.cs
 * @brief Mantiene la información de las aves equipadas por el jugador durante la exploración.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

// EquippedBirdsData.cs
// ─────────────────────────────────────────────────────────────────────────────
// Static data bridge: stores the 3 equipped bird sprites/data so they are
// available in ANY scene without DontDestroyOnLoad.
// Set from BirdButton (main menu), read from BirdFollower (exploration scene).
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

/// <summary>
/// Mantiene la información de las aves equipadas por el jugador durante la exploración.
/// </summary>
public static class EquippedBirdsData
{
    /// <summary>
    /// Colección de birds utilizada por este componente.
    /// </summary>
    private static readonly Bird[] _birds = new Bird[3];
    /// <summary>
    /// Colección de sprites utilizada por este componente.
    /// </summary>
    private static readonly Sprite[] _sprites = new Sprite[3];
    /// <summary>
    /// Colección de active utilizada por este componente.
    /// </summary>
    private static readonly bool[] _active = new bool[3];

    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>Register a bird in the given slot (0-2).</summary>
    public static void Set(int slot, Bird bird, Sprite sprite)
    {
        if (!ValidSlot(slot)) return;
        _birds[slot] = bird;
        _sprites[slot] = sprite;
        _active[slot] = true;
    }

    /// <summary>Clear a single slot.</summary>
    public static void Clear(int slot)
    {
        if (!ValidSlot(slot)) return;
        _birds[slot] = null;
        _sprites[slot] = null;
        _active[slot] = false;
    }

    /// <summary>Clear all three slots.</summary>
    public static void ClearAll()
    {
        for (int i = 0; i < 3; i++) Clear(i);
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Ejecuta la lógica asociada a is active dentro de EquippedBirdsData.
    /// </summary>
    /// <param name="slot">Parámetro slot empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public static bool IsActive(int slot) => ValidSlot(slot) && _active[slot];
    /// <summary>
    /// Obtiene bird a partir del estado actual del sistema.
    /// </summary>
    /// <param name="slot">Parámetro slot empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Bird resultante de la operación.</returns>
    public static Bird GetBird(int slot) => ValidSlot(slot) ? _birds[slot] : null;
    /// <summary>
    /// Obtiene sprite a partir del estado actual del sistema.
    /// </summary>
    /// <param name="slot">Parámetro slot empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Sprite resultante de la operación.</returns>
    public static Sprite GetSprite(int slot) => ValidSlot(slot) ? _sprites[slot] : null;

    /// <summary>
    /// Ejecuta la lógica asociada a valid slot dentro de EquippedBirdsData.
    /// </summary>
    /// <param name="slot">Parámetro slot empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    private static bool ValidSlot(int slot) => slot >= 0 && slot < 3;
}
