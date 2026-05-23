// BirdFollower.cs
// ─────────────────────────────────────────────────────────────────────────────
// Attach to the Player GameObject in the exploration scene.
// Birds wander freely around the player within a configurable radius,
// steering toward random targets, avoiding each other, and bobbing gently.
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

public class BirdFollower : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("References")]
    [Tooltip("Auto-found via 'Player' tag if left empty.")]
    [SerializeField] private Transform player;

    [Header("Appearance")]
    [Tooltip("Scale of each follower sprite. Reduce if birds appear too large.")]
    [SerializeField] private float birdScale = 0.25f;
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 10;
    [Tooltip("Enable if your bird sprite naturally faces RIGHT. " +
             "Disable if it naturally faces LEFT. Toggle this if birds always look backwards.")]
    [SerializeField] private bool defaultFacingRight = true;

    [Header("Wander Orbit")]
    [Tooltip("Minimum distance from player. Birds never get closer than this.")]
    [SerializeField] private float minRadius = 0.4f;
    [Tooltip("Maximum distance from player. Birds are pulled back beyond this.")]
    [SerializeField] private float maxRadius = 1.2f;
    [Tooltip("How many units above the player the wander zone is centered. " +
             "Increase this to push birds higher above the character's head.")]
    [SerializeField] private float verticalBias = 0.6f;
    [Tooltip("How often (seconds) each bird picks a new wander destination.")]
    [SerializeField] private float wanderInterval = 2.2f;
    [Tooltip("Random +/- seconds added to wanderInterval so birds don't sync up.")]
    [SerializeField] private float wanderVariance = 0.8f;

    [Header("Movement")]
    [Tooltip("Top speed of each bird (world units/sec).")]
    [SerializeField] private float maxSpeed = 2.0f;
    [Tooltip("Acceleration toward the current wander target.")]
    [SerializeField] private float steerStrength = 4.0f;
    [Tooltip("Extra pull force applied when a bird exceeds maxRadius.")]
    [SerializeField] private float pullStrength = 6.0f;
    [Tooltip("Drag applied each frame so birds decelerate naturally (0-1).")]
    [SerializeField][Range(0f, 1f)] private float drag = 0.97f;

    [Header("Separation")]
    [Tooltip("Birds push each other away when closer than this.")]
    [SerializeField] private float separationRadius = 0.35f;
    [SerializeField] private float separationStrength = 5.0f;

    [Header("Bob")]
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobFrequency = 1.4f;

    // ── Private State ─────────────────────────────────────────────────────────

    private const int BirdCount = 3;

    private GameObject[] _birdGOs = new GameObject[BirdCount];
    private SpriteRenderer[] _renderers = new SpriteRenderer[BirdCount];
    private Vector3[] _velocities = new Vector3[BirdCount];
    private Vector3[] _wanderTargets = new Vector3[BirdCount];
    private float[] _wanderTimers = new float[BirdCount];

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

        CreateFollowerObjects();
        RefreshFromRegistry();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Re-reads EquippedBirdsData and refreshes sprites/visibility.</summary>
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

    // ── Main Loop ─────────────────────────────────────────────────────────────

    private void Update()
    {
        if (player == null) return;

        for (int i = 0; i < BirdCount; i++)
        {
            if (!_birdGOs[i].activeSelf) continue;

            TickWanderTimer(i);
            ApplyForces(i);
            ApplyMovement(i);
        }
    }

    // ── Per-Bird Logic ────────────────────────────────────────────────────────

    private void TickWanderTimer(int i)
    {
        _wanderTimers[i] -= Time.deltaTime;
        if (_wanderTimers[i] <= 0f)
            PickNewWanderTarget(i);
    }

    private void PickNewWanderTarget(int i)
    {
        // Random point inside an annulus (ring) around the player.
        // Y is halved to respect the isometric perspective.
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minRadius, maxRadius);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius * 0.5f + verticalBias, // flatten for isometric + bias upward
            0f
        );

        _wanderTargets[i] = player.position + offset;
        _wanderTimers[i] = wanderInterval + Random.Range(-wanderVariance, wanderVariance);
    }

    private void ApplyForces(int i)
    {
        // Strip the bob offset to get the logical position for physics
        Vector3 pos = _birdGOs[i].transform.position;
        pos.y -= GetBob(i); // remove bob before force calculations

        // ── 1. Steer toward wander target ──
        Vector3 toTarget = _wanderTargets[i] - pos;
        _velocities[i] += toTarget.normalized * (steerStrength * Time.deltaTime);

        // ── 2. Pull back if beyond maxRadius ──
        Vector3 toPlayer = player.position - pos;
        float dist = toPlayer.magnitude;
        if (dist > maxRadius)
        {
            float excess = dist - maxRadius;
            _velocities[i] += toPlayer.normalized * (excess * pullStrength * Time.deltaTime);
        }

        // ── 3. Separation from other birds ──
        for (int j = 0; j < BirdCount; j++)
        {
            if (i == j || !_birdGOs[j].activeSelf) continue;

            Vector3 away = pos - _birdGOs[j].transform.position;
            float awayDist = away.magnitude;

            if (awayDist < separationRadius && awayDist > 0.001f)
            {
                float strength = separationStrength * (1f - awayDist / separationRadius);
                _velocities[i] += away.normalized * (strength * Time.deltaTime);
            }
        }

        // ── 4. Drag + speed clamp ──
        _velocities[i] *= drag;

        if (_velocities[i].magnitude > maxSpeed)
            _velocities[i] = _velocities[i].normalized * maxSpeed;

        _velocities[i].z = 0f;
    }

    private void ApplyMovement(int i)
    {
        // Advance logical position
        Vector3 pos = _birdGOs[i].transform.position;
        pos.y -= GetBob(i);          // strip old bob
        pos += _velocities[i] * Time.deltaTime;
        pos.z = 0f;

        // Re-apply fresh bob
        pos.y += GetBob(i);

        _birdGOs[i].transform.position = pos;

        // Flip sprite to face direction of travel.
        // defaultFacingRight=true  → flip when moving left  (velocity.x < 0)
        // defaultFacingRight=false → flip when moving right (velocity.x > 0)
        if (Mathf.Abs(_velocities[i].x) > 0.05f)
            _renderers[i].flipX = defaultFacingRight ? _velocities[i].x < 0f
                                                     : _velocities[i].x > 0f;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private float GetBob(int i)
    {
        float phase = i * (Mathf.PI * 2f / BirdCount);
        return Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f + phase) * bobAmplitude;
    }

    private void CreateFollowerObjects()
    {
        float angleStep = 360f / BirdCount;

        for (int i = 0; i < BirdCount; i++)
        {
            // Spread birds evenly around player at start
            float angle = i * angleStep * Mathf.Deg2Rad;
            float radius = (minRadius + maxRadius) * 0.5f;
            Vector3 startPos = player.position + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * 0.5f + verticalBias,
                0f
            );

            var go = new GameObject($"BirdFollower_{i + 1}");
            go.transform.position = startPos;
            go.transform.localScale = Vector3.one * birdScale;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder - i;

            go.SetActive(false);

            _birdGOs[i] = go;
            _renderers[i] = sr;
            _velocities[i] = Vector3.zero;
            _wanderTargets[i] = startPos;

            // Stagger timers so birds don't all pick new targets at the same time
            _wanderTimers[i] = Random.Range(0f, wanderInterval);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < BirdCount; i++)
            if (_birdGOs[i] != null) Destroy(_birdGOs[i]);
    }
}