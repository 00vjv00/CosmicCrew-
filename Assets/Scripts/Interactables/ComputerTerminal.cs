// ============================================================================
// COMPUTER TERMINAL - Terminal que abre puertas de zunas
// El player interactúa con la computadora y se abre la puerta correspondiente
// ============================================================================

using UnityEngine;

/// <summary>
/// Una terminal/computadora que abre una puerta de zona cuando se interactúa.
/// Requiere: Door.cs, InventorySystem.cs, InputManager.cs
/// </summary>
public class ComputerTerminal : MonoBehaviour
{
    [Header("== REFERENCIAS ==")]
    [SerializeField] private Door door;                      // La puerta a abrir
    [SerializeField] private float interactRange = 3f;       // Distancia máxima para interactuar
    
    [Header("== COMPORTAMIENTO ==")]
    [SerializeField] private bool requiresKey = true;        // ¿Requiere llave?
    [SerializeField] private bool consumeKeyOnUse = true;    // ¿Consumir la llave al abrir?
    
    // ────────────────────────────────────────────────────────
    // ESTADO INTERNO
    // ────────────────────────────────────────────────────────
    
    private bool playerInRange = false;
    private Transform playerTransform = null;
    private Collider triggerCollider;
    
    // ────────────────────────────────────────────────────────
    // PROPIEDADES
    // ────────────────────────────────────────────────────────
    
    public bool PlayerInRange => playerInRange;
    public Door DoorReference => door;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        Debug.Log($"[COMPUTER] Awake() called for {gameObject.name}", gameObject);
        
        // Validar referencias
        if (door == null)
        {
            Debug.LogError("[COMPUTER] Missing door reference!", gameObject);
            return;
        }
        
        // Obtener el collider trigger
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogError("[COMPUTER] No Collider found! Add a trigger collider.", gameObject);
            return;
        }
        
        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("[COMPUTER] Collider is not set as trigger. Changing to trigger...", gameObject);
            triggerCollider.isTrigger = true;
        }
        
        Debug.Log($"[COMPUTER] Initialized: {gameObject.name} (Door: {door.Config.doorName})", gameObject);
    }
    
    private void Start()
    {
        // Suscribirse al botón de interacción
        // Usar Start() en lugar de OnEnable() porque InputManager puede no estar inicializado aún
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractButtonPressed += OnInteractButtonPressed;
            Debug.Log($"[COMPUTER] {gameObject.name}: Subscribed to OnInteractButtonPressed", gameObject);
        }
        else
        {
            Debug.LogError("[COMPUTER] InputManager.Instance is NULL! Cannot subscribe to interact button!", gameObject);
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
    
    // ────────────────────────────────────────────────────────
    // TRIGGERS - Entrada/Salida del player
    // ────────────────────────────────────────────────────────
    
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        playerInRange = true;
        playerTransform = collision.transform;
        
        Debug.Log($"[COMPUTER] Player entered {gameObject.name}", gameObject);
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        playerInRange = false;
        playerTransform = null;
        
        Debug.Log($"[COMPUTER] Player left {gameObject.name}", gameObject);
    }
    
    // ────────────────────────────────────────────────────────
    // INTERACCIÓN
    // ────────────────────────────────────────────────────────
    
    private void OnInteractButtonPressed()
    {
        Debug.Log($"[COMPUTER] OnInteractButtonPressed called! playerInRange={playerInRange}", gameObject);
        
        // Solo si el player está en rango
        if (!playerInRange || playerTransform == null)
        {
            Debug.LogWarning($"[COMPUTER] {gameObject.name}: Player not in range or playerTransform is null", gameObject);
            return;
        }
        
        // Validar distancia (opcional, extra safety)
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > interactRange)
        {
            Debug.LogWarning($"[COMPUTER] {gameObject.name}: Distance to player ({distanceToPlayer}) exceeds interact range ({interactRange})", gameObject);
            return;
        }
        
        // Verificar si tiene la llave (si es requerida)
        if (requiresKey && !InventorySystem.Instance.HasCrateKey)
        {
            Debug.Log($"[COMPUTER] {gameObject.name}: Player doesn't have the key!", gameObject);
            return;
        }
        
        // Intentar abrir la puerta
        bool opened = door.Open(playerTransform.gameObject);
        
        if (opened && consumeKeyOnUse && requiresKey)
        {
            // Consumir la llave si fue exitoso
            InventorySystem.Instance.ConsumeCrateKey();
            Debug.Log($"[COMPUTER] {gameObject.name}: Key consumed after opening door", gameObject);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // DEBUG & GIZMOS
    // ────────────────────────────────────────────────────────
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        // Dibujar rango de interacción
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
        
        // Si hay puerta asignada, dibujar línea hacia ella
        if (door != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, door.transform.position);
        }
    }
    
    #endif
}
