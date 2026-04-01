// ============================================================================
// DOOR - Puerta individual en el mapa
// Cosmic Crew - Sistema de Zonas Event-Driven
// ============================================================================

using UnityEngine;

/// <summary>
/// Puerta individual en el mapa
/// Puede conectar dos zonas
/// Genera eventos cuando se abre/cierra
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField] private DoorConfig config;
    
    private DoorState currentState = DoorState.Closed;
    private float stateTimer = 0f;
    
    private Animator doorAnimator;  // Si tiene animación
    private Collider doorCollider;
    
    private ZoneManager zoneManager;
    
    // ────────────────────────────────────────────────────────
    // EVENTOS - Se disparan cuando puerta cambia estado
    // ────────────────────────────────────────────────────────
    
    public System.Action<Door> OnDoorOpened;
    public System.Action<Door> OnDoorClosed;
    public System.Action<Door> OnDoorStartedOpening;
    public System.Action<Door> OnDoorStartedClosing;
    
    // ────────────────────────────────────────────────────────
    // PROPERTIES
    // ────────────────────────────────────────────────────────
    
    public DoorConfig Config => config;
    public DoorState CurrentState => currentState;
    public bool IsOpen => currentState == DoorState.Open || currentState == DoorState.Opening;
    public bool IsClosed => currentState == DoorState.Closed || currentState == DoorState.Closing;
    public bool IsLocked => currentState == DoorState.Locked;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();
        zoneManager = Object.FindAnyObjectByType<ZoneManager>();
        
        // Validar configuración
        if (config.zoneA == null || config.zoneB == null)
        {
            Debug.LogWarning($"[DOOR] {gameObject.name} no tiene zonas conectadas", gameObject);
        }
    }
    
    private void Update()
    {
        // Actualizar animaciones y timers
        if (currentState == DoorState.Opening)
            UpdateOpening();
        else if (currentState == DoorState.Closing)
            UpdateClosing();
    }
    
    // ────────────────────────────────────────────────────────
    // ACCIÓN: ABRIR PUERTA
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Abrir la puerta (si está cerrada y no está bloqueada)
    /// </summary>
    public bool Open(GameObject opener = null)
    {
        // ¿Está ya abierta?
        if (IsOpen)
        {
            Debug.Log($"[DOOR] {config.doorName} is already open", gameObject);
            return true;
        }
        
        // ¿Está bloqueada?
        if (IsLocked)
        {
            Debug.Log($"[DOOR] {config.doorName} is locked!", gameObject);
            return false;
        }
        
        // ¿Necesita llave?
        if (config.requiresKey)
        {
            // Verificar si el opener tiene la llave
            // (Implementar según tu sistema de inventario)
            Debug.Log($"[DOOR] {config.doorName} requires key {config.keyRequired}", gameObject);
            return false;
        }
        
        // ════════════════════════════════════════════════════════════
        // INICIO DE APERTURA
        // ════════════════════════════════════════════════════════════
        
        currentState = DoorState.Opening;
        stateTimer = 0f;
        
        // EVENT: Comenzar a abrir
        OnDoorStartedOpening?.Invoke(this);
        
        // EVENT: Avisar a ZoneManager para pre-cargar zona destino
        if (zoneManager != null)
        {
            zoneManager.OnDoorStartedOpening(this);
        }
        
        // Audio
        if (config.playSoundOnOpen)
        {
            PlaySound(config.openSoundClip);
        }
        
        // Animación
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
        
        Debug.Log($"[DOOR] {config.doorName} opening...", gameObject);
        
        return true;
    }
    
    /// <summary>
    /// Cerrar la puerta
    /// </summary>
    public void Close()
    {
        if (IsClosed)
            return;
        
        currentState = DoorState.Closing;
        stateTimer = 0f;
        
        OnDoorStartedClosing?.Invoke(this);
        
        if (config.playSoundOnClose)
        {
            PlaySound(config.closeSoundClip);
        }
        
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
        }
        
        Debug.Log($"[DOOR] {config.doorName} closing...", gameObject);
    }
    
    /// <summary>
    /// Forzar apertura (por Stone o explosión)
    /// </summary>
    public bool ForceOpen(GameObject opener = null)
    {
        if (!config.canBeForcedOpen)
        {
            Debug.Log($"[DOOR] {config.doorName} cannot be forced open", gameObject);
            return false;
        }
        
        // Mismo que Open pero sin validar llaves
        currentState = DoorState.Opening;
        stateTimer = 0f;
        
        OnDoorStartedOpening?.Invoke(this);
        
        if (zoneManager != null)
        {
            zoneManager.OnDoorStartedOpening(this);
        }
        
        PlaySound("door_force_open");
        
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("ForceOpen");
        }
        
        return true;
    }
    
    /// <summary>
    /// Bloquear la puerta (no se puede abrir)
    /// </summary>
    public void Lock()
    {
        currentState = DoorState.Locked;
        doorCollider.enabled = true;  // Colisión activada
        Debug.Log($"[DOOR] {config.doorName} locked", gameObject);
    }
    
    /// <summary>
    /// Desbloquear la puerta
    /// </summary>
    public void Unlock()
    {
        if (currentState == DoorState.Locked)
        {
            currentState = DoorState.Closed;
            Debug.Log($"[DOOR] {config.doorName} unlocked", gameObject);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // ACTUALIZACIÓN: Animations progress
    // ────────────────────────────────────────────────────────
    
    private void UpdateOpening()
    {
        stateTimer += Time.deltaTime;
        
        if (stateTimer >= config.openDuration)
        {
            // Apertura completada
            currentState = DoorState.Open;
            stateTimer = 0f;
            
            // EVENT: Puerta completamente abierta
            OnDoorOpened?.Invoke(this);
            
            // ¿Auto-cierre?
            if (config.autoCloseDuration > 0)
            {
                // Esperar N segundos y cerrarse
                Invoke(nameof(Close), config.autoCloseDuration);
            }
        }
    }
    
    private void UpdateClosing()
    {
        stateTimer += Time.deltaTime;
        
        if (stateTimer >= config.closeDuration)
        {
            // Cierre completado
            currentState = DoorState.Closed;
            stateTimer = 0f;
            
            // EVENT: Puerta completamente cerrada
            OnDoorClosed?.Invoke(this);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // HELPERS
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Obtener la zona destino desde una zona origen
    /// </summary>
    public DynamicZone GetDestinationZone(DynamicZone fromZone)
    {
        if (fromZone == config.zoneA)
            return config.zoneB;
        else if (fromZone == config.zoneB)
            return config.zoneA;
        
        return null;
    }
    
    /// <summary>
    /// ¿Esta puerta conecta las dos zonas dadas?
    /// </summary>
    public bool ConnectZones(DynamicZone z1, DynamicZone z2)
    {
        return (config.zoneA == z1 && config.zoneB == z2) ||
               (config.zoneA == z2 && config.zoneB == z1);
    }
    
    /// <summary>
    /// Reproducir sonido de puerta
    /// </summary>
    private void PlaySound(string clipName)
    {
        // Integración con AudioManager
        // AudioManager.Instance.PlaySFX(clipName);
        
        Debug.Log($"[DOOR SFX] {clipName}");
    }
    
    // ────────────────────────────────────────────────────────
    // DEBUG
    // ────────────────────────────────────────────────────────
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        // Dibujar conexión entre zonas
        Gizmos.color = IsOpen ? Color.green : Color.red;
        
        if (config.zoneA != null && config.zoneB != null)
        {
            Vector3 posA = config.zoneA.Config.zoneCenter;
            Vector3 posB = config.zoneB.Config.zoneCenter;
            
            Gizmos.DrawLine(posA, posB);
            Gizmos.DrawWireSphere(posA, 1f);
            Gizmos.DrawWireSphere(posB, 1f);
        }
        
        // Mostrar estado
        Vector3 labelPos = transform.position + Vector3.up * 2f;
        UnityEditor.Handles.Label(labelPos, 
            $"{config.doorName}\n{currentState}");
    }
    
    #endif
}
