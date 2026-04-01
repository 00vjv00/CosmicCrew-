// ============================================================================
// CRATE - Contenedor interactivo con puertas
// Gestiona entrada/salida del player y rotación de puertas
// ============================================================================

using UnityEngine;

/// <summary>
/// Un crate con dos puertas que se abren cuando el player interactúa.
/// El trigger está en el crate root, detecta entrada/salida del player.
/// </summary>
public class Crate : MonoBehaviour
{
    [Header("== REFERENCIAS ==")]
    [SerializeField] private CrateDoor doorLeft;              // Puerta izquierda
    [SerializeField] private CrateDoor doorRight;             // Puerta derecha
    [SerializeField] private Rigidbody keyRigidbody;          // Llave que sale despedida
    [SerializeField] private float interactRange = 3f;        // Distancia máxima para interactuar
    
    [Header("== COMPORTAMIENTO ==")]
    [SerializeField] private bool canToggle = false;          // ¿Las puertas se cierran si vuelves a interactuar?
    [SerializeField] private float keyLaunchForce = 5f;       // Fuerza del impulso de la llave
    [SerializeField] private Vector3 keyLaunchDirection = Vector3.up;  // Dirección del impulso
    
    // ────────────────────────────────────────────────────────────
    // ESTADO INTERNO
    // ────────────────────────────────────────────────────────────
    
    private bool playerInRange = false;
    private Transform playerTransform = null;
    private Collider crateCollider;
    private bool doorsOpen = false;
    
    // ────────────────────────────────────────────────────────────
    // PROPIEDADES
    // ────────────────────────────────────────────────────────────
    
    public bool PlayerInRange => playerInRange;
    public bool DoorsOpen => doorsOpen;
    
    // ────────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────────
    
    private void Awake()
    {
        Debug.Log($"[CRATE] Awake() called for {gameObject.name}", gameObject);
        
        // Validar referencias
        if (doorLeft == null || doorRight == null)
        {
            Debug.LogError("[CRATE] Missing door references!", gameObject);
            return;
        }
        
        // Obtener el collider trigger
        crateCollider = GetComponent<Collider>();
        if (crateCollider == null)
        {
            Debug.LogError("[CRATE] No Collider found! Add a trigger collider to the Crate root.", gameObject);
            return;
        }
        
        if (!crateCollider.isTrigger)
        {
            Debug.LogWarning("[CRATE] Collider is not set as trigger. Changing to trigger...", gameObject);
            crateCollider.isTrigger = true;
        }
        
        Debug.Log($"[CRATE] Initialized: {gameObject.name} (Left door: {doorLeft.name}, Right door: {doorRight.name})", gameObject);
    }
    
    private void Start()
    {
        // Suscribirse al botón de interacción
        // Usar Start() en lugar de OnEnable() porque InputManager puede no estar inicializado aún
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractButtonPressed += OnInteractButtonPressed;
            Debug.Log($"[CRATE] {gameObject.name}: Subscribed to OnInteractButtonPressed", gameObject);
        }
        else
        {
            Debug.LogError("[CRATE] InputManager.Instance is NULL! Cannot subscribe to interact button!", gameObject);
        }
    }
    
    private void OnDisable()
    {
        // Desuscribirse
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractButtonPressed -= OnInteractButtonPressed;
        }
    }
    
    // ────────────────────────────────────────────────────────────
    // TRIGGERS - Entrada/Salida del player
    // ────────────────────────────────────────────────────────────
    
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        playerInRange = true;
        playerTransform = collision.transform;
        
        Debug.Log($"[CRATE] Player entered {gameObject.name}", gameObject);
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        playerInRange = false;
        playerTransform = null;
        
        Debug.Log($"[CRATE] Player left {gameObject.name}", gameObject);
    }
    
    // ────────────────────────────────────────────────────────────
    // INTERACCIÓN
    // ────────────────────────────────────────────────────────────
    
    private void OnInteractButtonPressed()
    {
        Debug.Log($"[CRATE] OnInteractButtonPressed called! playerInRange={playerInRange}", gameObject);
        
        // Solo si el player está en rango
        if (!playerInRange || playerTransform == null)
        {
            Debug.LogWarning($"[CRATE] {gameObject.name}: Player not in range or playerTransform is null", gameObject);
            return;
        }
        
        // Validar distancia (opcional, extra safety)
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > interactRange)
        {
            Debug.LogWarning($"[CRATE] {gameObject.name}: Distance to player ({distanceToPlayer}) exceeds interact range ({interactRange})", gameObject);
            return;
        }
        
        // Abrir o toggle
        if (!doorsOpen)
        {
            OpenDoors();
        }
        else if (canToggle)
        {
            CloseDoors();
        }
    }
    
    /// <summary>
    /// Abrir ambas puertas
    /// </summary>
    public void OpenDoors()
    {
        if (doorsOpen) return;
        
        doorLeft.Open();
        doorRight.Open();
        doorsOpen = true;
        
        // Lanzar la llave si existe
        if (keyRigidbody != null)
        {
            keyRigidbody.linearVelocity = Vector3.zero;  // Reset velocidad anterior
            keyRigidbody.AddForce(keyLaunchDirection.normalized * keyLaunchForce, ForceMode.Impulse);
            Debug.Log($"[CRATE] Key launched with force {keyLaunchForce}", gameObject);
        }
        
        Debug.Log($"[CRATE] Opening doors: {gameObject.name}", gameObject);
    }
    
    /// <summary>
    /// Cerrar ambas puertas
    /// </summary>
    public void CloseDoors()
    {
        if (!doorsOpen) return;
        
        doorLeft.Close();
        doorRight.Close();
        doorsOpen = false;
        
        Debug.Log($"[CRATE] Closing doors: {gameObject.name}", gameObject);
    }
    
    /// <summary>
    /// Alternar apertura/cierre
    /// </summary>
    public void ToggleDoors()
    {
        if (doorsOpen)
            CloseDoors();
        else
            OpenDoors();
    }
    
    // ────────────────────────────────────────────────────────────
    // DEBUG & GIZMOS
    // ────────────────────────────────────────────────────────────
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        // Dibujar rango de interacción
        Gizmos.color = playerInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
        
        // Label
        UnityEditor.Handles.Label(
            transform.position + Vector3.up,
            $"Crate: {gameObject.name}\nPlayer in range: {playerInRange}\nDoors open: {doorsOpen}"
        );
    }
    
    #endif
}
