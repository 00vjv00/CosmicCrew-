// ============================================================================
// PLAYER CONTROLLER - CONTROLADOR DEL JUGADOR
// Archivo: Assets/Scripts/Player/PlayerController.cs
// Descripción: Maneja movimiento del jugador. Versión MINIMALISTA.
// ============================================================================

using UnityEngine;

/// <summary>
/// PlayerController gestiona el movimiento del jugador.
/// 
/// RESPONSABILIDADES (MÍNIMAS):
/// - Leer input de InputManager
/// - Aplicar movimiento al Rigidbody
/// - Mantener orientación correcta
/// 
/// NO hace:
/// - Gravedad artificial (todavía)
/// - Poderes (todavía)
/// - Animaciones (todavía)
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========================================================================
    // REFERENCIAS
    // ========================================================================
    
    private Rigidbody rb;
    private InputManager inputManager;
    
    // ========================================================================
    // PARÁMETROS DE MOVIMIENTO
    // ========================================================================
    
    /// <summary>
    /// Velocidad de movimiento (unidades/segundo)
    /// </summary>
    [SerializeField] private float moveSpeed = 5f;
    
    // ========================================================================
    // ESTADO
    // ========================================================================
    
    private Vector3 currentMoveDirection = Vector3.zero;
    
    // ========================================================================
    // INICIALIZACIÓN
    // ========================================================================
    
    void Awake()
    {
        // Obtener componentes del jugador
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[PLAYER] No hay Rigidbody en el jugador");
        }
        
        Debug.Log("[PLAYER] Inicializado");
    }
    
    void Start()
    {
        // Obtener referencia a InputManager
        inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogWarning("[PLAYER] InputManager no encontrado");
        }
    }
    
    void FixedUpdate()
    {
        // Actualizar movimiento
        UpdateMovement();
    }
    
    // ========================================================================
    // MOVIMIENTO
    // ========================================================================
    
    /// <summary>
    /// Actualizar movimiento del jugador
    /// </summary>
    private void UpdateMovement()
    {
        if (inputManager == null || rb == null)
            return;
        
        // Obtener dirección de input
        currentMoveDirection = inputManager.GetMoveDirection();
        
        // Si no hay movimiento, detener
        if (currentMoveDirection.magnitude < 0.1f)
        {
            StopMovement();
            return;
        }
        
        // Calcular velocidad de movimiento
        Vector3 moveVelocity = currentMoveDirection * moveSpeed;
        
        // Aplicar movimiento sin afectar el eje Y (gravedad)
        rb.linearVelocity = new Vector3(
            moveVelocity.x,
            rb.linearVelocity.y,  // Mantener velocidad vertical actual
            moveVelocity.z
        );
    }
    
    /// <summary>
    /// Detener movimiento horizontal
    /// </summary>
    private void StopMovement()
    {
        // Detener solo movimiento horizontal, mantener vertical
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }
    
    // ========================================================================
    // DEBUG
    // ========================================================================
    
    public void DebugPrintState()
    {
        Debug.Log("[PLAYER STATE]");
        Debug.Log($"  Position: {transform.position}");
        Debug.Log($"  Velocity: {rb.linearVelocity}");
        Debug.Log($"  Move Direction: {currentMoveDirection}");
    }
}
