// ============================================================================
// STATION MANAGER - GESTOR DE LA ESTACIÓN
// Archivo: Assets/Scripts/Station/StationManager.cs
// Descripción: Maneja la estación espacial. Versión MINIMALISTA.
// ============================================================================

using UnityEngine;

/// <summary>
/// StationManager gestiona todo lo relacionado con la estación.
/// 
/// RESPONSABILIDADES (MÍNIMAS):
/// - Inicializar la estación
/// - Proporcionar información de la estación
/// 
/// NO hace (todavía):
/// - Rotación de estación
/// - Carga dinámica de zonas
/// - Generación procedural
/// </summary>
public class StationManager : MonoBehaviour
{
    // ========================================================================
    // SINGLETON
    // ========================================================================
    
    public static StationManager Instance { get; private set; }
    
    // ========================================================================
    // INFORMACIÓN DE LA ESTACIÓN
    // ========================================================================
    
    /// <summary>
    /// Nombre de la estación
    /// </summary>
    [SerializeField] private string stationName = "Prison Station";
    
    /// <summary>
    /// Número total de sectores
    /// </summary>
    [SerializeField] private int totalSectors = GameConstants.TOTAL_SECTORS;
    
    /// <summary>
    /// Número total de planos
    /// </summary>
    [SerializeField] private int totalPlanes = GameConstants.TOTAL_PLANES;
    
    /// <summary>
    /// Transform del jugador (para referencias)
    /// </summary>
    private Transform playerTransform;
    
    // ========================================================================
    // INICIALIZACIÓN
    // ========================================================================
    
    void Awake()
    {
        // SINGLETON PATTERN
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[STATION] Ya existe una instancia. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[STATION] Inicializado correctamente");
    }
    
    void Start()
    {
        // Obtener referencia al jugador
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("[STATION] Jugador encontrado");
        }
        else
        {
            Debug.LogWarning("[STATION] Jugador no encontrado");
        }
        
        // Inicializar estación
        InitializeStation();
    }
    
    // ========================================================================
    // INICIALIZACIÓN DE ESTACIÓN
    // ========================================================================
    
    /// <summary>
    /// Inicializar la estación
    /// </summary>
    private void InitializeStation()
    {
        Debug.Log($"[STATION] Inicializando estación '{stationName}'");
        Debug.Log($"[STATION]   Sectores: {totalSectors}");
        Debug.Log($"[STATION]   Planos: {totalPlanes}");
        Debug.Log($"[STATION]   Total: {totalPlanes * totalSectors} planos");
    }
    
    // ========================================================================
    // QUERIES - INFORMACIÓN
    // ========================================================================
    
    /// <summary>
    /// Obtener nombre de la estación
    /// </summary>
    public string GetStationName()
    {
        return stationName;
    }
    
    /// <summary>
    /// Obtener número total de sectores
    /// </summary>
    public int GetTotalSectors()
    {
        return totalSectors;
    }
    
    /// <summary>
    /// Obtener número de planos por sector
    /// </summary>
    public int GetPlanesPerSector()
    {
        return totalPlanes;
    }
    
    /// <summary>
    /// Obtener número total de planos
    /// </summary>
    public int GetTotalPlanes()
    {
        return totalPlanes * totalSectors;
    }
    
    /// <summary>
    /// Obtener posición del jugador
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        if (playerTransform != null)
        {
            return playerTransform.position;
        }
        return Vector3.zero;
    }
    
    // ========================================================================
    // DEBUG
    // ========================================================================
    
    public void DebugPrintState()
    {
        Debug.Log("[STATION STATE]");
        Debug.Log($"  Name: {stationName}");
        Debug.Log($"  Sectors: {totalSectors}");
        Debug.Log($"  Planes per Sector: {totalPlanes}");
        Debug.Log($"  Total Planes: {GetTotalPlanes()}");
        Debug.Log($"  Player Position: {GetPlayerPosition()}");
    }
}
