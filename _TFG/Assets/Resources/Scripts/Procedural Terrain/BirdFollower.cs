// BirdFollower.cs
// ─────────────────────────────────────────────────────────────────────────────
// Attach this script to the PLAYER GameObject in the exploration scene.
//
// It reads EquippedBirdsData (filled by BirdButton in the main menu) and
// spawns up to 3 follower-bird sprites that trail the player using a
// position-history queue — each bird follows the path the player already
// walked, creating a natural chained-trail effect.
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using UnityEngine;

public class BirdFollower : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("References")]
    [Tooltip("The player Transform to follow. Auto-found via 'Player' tag if left empty.")]
    [SerializeField] private Transform player;

    [Header("Trail Settings")]
    [Tooltip("How often (seconds) the player's position is recorded into history.")]
    [SerializeField] private float recordInterval = 0.08f;

    [Tooltip("How many recorded steps behind each successive bird sits. " +
             "Bird 1 = delay*1, Bird 2 = delay*2, Bird 3 = delay*3.")]
    [SerializeField] private int stepsPerBird = 6;

    [Tooltip("How fast each follower moves toward its target position.")]
    [SerializeField] private float followSpeed = 6f;

    [Header("Appearance")]
    [Tooltip("Scale applied to each follower sprite. Tune to match your tile size.")]
    [SerializeField] private float birdScale = 0.45f;

    [Tooltip("Sorting layer name used for follower sprites.")]
    [SerializeField] private string sortingLayerName = "Default";

    [Tooltip("Sorting order for follower sprites (should render above tiles).")]
    [SerializeField] private int sortingOrder = 10;

    [Tooltip("Vertical offset so birds float slightly above the ground plane.")]
    [SerializeField] private float heightOffset = 0.35f;

    [Header("Bob Animation")]
    [Tooltip("Vertical bob amplitude (world units).")]
    [SerializeField] private float bobAmplitude = 0.08f;

    [Tooltip("Bob speed (cycles per second).")]
    [SerializeField] private float bobFrequency = 2.5f;

    // ── Private State ─────────────────────────────────────────────────────────

    private const int MaxHistory = 120;   // Enough for any reasonable stepsPerBird*3
    private const int BirdCount = 3;

    private GameObject[] _birdGOs = new GameObject[BirdCount];
    private SpriteRenderer[] _renderers = new SpriteRenderer[BirdCount];

    private List<Vector3> _posHistory = new List<Vector3>(MaxHistory + 4);
    private float _recordTimer;

    // ─────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("[BirdFollower] No Player transform found. Disabling.");
            enabled = false;
            return;
        }

        // Pre-fill history so birds don't snap from world origin on first frame
        for (int i = 0; i < MaxHistory; i++)
            _posHistory.Add(player.position);

        CreateFollowerObjects();
        RefreshFromRegistry();         // Read EquippedBirdsData right away
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Called every frame — advance the trail and move followers
    // ─────────────────────────────────────────────────────────────────────────

    private void Update()
    {
        RecordPlayerPosition();
        MoveFollowers();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public API — call this from BirdButton whenever the lineup changes so
    // that the followers update immediately if the scene is already loaded.
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Re-reads EquippedBirdsData and updates follower sprites / visibility.
    /// Safe to call at any time (Start, or whenever the player equips birds).
    /// </summary>
    public void RefreshFromRegistry()
    {
        for (int i = 0; i < BirdCount; i++)
        {
            bool occupied = EquippedBirdsData.IsActive(i);
            _birdGOs[i].SetActive(occupied);

            if (occupied)
            {
                Sprite s = EquippedBirdsData.GetSprite(i);
                if (s != null) _renderers[i].sprite = s;
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    private void CreateFollowerObjects()
    {
        for (int i = 0; i < BirdCount; i++)
        {
            var go = new GameObject($"BirdFollower_{i + 1}");
            go.transform.position = player.position;
            go.transform.localScale = Vector3.one * birdScale;
            // NOTE: not parented to player — it moves independently via script

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder - i; // Bird 1 renders in front

            go.SetActive(false);

            _birdGOs[i] = go;
            _renderers[i] = sr;
        }
    }

    private void RecordPlayerPosition()
    {
        _recordTimer += Time.deltaTime;
        if (_recordTimer < recordInterval) return;
        _recordTimer = 0f;

        _posHistory.Insert(0, player.position);

        if (_posHistory.Count > MaxHistory)
            _posHistory.RemoveAt(_posHistory.Count - 1);
    }

    private void MoveFollowers()
    {
        // Horizontal direction from recent history — used for sprite flipping
        float recentDx = 0f;
        if (_posHistory.Count > 4)
            recentDx = _posHistory[0].x - _posHistory[4].x;

        for (int i = 0; i < BirdCount; i++)
        {
            if (!_birdGOs[i].activeSelf) continue;

            // Each bird targets a different point in the history trail
            int histIndex = Mathf.Min((i + 1) * stepsPerBird, _posHistory.Count - 1);
            Vector3 target = _posHistory[histIndex];
            target.y += heightOffset;

            // Gentle bob per bird (phase-shifted so they don't all bob together)
            float phase = i * (Mathf.PI * 2f / BirdCount);
            target.y += Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f + phase) * bobAmplitude;

            // Smooth movement
            _birdGOs[i].transform.position = Vector3.Lerp(
                _birdGOs[i].transform.position,
                target,
                followSpeed * Time.deltaTime
            );

            // Flip to face direction of travel
            if (Mathf.Abs(recentDx) > 0.01f)
                _renderers[i].flipX = recentDx < 0f;
        }
    }

    private void OnDestroy()
    {
        // Clean up spawned GameObjects when the player is destroyed
        for (int i = 0; i < BirdCount; i++)
        {
            if (_birdGOs[i] != null)
                Destroy(_birdGOs[i]);
        }
    }
}