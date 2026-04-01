using UnityEngine;

/// <summary>
/// Punto láser individual con soporte de zonas y cambio de estado.
/// El estado (Lethal / Slow) puede ser sobreescrito por un ZoneComputer
/// y revierte automáticamente tras un timer gestionado por LaserRoomManager.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class LaserPoint : MonoBehaviour
{
    // ── Zona ──────────────────────────────────────────────────────────────
    [HideInInspector] public LaserZoneID zoneID;

    // ── Config (asignada por Manager) ─────────────────────────────────────
    [HideInInspector] public float cycleMin        = 0.8f;
    [HideInInspector] public float cycleMax        = 2.0f;
    [HideInInspector] public float damagePerSecond = 20f;
    [HideInInspector] public float slowFactor      = 0.4f;

    // ── Estado ────────────────────────────────────────────────────────────
    private LaserState _state = LaserState.Lethal;

    // ── Colores por estado ─────────────────────────────────────────────────
    private static readonly Color ColLethalIdle   = new Color(0.5f, 0f,    0f);
    private static readonly Color ColLethalWarn   = new Color(1f,   0.4f,  0f);
    private static readonly Color ColLethalDanger = new Color(1f,   0f,    0f);
    private static readonly Color ColSlowIdle     = new Color(0.4f, 0.25f, 0f);
    private static readonly Color ColSlowWarn     = new Color(1f,   0.55f, 0f);
    private static readonly Color ColSlowDanger   = new Color(1f,   0.75f, 0f);

    // ── Umbrales ──────────────────────────────────────────────────────────
    private const float WarnThreshold   = 0.60f;
    private const float DangerThreshold = 0.85f;
    private const float ScaleMin        = 0.05f;
    private const float ScaleMax        = 2.00f;

    // ── Pausa en escala 0 ─────────────────────────────────────────────────
    // Duración configurable desde el Manager vía SetConfig()
    [HideInInspector] public float restDuration = 1.5f;
    private bool  _resting;
    private float _restTimer;

    // ── Runtime ───────────────────────────────────────────────────────────
    private float          _timer;
    private float          _cycleDuration;
    private bool           _expanding = true;
    private bool           _isDangerous;

    private MeshRenderer   _rend;
    private Material       _mat;
    private SphereCollider _col;

    private bool           _playerInside;
/*     private PlayerHealth   _cachedHealth;
 */    
//  private PlayerMovement _cachedMovement;

    // ── Unity ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        _rend = GetComponent<MeshRenderer>();
        _col  = GetComponent<SphereCollider>();

        _mat = new Material(_rend.sharedMaterial);
        _rend.material = _mat;

        _col.isTrigger = true;
        _col.enabled   = false;

        _timer = Random.Range(0f, 1f);
        RandomizeCycle();
    }

    private void Update() => TickPulse();

    private void OnDestroy() { if (_mat != null) Destroy(_mat); }

    // ── Triggers ──────────────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside   = true;
/*         _cachedHealth   = other.GetComponent<PlayerHealth>();
 */        //_cachedMovement = other.GetComponent<PlayerMovement>();
    }

    private void OnTriggerExit(Collider other)
    {
       /*  if (!other.CompareTag("Player")) return;
        _playerInside = false;
        _cachedMovement?.RemoveSlow(this);
        _cachedHealth   = null;
        _cachedMovement = null; */
    }

    // ── Pulso ─────────────────────────────────────────────────────────────
    private void TickPulse()
    {
        // ── Fase de pausa (escala 0 mantenida) ────────────────────────────
        if (_resting)
        {
            _restTimer += Time.deltaTime;
            if (_restTimer >= restDuration)
            {
                _resting   = false;
                _restTimer = 0f;
                _expanding = true;
                RandomizeCycle();
            }
            // Durante la pausa: escala mínima, color idle, sin colisión
            ApplyScale(0f);
            ApplyColor(0f);
            UpdateDanger(0f);
            return;
        }

        // ── Fase de pulso normal ──────────────────────────────────────────
        _timer += Time.deltaTime / _cycleDuration;
        if (_timer >= 1f)
        {
            _timer = 0f;
            if (_expanding)
            {
                // Llegó a escala máxima → empieza a encoger
                _expanding = false;
            }
            else
            {
                // Llegó a escala 0 → entra en pausa
                _resting = true;
                return;
            }
        }

        float t = _expanding
            ? Mathf.SmoothStep(0f, 1f, _timer)
            : Mathf.SmoothStep(0f, 1f, 1f - _timer);

        ApplyScale(t);
        ApplyColor(t);
        UpdateDanger(t);

        if (_playerInside && _isDangerous) ApplyPlayerEffect();
    }

    private void ApplyScale(float t)
    {
        float s = Mathf.Lerp(ScaleMin, ScaleMax, t);
        transform.localScale = Vector3.one * s;
    }

    private void ApplyColor(float t)
    {
        Color idle, warn, danger;
        if (_state == LaserState.Lethal)
        { idle = ColLethalIdle; warn = ColLethalWarn; danger = ColLethalDanger; }
        else
        { idle = ColSlowIdle;   warn = ColSlowWarn;   danger = ColSlowDanger;   }

        Color c = t >= DangerThreshold ? danger
                : t >= WarnThreshold   ? warn
                :                        idle;

        _mat.color = c;
        _mat.SetColor("_EmissionColor", c * 2.5f);
    }

    private void UpdateDanger(float t)
    {
        bool should = t >= DangerThreshold;
        if (should == _isDangerous) return;
        _isDangerous = should;
        _col.enabled = _isDangerous;
    }

    private void ApplyPlayerEffect()
    {
        if (_state == LaserState.Lethal){}
            //_cachedHealth?.TakeDamage(damagePerSecond * Time.deltaTime);
        //else {}
            // _cachedMovement?.ApplySlow(this, slowFactor);
    }

    private void RandomizeCycle() => _cycleDuration = Random.Range(cycleMin, cycleMax);

    // ── API pública ───────────────────────────────────────────────────────
    public void SetConfig(float cMin, float cMax, float dps, float slow, LaserZoneID zone, float rest = 1.5f)
    {
        cycleMin = cMin; cycleMax = cMax;
        damagePerSecond = dps; slowFactor = slow;
        zoneID = zone; restDuration = rest;
    }

    public void SetState(LaserState newState) => _state = newState;
    public LaserState GetState()              => _state;
}
