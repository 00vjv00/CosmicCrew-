// ============================================================================
// STAIR ZONE - Zona especial de escaleras con vista de corte vertical
// Cosmic Crew - Sistema de Zonas con vista arquitectónica
// ============================================================================

using UnityEngine;

/// <summary>
/// Zona especial que representa una escalera (vista de corte vertical)
/// El jugador sube/baja pisos mientras la cámara muestra un corte vertical del edificio
/// </summary>
public class StairZone : DynamicZone
{
    [Header("═ ESCALERA - CORTE VERTICAL ═")]
    [SerializeField] private int bottomFloor = 0;
    [SerializeField] private int topFloor = 3;
    [SerializeField] private float floorHeight = 3f;  // Unidades entre pisos
    
    [Header("═ CÁMARA CORTE VERTICAL ═")]
    [SerializeField] private float cameraDistance = 30f;      // Distancia para ver todos los pisos
    [SerializeField] private float cameraHeight = 6f;         // Altura media de los pisos
    [SerializeField] private float orthographicSize = 12f;    // Tamaño ortho para captar altura total
    [SerializeField] private float cameraTransitionSpeed = 2f;
    
    private StairController playerStairMovement;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Start()
    {
        // Subscribirse a eventos de entrada/salida
        OnPlayerEntered += EnterStairZone;
        OnPlayerExited += ExitStairZone;
        
        Debug.Log($"[STAIR ZONE] {Config.zoneName} initialized: Floors {bottomFloor}-{topFloor}, Height per floor: {floorHeight}u", gameObject);
    }
    
    // ────────────────────────────────────────────────────────
    // EVENT HANDLERS
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// El player entró a la zona de escaleras
    /// </summary>
    private void EnterStairZone(DynamicZone zone)
    {
        // Obtener el controlador de escaleras del jugador
        playerStairMovement = FindAnyObjectByType<StairController>();
        if (playerStairMovement != null)
        {
            playerStairMovement.StartStairMode(this, bottomFloor, topFloor, floorHeight);
            Debug.Log($"[STAIR] {Config.zoneName}: Player entered - Stair mode activated", gameObject);
        }
        else
        {
            Debug.LogWarning($"[STAIR] {Config.zoneName}: StairController not found on player!", gameObject);
        }
        
        // Cambiar cámara a vista de CORTE VERTICAL
        CameraController instance = CameraController.Instance;
        if (instance != null)
        {
            instance.SetStairCutMode(cameraDistance, cameraHeight, orthographicSize, cameraTransitionSpeed);
        }
        else
        {
            Debug.LogWarning("[STAIR] CameraController.Instance not found!");
        }
    }
    
    /// <summary>
    /// El player salió de la zona de escaleras
    /// </summary>
    private void ExitStairZone(DynamicZone zone)
    {
        // Desactivar modo escalera del jugador
        if (playerStairMovement != null)
        {
            playerStairMovement.StopStairMode();
            Debug.Log("[STAIR] Player exited - Stair mode deactivated", gameObject);
        }
        
        // Restaurar cámara isométrica normal
        CameraController instance = CameraController.Instance;
        if (instance != null)
        {
            instance.ExitStairCutMode();
        }
    }
    
    // ────────────────────────────────────────────────────────
    // GETTERS
    // ────────────────────────────────────────────────────────
    
    public float GetFloorHeight() => floorHeight;
    public int GetTopFloor() => topFloor;
    public int GetBottomFloor() => bottomFloor;
    
    public float GetCameraDistance() => cameraDistance;
    public float GetCameraHeight() => cameraHeight;
    public float GetOrthographicSize() => orthographicSize;
}
