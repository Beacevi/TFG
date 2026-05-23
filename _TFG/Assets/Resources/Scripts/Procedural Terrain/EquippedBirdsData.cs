// EquippedBirdsData.cs
// ─────────────────────────────────────────────────────────────────────────────
// Static data bridge: stores the 3 equipped bird sprites/data so they are
// available in ANY scene without DontDestroyOnLoad.
// Set from BirdButton (main menu), read from BirdFollower (exploration scene).
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

public static class EquippedBirdsData
{
    private static readonly Bird[] _birds = new Bird[3];
    private static readonly Sprite[] _sprites = new Sprite[3];
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

    public static bool IsActive(int slot) => ValidSlot(slot) && _active[slot];
    public static Bird GetBird(int slot) => ValidSlot(slot) ? _birds[slot] : null;
    public static Sprite GetSprite(int slot) => ValidSlot(slot) ? _sprites[slot] : null;

    private static bool ValidSlot(int slot) => slot >= 0 && slot < 3;
}