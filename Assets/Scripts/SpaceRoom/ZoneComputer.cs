using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ordenador hackeable en la isla central.
/// Al interactuar, cambia el estado de los lásers de su zona asignada
/// de Lethal → Slow durante 'hackedDuration' segundos, luego revierte.
///
/// SETUP en Unity:
///   1. Coloca este script en el prefab del ordenador.
///   2. LaserRoomManager le asigna la zona y la lista de puntos vía Init().
///   3. El jugador llama Interact() al pulsar el botón de acción (tecla E / botón móvil).
/// </summary>
public class ZoneComputer : MonoBehaviour
{
    // ── Config ────────────────────────────────────────────────────────────
    [Header("Hack Settings")]
    [SerializeField] private float hackedDuration    = 12f;   // segundos en estado Slow
    [SerializeField] private float interactRange     = 2.5f;  // distancia máxima para interactuar
    [SerializeField] private float revertWarningTime = 3f;    // segundos antes de revertir que avisa

    [Header("Visual Feedback")]
    [SerializeField] private MeshRenderer screenRenderer;     // renderer de la pantalla del ordenador
    [SerializeField] private Color colorIdle    = new Color(0f,   0.6f, 0f);   // verde apagado
    [SerializeField] private Color colorHacked  = new Color(1f,   0.6f, 0f);   // naranja activo
    [SerializeField] private Color colorWarning = new Color(1f,   0.1f, 0.1f); // rojo — va a revertir

    [Header("Events")]
    public UnityEvent<LaserZoneID> onHackStart;    // (zoneID)
    public UnityEvent<LaserZoneID> onHackRevert;   // (zoneID)

    // ── Estado ────────────────────────────────────────────────────────────
    private LaserZoneID        _zone;
    private List<LaserPoint>   _zonePoints = new List<LaserPoint>();
    private bool               _isHacked;
    private Coroutine          _revertCoroutine;
    private Material           _screenMat;

    // ── Unity ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (screenRenderer != null)
        {
            _screenMat = new Material(screenRenderer.sharedMaterial);
            screenRenderer.material = _screenMat;
            SetScreenColor(colorIdle);
        }
    }

    private void OnDestroy()
    {
        if (_screenMat != null) Destroy(_screenMat);
    }

    // ── API pública ───────────────────────────────────────────────────────

    /// <summary>Llamado por LaserRoomManager al instanciar este ordenador.</summary>
    public void Init(LaserZoneID zone, List<LaserPoint> points)
    {
        _zone       = zone;
        _zonePoints = points;
    }

    /// <summary>
    /// Llamado por el PlayerInteraction cuando el jugador pulsa el botón de acción.
    /// Devuelve true si la interacción tuvo efecto.
    /// </summary>
    public bool Interact(Transform playerTransform)
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > interactRange) return false;
        if (_isHacked)            return false; // ya hackeado, esperar a que revierta

        HackZone();
        return true;
    }

    public bool IsHacked()         => _isHacked;
    public LaserZoneID GetZone()   => _zone;
    public float GetInteractRange()=> interactRange;

    // ── Lógica interna ────────────────────────────────────────────────────
    private void HackZone()
    {
        _isHacked = true;
        SetZoneState(LaserState.Slow);
        SetScreenColor(colorHacked);

        onHackStart?.Invoke(_zone);
        Debug.Log($"[ZoneComputer] Zona {_zone} hackeada → Slow durante {hackedDuration}s");

        if (_revertCoroutine != null) StopCoroutine(_revertCoroutine);
        _revertCoroutine = StartCoroutine(RevertAfterDelay());
    }

    private IEnumerator RevertAfterDelay()
    {
        // Esperar hasta el aviso
        yield return new WaitForSeconds(hackedDuration - revertWarningTime);

        // Aviso visual: parpadeo rojo
        SetScreenColor(colorWarning);
        float blinkTimer = 0f;
        bool  blinkOn    = true;
        while (blinkTimer < revertWarningTime)
        {
            blinkTimer += Time.deltaTime;
            blinkOn     = Mathf.FloorToInt(blinkTimer / 0.3f) % 2 == 0;
            SetScreenColor(blinkOn ? colorWarning : colorHacked);
            yield return null;
        }

        // Revertir
        _isHacked = false;
        SetZoneState(LaserState.Lethal);
        SetScreenColor(colorIdle);

        onHackRevert?.Invoke(_zone);
        Debug.Log($"[ZoneComputer] Zona {_zone} revertida → Lethal");
    }

    private void SetZoneState(LaserState state)
    {
        foreach (LaserPoint lp in _zonePoints)
            if (lp != null) lp.SetState(state);
    }

    private void SetScreenColor(Color c)
    {
        if (_screenMat == null) return;
        _screenMat.color = c;
        _screenMat.SetColor("_EmissionColor", c * 2f);
    }

    // ── Gizmo ─────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
