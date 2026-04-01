// ============================================================================
// STAIR CONTROLLER - Controlador de movimiento en escaleras
// Cosmic Crew - Maneja el movimiento vertical en zonas de escaleras
// ============================================================================

using UnityEngine;

/// <summary>
/// Componente attachado al Player que maneja el movimiento en escaleras
/// Controla subida/bajada vertical mientras está en modo escalera
/// </summary>
public class StairController : MonoBehaviour
{
    // ────────────────────────────────────────────────────────
    // REFERENCIAS
    // ────────────────────────────────────────────────────────
    
    private Transform playerTransform;
    private StairZone currentStair;
    
    // ────────────────────────────────────────────────────────
    // ESTADO
    // ────────────────────────────────────────────────────────
    
    private int currentFloor = 0;
    private int topFloor = 0;
    private float floorHeight = 3f;
    private bool isInStairMode = false;
    
    private float stairClimbSpeed = 4f;  // Velocidad de subida suave
    private float targetHeight = 0f;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        playerTransform = transform;
        Debug.Log("[STAIR] StairController initialized");
    }
    
    // ────────────────────────────────────────────────────────
    // UPDATE
    // ────────────────────────────────────────────────────────
    
    private void FixedUpdate()
    {
        if (!isInStairMode)
            return;
        
        HandleStairInput();
        UpdateStairMovement();
    }
    
    // ────────────────────────────────────────────────────────
    // LÓGICA: Entrada y movimiento
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Procesar input de escaleras (W/↑ sube, S/↓ baja)
    /// </summary>
    private void HandleStairInput()
    {
        // Eje vertical: W/↑ = +1, S/↓ = -1
        float verticalInput = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            int newFloor = currentFloor;
            
            // Calcular nuevo piso basado en input
            if (verticalInput > 0)
            {
                // Subir
                newFloor = Mathf.Min(currentFloor + 1, topFloor);
            }
            else
            {
                // Bajar
                newFloor = Mathf.Max(currentFloor - 1, 0);
            }
            
            // Cambiar piso si es diferente
            if (newFloor != currentFloor)
            {
                currentFloor = newFloor;
                targetHeight = currentFloor * floorHeight;
                
                Debug.Log($"[STAIR] Floor changed: {currentFloor} (Height: {targetHeight:F1}u)");
            }
        }
    }
    
    /// <summary>
    /// Actualizar posición del jugador hacia la altura objetivo
    /// </summary>
    private void UpdateStairMovement()
    {
        // Suavizar movimiento vertical
        Vector3 currentPos = playerTransform.position;
        float newY = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * stairClimbSpeed);
        
        Vector3 newPos = currentPos;
        newPos.y = newY;
        
        // Aplicar posición
        playerTransform.position = newPos;
    }
    
    // ────────────────────────────────────────────────────────
    // CONTROL: Activar/desactivar modo escalera
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Activar modo escalera
    /// Llamado desde StairZone.OnPlayerEntered
    /// </summary>
    public void StartStairMode(StairZone stair, int bottomFloor, int topFloor_param, float floorHeight_param)
    {
        if (stair == null)
        {
            Debug.LogError("[STAIR] Cannot start stair mode with null StairZone!");
            return;
        }
        
        isInStairMode = true;
        currentStair = stair;
        currentFloor = bottomFloor;
        topFloor = topFloor_param;
        floorHeight = floorHeight_param;
        
        // Posicionar al jugador en el piso inicial
        targetHeight = bottomFloor * floorHeight;
        Vector3 startPos = playerTransform.position;
        startPos.y = targetHeight;
        playerTransform.position = startPos;
        
        Debug.Log($"[STAIR] Started stair mode - Floors {bottomFloor} to {topFloor}, Floor height {floorHeight}u, Player at {targetHeight}u");
    }
    
    /// <summary>
    /// Desactivar modo escalera
    /// Llamado desde StairZone.OnPlayerExited
    /// </summary>
    public void StopStairMode()
    {
        if (!isInStairMode)
            return;
        
        isInStairMode = false;
        currentStair = null;
        
        Debug.Log($"[STAIR] Stopped stair mode - Player at floor {currentFloor}");
    }
    
    // ────────────────────────────────────────────────────────
    // GETTERS
    // ────────────────────────────────────────────────────────
    
    public bool IsInStairMode => isInStairMode;
    public int GetCurrentFloor() => currentFloor;
    public int GetTopFloor() => topFloor;
    public float GetTargetHeight() => targetHeight;
}
