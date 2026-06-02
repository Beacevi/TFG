/**
 * @file BirdFollower.cs
 * @brief Controla el comportamiento de seguimiento de las aves equipadas alrededor del jugador.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

// BirdFollower.cs
// ─────────────────────────────────────────────────────────────────────────────
// Attach to the Player GameObject in the exploration scene.
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla el comportamiento de seguimiento de las aves equipadas alrededor del jugador.
/// </summary>
public class BirdFollower : MonoBehaviour
{
    // ═════════════════════════════════════════════════════════════════════════
    // INSPECTOR
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Referencia al jugador o a su objeto asociado en la escena.
    /// </summary>
    [Header("References")]
    [Tooltip("Auto-found via 'Player' tag if left empty.")]
    [SerializeField] private Transform player;

    // ── Appearance ────────────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar bird scale.
    /// </summary>
    [Header("Appearance")]
    [SerializeField] private float birdScale = 0.25f;
    [Tooltip("Create a Sorting Layer called 'AlwaysOnTop' in Project Settings → Graphics → " +
             "Sorting Layers, move it to the very top of the list, then set this field to match.")]
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar sorting layer name.
    /// </summary>
    [SerializeField] private string sortingLayerName = "Default";
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar sorting order.
    /// </summary>
    [SerializeField] private int sortingOrder = 1000;
    /// <summary>
    /// Indica si default facing right está activo o habilitado.
    /// </summary>
    [Tooltip("Enable if your sprite naturally faces RIGHT. Disable if it faces LEFT.")]
    [SerializeField] private bool defaultFacingRight = true;

    // ── Wander ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Valor numérico que limita o define min radius.
    /// </summary>
    [Header("Wander Orbit")]
    [SerializeField] private float minRadius = 0.4f;
    /// <summary>
    /// Valor numérico que limita o define max radius.
    /// </summary>
    [SerializeField] private float maxRadius = 1.2f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar vertical bias.
    /// </summary>
    [Tooltip("Units above the player the wander zone is centered.")]
    [SerializeField] private float verticalBias = 0.6f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar wander interval.
    /// </summary>
    [SerializeField] private float wanderInterval = 2.2f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar wander variance.
    /// </summary>
    [SerializeField] private float wanderVariance = 0.8f;

    // ── Movement ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Valor numérico que limita o define max speed.
    /// </summary>
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 2.0f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar steer strength.
    /// </summary>
    [SerializeField] private float steerStrength = 4.0f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar pull strength.
    /// </summary>
    [SerializeField] private float pullStrength = 6.0f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar drag.
    /// </summary>
    [SerializeField][Range(0f, 1f)] private float drag = 0.97f;

    // ── Separation ────────────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar separation radius.
    /// </summary>
    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.35f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar separation strength.
    /// </summary>
    [SerializeField] private float separationStrength = 5.0f;

    // ── Bob ───────────────────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar bob amplitude.
    /// </summary>
    [Header("Bob")]
    [SerializeField] private float bobAmplitude = 0.08f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar bob frequency.
    /// </summary>
    [SerializeField] private float bobFrequency = 1.4f;

    // ── Find Event — Timing ───────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar find delay min.
    /// </summary>
    [Header("Find Event — Timing")]
    [SerializeField] private float findDelayMin = 20f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar find delay max.
    /// </summary>
    [SerializeField] private float findDelayMax = 60f;

    // ── Find Event — Animation ────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar dive amount.
    /// </summary>
    [Header("Find Event — Animation")]
    [Tooltip("How far below the wander zone the bird dives.")]
    [SerializeField] private float diveAmount = 0.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar dive speed.
    /// </summary>
    [SerializeField] private float diveSpeed = 2.5f;
    /// <summary>
    /// Tiempo o duración asociado a rummage time.
    /// </summary>
    [Tooltip("Seconds the bird shakes before the reward appears.")]
    [SerializeField] private float rummageTime = 2.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar rummage amount.
    /// </summary>
    [SerializeField] private float rummageAmount = 0.06f;

    // ── Find Event — Visual Cue ───────────────────────────────────────────────
    /// <summary>
    /// Color utilizado para representar found color.
    /// </summary>
    [Header("Find Event — Visual Cue")]
    [Tooltip("Color the bird turns when it has found something and is waiting to be collected.")]
    [SerializeField] private Color foundColor = new Color(1f, 0.85f, 0f, 1f);
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar pulse amount.
    /// </summary>
    [Tooltip("How far the pulse grows beyond the base scale (0.15 = 15% larger at peak).")]
    [SerializeField] private float pulseAmount = 0.15f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar pulse frequency.
    /// </summary>
    [Tooltip("Complete pulse cycles per second. Lower = slower, calmer pulse.")]
    [SerializeField] private float pulseFrequency = 1.2f;

    // ── Find Event — Reward ───────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar steps reward.
    /// </summary>
    [Header("Find Event — Reward")]
    [SerializeField] private int stepsReward = 5;
    [Tooltip("World-space radius around the bird that counts as a tap to collect. " +
             "Increase if tapping feels imprecise on mobile.")]
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar bird tap radius.
    /// </summary>
    [SerializeField] private float birdTapRadius = 0.4f;

    // ── Find Event — Notification ─────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo TMP_FontAsset utilizado para almacenar o configurar notif font.
    /// </summary>
    [Header("Find Event — Notification")]
    [Tooltip("Assign Roboto-Bold SDF (or any TMP font asset) from your project here.")]
    [SerializeField] private TMP_FontAsset notifFont;
    /// <summary>
    /// Campo de tipo Sprite utilizado para almacenar o configurar reward icon.
    /// </summary>
    [Tooltip("The icon sprite shown inside the notification bubble (e.g. a footstep or reward icon).")]
    [SerializeField] private Sprite rewardIcon;
    /// <summary>
    /// Color utilizado para representar notif bg color.
    /// </summary>
    [Tooltip("Dark background keeps text readable — change for a different look.")]
    [SerializeField] private Color notifBgColor = new Color(0.07f, 0.07f, 0.10f, 0.96f);
    /// <summary>
    /// Color utilizado para representar notif text color.
    /// </summary>
    [Tooltip("Main text color — white on the dark background gives maximum contrast.")]
    [SerializeField] private Color notifTextColor = Color.white;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar notif font size.
    /// </summary>
    [SerializeField] private float notifFontSize = 22f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar notif icon size.
    /// </summary>
    [Tooltip("Size of the icon inside the notification bubble (pixels).")]
    [SerializeField] private float notifIconSize = 44f;

    // ── SFX Placeholders ──────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sfx dive.
    /// </summary>
    [Header("SFX Placeholders")]
    [Tooltip("Plays when a bird begins diving toward the ground.")]
    [SerializeField] private AudioClip sfxDive;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sfx rummage.
    /// </summary>
    [Tooltip("Plays when a bird starts rummaging/shaking.")]
    [SerializeField] private AudioClip sfxRummage;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sfx found.
    /// </summary>
    [Tooltip("Plays when the reward is ready to be collected.")]
    [SerializeField] private AudioClip sfxFound;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sfx collect.
    /// </summary>
    [Tooltip("Plays when the player taps to collect the reward.")]
    [SerializeField] private AudioClip sfxCollect;

    // ── VFX Placeholders ──────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo ParticleSystem utilizado para almacenar o configurar vfx dive.
    /// </summary>
    [Header("VFX Placeholders")]
    [Tooltip("Particle prefab spawned when a bird starts diving.")]
    [SerializeField] private ParticleSystem vfxDive;
    /// <summary>
    /// Campo de tipo ParticleSystem utilizado para almacenar o configurar vfx rummage.
    /// </summary>
    [Tooltip("Particle prefab spawned when a bird starts rummaging.")]
    [SerializeField] private ParticleSystem vfxRummage;
    /// <summary>
    /// Campo de tipo ParticleSystem utilizado para almacenar o configurar vfx found.
    /// </summary>
    [Tooltip("Particle prefab spawned when the reward becomes available (bird found something).")]
    [SerializeField] private ParticleSystem vfxFound;
    /// <summary>
    /// Campo de tipo ParticleSystem utilizado para almacenar o configurar vfx collect.
    /// </summary>
    [Tooltip("Particle prefab spawned when the player collects the reward.")]
    [SerializeField] private ParticleSystem vfxCollect;

    // ── Testing ───────────────────────────────────────────────────────────────
    /// <summary>
    /// Campo de tipo KeyCode utilizado para almacenar o configurar test trigger key.
    /// </summary>
    [Header("Testing")]
    [Tooltip("Press this key in Play mode to immediately trigger a find on the next available bird.")]
    [SerializeField] private KeyCode testTriggerKey = KeyCode.F;

    // ═════════════════════════════════════════════════════════════════════════
    // PRIVATE STATE
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Valor numérico que limita o define bird count.
    /// </summary>
    private const int BirdCount = 3;

    // Wander
    /// <summary>
    /// Colección de bird g os utilizada por este componente.
    /// </summary>
    private GameObject[] _birdGOs = new GameObject[BirdCount];
    /// <summary>
    /// Colección de renderers utilizada por este componente.
    /// </summary>
    private SpriteRenderer[] _renderers = new SpriteRenderer[BirdCount];
    /// <summary>
    /// Colección de base scales utilizada por este componente.
    /// </summary>
    private Vector3[] _baseScales = new Vector3[BirdCount];
    /// <summary>
    /// Colección de velocities utilizada por este componente.
    /// </summary>
    private Vector3[] _velocities = new Vector3[BirdCount];
    /// <summary>
    /// Colección de wander targets utilizada por este componente.
    /// </summary>
    private Vector3[] _wanderTargets = new Vector3[BirdCount];
    /// <summary>
    /// Colección de wander timers utilizada por este componente.
    /// </summary>
    private float[] _wanderTimers = new float[BirdCount];

    // Find event
    /// <summary>
    /// Estados internos utilizados por el sistema de seguimiento de aves para localizar o acompañar al jugador.
    /// </summary>
    private enum FindState { Wandering, FlyingDown, Rummaging, ShowingReward, Done }
    /// <summary>
    /// Colección de find states utilizada por este componente.
    /// </summary>
    private FindState[] _findStates = new FindState[BirdCount];
    /// <summary>
    /// Colección de find timers utilizada por este componente.
    /// </summary>
    private float[] _findTimers = new float[BirdCount];
    /// <summary>
    /// Colección de rummage timers utilizada por este componente.
    /// </summary>
    private float[] _rummageTimers = new float[BirdCount];
    /// <summary>
    /// Colección de dive targets utilizada por este componente.
    /// </summary>
    private Vector3[] _diveTargets = new Vector3[BirdCount];
    /// <summary>
    /// Indica si find in progress está activo o habilitado.
    /// </summary>
    private bool _findInProgress;

    // Notification UI
    /// <summary>
    /// Campo de tipo Canvas utilizado para almacenar o configurar notif canvas.
    /// </summary>
    private Canvas _notifCanvas;
    /// <summary>
    /// Panel de interfaz asociado a notif panel.
    /// </summary>
    private RectTransform _notifPanel;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar notif text.
    /// </summary>
    private TextMeshProUGUI _notifText;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar notif icon.
    /// </summary>
    private Image _notifIcon;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar notif bird index.
    /// </summary>
    private int _notifBirdIndex = -1;

    // Misc
    /// <summary>
    /// Colección de bird names utilizada por este componente.
    /// </summary>
    private string[] _birdNames = new string[BirdCount];
    /// <summary>
    /// Campo de tipo TileAStar utilizado para almacenar o configurar tile a star.
    /// </summary>
    private TileAStar _tileAStar;
    /// <summary>
    /// Campo de tipo AudioSource utilizado para almacenar o configurar audio source.
    /// </summary>
    private AudioSource _audioSource;

    // ═════════════════════════════════════════════════════════════════════════
    // LIFECYCLE
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (player == null)
        {
            Debug.LogWarning("[BirdFollower] No Player found. Disabling.");
            enabled = false;
            return;
        }

        _tileAStar = FindObjectOfType<TileAStar>();
        if (_tileAStar == null)
            Debug.LogWarning("[BirdFollower] TileAStar not found — step rewards won't work.");

        // Use or add an AudioSource on this GameObject
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        CreateFollowerObjects();
        CreateNotificationUI();
        RefreshFromRegistry();
    }

    /// <summary>
    /// Libera referencias o recursos cuando el objeto va a destruirse.
    /// </summary>
    private void OnDestroy()
    {
        for (int i = 0; i < BirdCount; i++)
            if (_birdGOs[i] != null) Destroy(_birdGOs[i]);
        if (_notifCanvas != null)
            Destroy(_notifCanvas.gameObject);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // PUBLIC API
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ejecuta la lógica asociada a refresh from registry dentro de FindState.
    /// </summary>
    public void RefreshFromRegistry()
    {
        for (int i = 0; i < BirdCount; i++)
        {
            bool occupied = EquippedBirdsData.IsActive(i);
            _birdGOs[i].SetActive(occupied);
            
            if (occupied)
            {
                Bird b = EquippedBirdsData.GetBird(i);
                
                // 1. Asignar Sprite
                Sprite s = EquippedBirdsData.GetSprite(i);
                if (s != null) _renderers[i].sprite = s;

                // 2. NUEVA LÓGICA: Asignar el Animator Controller
                if (b != null)
                {
                    _birdNames[i] = !string.IsNullOrEmpty(b.name) ? b.name : $"Bird {i + 1}";
                    
                    var anim = _birdGOs[i].GetComponent<Animator>();
                    if (anim != null && b.animator != null)
                    {
                        anim.runtimeAnimatorController = b.animator;
                    }
                }
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // MAIN LOOP
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        if (player == null) return;

        HandleTestInput();
        CheckBirdTap();        // lets player tap the bird itself to collect
        UpdateNotifPosition();

        for (int i = 0; i < BirdCount; i++)
        {
            if (!_birdGOs[i].activeSelf) continue;

            switch (_findStates[i])
            {
                case FindState.Wandering:
                    TickFindTimer(i);
                    TickWanderTimer(i);
                    ApplyForces(i);
                    ApplyMovement(i);
                    break;

                case FindState.FlyingDown:
                    UpdateFlyDown(i);
                    break;

                case FindState.Rummaging:
                    UpdateRummage(i);
                    break;

                case FindState.ShowingReward:
                    UpdateShowingReward(i);
                    break;

                case FindState.Done:
                    TickWanderTimer(i);
                    ApplyForces(i);
                    ApplyMovement(i);
                    break;
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // FIND EVENT — STATE HANDLERS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ejecuta la lógica asociada a tick find timer dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void TickFindTimer(int i)
    {
        if (_findInProgress) return;
        _findTimers[i] -= Time.deltaTime;
        if (_findTimers[i] <= 0f)
            StartDive(i);
    }

    // ── State: Wandering → FlyingDown ─────────────────────────────────────────
    /// <summary>
    /// Ejecuta la lógica asociada a start dive dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void StartDive(int i)
    {
        _findInProgress = true;
        _findStates[i] = FindState.FlyingDown;
        _velocities[i] = Vector3.zero;

        _diveTargets[i] = player.position + new Vector3(
            Random.Range(-0.3f, 0.3f), -diveAmount, 0f);

        PlaySFX(sfxDive);
        SpawnVFX(vfxDive, _birdGOs[i].transform.position);

        Debug.Log($"[BirdFollower] Bird {i + 1} ({_birdNames[i]}) starting dive.");
    }

    // ── State: FlyingDown ─────────────────────────────────────────────────────
    // Uses Vector3.MoveTowards — guaranteed to reach target, no velocity overshoot.
    /// <summary>
    /// Actualiza fly down para reflejar el estado actual del sistema.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void UpdateFlyDown(int i)
    {
        Vector3 current = _birdGOs[i].transform.position;
        Vector3 target = _diveTargets[i];
        Vector3 next = Vector3.MoveTowards(current, target, diveSpeed * Time.deltaTime);
        _birdGOs[i].transform.position = next;

        float dx = target.x - current.x;
        if (Mathf.Abs(dx) > 0.01f)
            _renderers[i].flipX = defaultFacingRight ? dx < 0f : dx > 0f;

        if (Vector3.Distance(next, target) < 0.001f)
        {
            // ── Transition: FlyingDown → Rummaging ──
            _birdGOs[i].transform.position = target;
            _rummageTimers[i] = rummageTime;
            _findStates[i] = FindState.Rummaging;

            float faceDx = player.position.x - target.x;
            _renderers[i].flipX = defaultFacingRight ? faceDx < 0f : faceDx > 0f;

            PlaySFX(sfxRummage);
            SpawnVFX(vfxRummage, target);

            Debug.Log($"[BirdFollower] Bird {i + 1} rummaging for {rummageTime}s.");
        }
    }

    // ── State: Rummaging ──────────────────────────────────────────────────────
    /// <summary>
    /// Actualiza rummage para reflejar el estado actual del sistema.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void UpdateRummage(int i)
    {
        _rummageTimers[i] -= Time.deltaTime;

        _birdGOs[i].transform.position = _diveTargets[i] + new Vector3(
            Random.Range(-rummageAmount, rummageAmount),
            Random.Range(-rummageAmount * 0.4f, rummageAmount * 0.4f),
            0f);

        if (_rummageTimers[i] <= 0f)
        {
            // ── Transition: Rummaging → ShowingReward ──
            _birdGOs[i].transform.position = _diveTargets[i];
            _renderers[i].color = foundColor;
            _findStates[i] = FindState.ShowingReward;
            _notifBirdIndex = i;

            PlaySFX(sfxFound);
            SpawnVFX(vfxFound, _diveTargets[i]);
            ShowNotification(i);

            Debug.Log($"[BirdFollower] Bird {i + 1} found something! Waiting for collection.");
        }
    }

    // ── State: ShowingReward ──────────────────────────────────────────────────
    /// <summary>
    /// Actualiza showing reward para reflejar el estado actual del sistema.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void UpdateShowingReward(int i)
    {
        // Gentle hover
        float bob = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
        _birdGOs[i].transform.position = _diveTargets[i] + new Vector3(0f, bob, 0f);

        // Pulse scale — driven by pulseFrequency, which is now clearly Inspector-editable
        float pulse = 1f + pulseAmount * Mathf.Sin(Time.time * pulseFrequency * Mathf.PI * 2f);
        _birdGOs[i].transform.localScale = _baseScales[i] * pulse;
    }

    // ── Collect ───────────────────────────────────────────────────────────────
    // Called by both the UI button AND the bird tap handler.
    /// <summary>
    /// Ejecuta la lógica asociada a collect reward dentro de FindState.
    /// </summary>
    private void CollectReward()
    {
        if (_notifBirdIndex < 0) return;
        int i = _notifBirdIndex;

        if (_tileAStar != null)
            _tileAStar.AddSteps(stepsReward);

        // Reset bird appearance
        _renderers[i].color = Color.white;
        _birdGOs[i].transform.localScale = _baseScales[i];

        // Spawn collect VFX at the bird's current position
        PlaySFX(sfxCollect);
        SpawnVFX(vfxCollect, _birdGOs[i].transform.position);

        _notifCanvas.gameObject.SetActive(false);
        _notifBirdIndex = -1;

        _findStates[i] = FindState.Done;
        _findInProgress = false;
        PickNewWanderTarget(i);

        Debug.Log($"[BirdFollower] Reward collected from Bird {i + 1}. +{stepsReward} steps.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // BIRD TAP — lets the player tap directly on the bird to collect
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ejecuta la lógica asociada a check bird tap dentro de FindState.
    /// </summary>
    private void CheckBirdTap()
    {
        // Detect tap/click (works on mobile and in the editor)
        bool tapped = false;
        Vector3 tapScreenPos = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            tapped = true;
            tapScreenPos = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tapped = true;
            tapScreenPos = Input.GetTouch(0).position;
        }

        if (!tapped) return;

        if (Camera.main == null) return;
        Vector3 worldTap = Camera.main.ScreenToWorldPoint(tapScreenPos);
        worldTap.z = 0f;

        for (int i = 0; i < BirdCount; i++)
        {
            if (_findStates[i] != FindState.ShowingReward) continue;

            float dist = Vector2.Distance(worldTap, _birdGOs[i].transform.position);
            if (dist <= birdTapRadius)
            {
                CollectReward();
                // Block TileAStar movement so this tap doesn't also move the player
                TileAStar.BlockInputForSeconds(0.2f);
                return;
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // NOTIFICATION UI
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Muestra notification en la interfaz o en la escena.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void ShowNotification(int i)
    {
        // Assign the reward icon sprite if one is provided
        if (_notifIcon != null)
            _notifIcon.sprite = rewardIcon;

        // "+" text — kept simple and large so it reads at a glance
        _notifText.text = $"<b>+{stepsReward}</b>";

        _notifCanvas.gameObject.SetActive(true);
        UpdateNotifPosition();
    }

    /// <summary>
    /// Actualiza notif position para reflejar el estado actual del sistema.
    /// </summary>
    private void UpdateNotifPosition()
    {
        if (_notifBirdIndex < 0 || !_notifCanvas.gameObject.activeSelf) return;
        if (Camera.main == null) return;

        Vector3 worldPos = _birdGOs[_notifBirdIndex].transform.position + Vector3.up * 0.6f;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        _notifPanel.position = screenPos;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a create notification ui dentro de FindState.
    /// </summary>
    private void CreateNotificationUI()
    {
        // ── Canvas ──
        var canvasGO = new GameObject("BirdFindNotification");
        _notifCanvas = canvasGO.AddComponent<Canvas>();
        _notifCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _notifCanvas.sortingOrder = 200;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel / Button ──
        // Compact square-ish bubble: just big enough for the icon + "+" text side by side
        var panelGO = new GameObject("NotifButton");
        panelGO.transform.SetParent(canvasGO.transform, false);

        _notifPanel = panelGO.AddComponent<RectTransform>();
        _notifPanel.sizeDelta = new Vector2(notifIconSize + notifFontSize * 3f + 28f, notifIconSize + 16f);
        _notifPanel.pivot = new Vector2(0.5f, 0f); // bottom edge anchors at bird position

        var bg = panelGO.AddComponent<Image>();
        bg.color = notifBgColor;

        var btn = panelGO.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 0.75f);
        colors.pressedColor = new Color(0.75f, 1f, 0.75f);
        btn.colors = colors;
        btn.onClick.AddListener(CollectReward);

        // ── Icon image (left side) ──
        var iconGO = new GameObject("RewardIcon");
        iconGO.transform.SetParent(panelGO.transform, false);

        var iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0f, 0.5f);
        iconRT.anchorMax = new Vector2(0f, 0.5f);
        iconRT.pivot = new Vector2(0f, 0.5f);
        iconRT.anchoredPosition = new Vector2(10f, 0f);
        iconRT.sizeDelta = new Vector2(notifIconSize, notifIconSize);

        _notifIcon = iconGO.AddComponent<Image>();
        _notifIcon.sprite = rewardIcon;           // null-safe: Image shows nothing if sprite is null
        _notifIcon.preserveAspect = true;

        // ── "+" text (right of icon) ──
        var textGO = new GameObject("NotifText");
        textGO.transform.SetParent(panelGO.transform, false);

        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0f, 0f);
        textRT.anchorMax = new Vector2(1f, 1f);
        // Left offset leaves room for the icon; right/top/bottom add padding
        textRT.offsetMin = new Vector2(notifIconSize + 16f, 6f);
        textRT.offsetMax = new Vector2(-8f, -6f);

        _notifText = textGO.AddComponent<TextMeshProUGUI>();
        _notifText.fontSize = notifFontSize;
        _notifText.color = notifTextColor;
        _notifText.alignment = TextAlignmentOptions.MidlineLeft;
        _notifText.richText = true;
        _notifText.enableWordWrapping = false;

        if (notifFont != null)
            _notifText.font = notifFont;

        canvasGO.SetActive(false);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // TESTING
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ejecuta la lógica asociada a handle test input dentro de FindState.
    /// </summary>
    private void HandleTestInput()
    {
        if (!Input.GetKeyDown(testTriggerKey)) return;

        for (int i = 0; i < BirdCount; i++)
        {
            if (_birdGOs[i].activeSelf && _findStates[i] == FindState.Wandering && !_findInProgress)
            {
                _findTimers[i] = 0f;
                Debug.Log($"[BirdFollower TEST] Triggering find for Bird {i + 1} ({_birdNames[i]})");
                break;
            }
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // WANDER
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ejecuta la lógica asociada a tick wander timer dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void TickWanderTimer(int i)
    {
        _wanderTimers[i] -= Time.deltaTime;
        if (_wanderTimers[i] <= 0f) PickNewWanderTarget(i);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a pick new wander target dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void PickNewWanderTarget(int i)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minRadius, maxRadius);
        _wanderTargets[i] = player.position + new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius * 0.5f + verticalBias,
            0f);
        _wanderTimers[i] = wanderInterval + Random.Range(-wanderVariance, wanderVariance);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a apply forces dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void ApplyForces(int i)
    {
        Vector3 pos = _birdGOs[i].transform.position;
        pos.y -= GetBob(i);

        _velocities[i] += (_wanderTargets[i] - pos).normalized * (steerStrength * Time.deltaTime);

        Vector3 toPlayer = player.position - pos;
        float dist = toPlayer.magnitude;
        if (dist > maxRadius)
            _velocities[i] += toPlayer.normalized * ((dist - maxRadius) * pullStrength * Time.deltaTime);

        for (int j = 0; j < BirdCount; j++)
        {
            if (i == j || !_birdGOs[j].activeSelf) continue;
            Vector3 away = pos - _birdGOs[j].transform.position;
            float d = away.magnitude;
            if (d < separationRadius && d > 0.001f)
                _velocities[i] += away.normalized *
                    (separationStrength * (1f - d / separationRadius) * Time.deltaTime);
        }

        _velocities[i] *= drag;
        if (_velocities[i].magnitude > maxSpeed)
            _velocities[i] = _velocities[i].normalized * maxSpeed;
        _velocities[i].z = 0f;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a apply movement dentro de FindState.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    private void ApplyMovement(int i)
    {
        Vector3 pos = _birdGOs[i].transform.position;
        pos.y -= GetBob(i);
        pos += _velocities[i] * Time.deltaTime;
        pos.z = 0f;
        pos.y += GetBob(i);
        _birdGOs[i].transform.position = pos;

        if (Mathf.Abs(_velocities[i].x) > 0.05f)
            _renderers[i].flipX = defaultFacingRight
                ? _velocities[i].x < 0f
                : _velocities[i].x > 0f;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Obtiene bob a partir del estado actual del sistema.
    /// </summary>
    /// <param name="i">Parámetro i empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    private float GetBob(int i)
    {
        float phase = i * (Mathf.PI * 2f / BirdCount);
        return Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f + phase) * bobAmplitude;
    }

    /// <summary>
    /// Reproduce o inicia sfx.
    /// </summary>
    /// <param name="clip">Parámetro clip empleado por el método.</param>
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
            _audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Instantiates a particle prefab at the given world position and auto-destroys it.
    /// Assign the prefab in the Inspector — leave empty to skip spawning.
    /// </summary>
    private void SpawnVFX(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return;
        var ps = Instantiate(prefab, position, Quaternion.identity);
        var main = ps.main;
        Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a create follower objects dentro de FindState.
    /// </summary>
    private void CreateFollowerObjects()
        {
            float angleStep = 360f / BirdCount;
                for (int i = 0; i < BirdCount; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad;
                    float radius = (minRadius + maxRadius) * 0.5f;
                    Vector3 startPos = player.position + new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius * 0.5f + verticalBias,
                        0f);

                    var go = new GameObject($"BirdFollower_{i + 1}");
                    go.transform.position = startPos;
                    go.transform.localScale = Vector3.one * birdScale;

                    // 1. Añadimos el componente Animator
                    go.AddComponent<Animator>();

                    // 2. Añadimos el SpriteRenderer
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = sortingOrder;

                    go.SetActive(false);

                    _birdGOs[i] = go;
                    _renderers[i] = sr;
                    _baseScales[i] = go.transform.localScale;
                    _velocities[i] = Vector3.zero;
                    _wanderTargets[i] = startPos;
                    _wanderTimers[i] = Random.Range(0f, wanderInterval);
                    _findStates[i] = FindState.Wandering;
                    _findTimers[i] = Random.Range(findDelayMin, findDelayMax);
                }
        }
}
