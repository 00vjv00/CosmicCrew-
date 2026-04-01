// ============================================================================
// CELL 1 SECTOR MANAGER
// Cosmic Crew - Gestor local de la celda 1 (independiente del generador procedural)
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestor de zonas para la celda 1. 
/// 
/// DIFERENCIA CON ZoneManager GLOBAL:
/// - Solo busca zonas DENTRO de este sector (GetComponentsInChildren)
/// - NO busca globalmente (no usa FindObjectsByType)
/// - Todas las coordenadas son LOCALES al prefab Cell1
/// - El prefab Cell1 puede moverse a cualquier posición mundial sin romper nada
/// 
/// RESPONSABILIDADES:
/// - Inicializar zonas locales
/// - Construir grafo BFS de zonas desde la zona actual del player
/// - Manejar transiciones entre zonas
/// - Optimizar carga/descarga de zonas basado en posición del player
/// </summary>
public class Cell1SectorManager : MonoBehaviour
{
    // ════════════════════════════════════════════════════════════════
    // SINGLETON (POR SECTOR)
    // ════════════════════════════════════════════════════════════════
    
    public static Cell1SectorManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    // ════════════════════════════════════════════════════════════════
    // REFERENCIAS
    // ════════════════════════════════════════════════════════════════
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int maxGraphDepth = 4;
    [SerializeField] private bool debugMode = true;
    
    private DynamicZone currentZone;
    private List<DynamicZone> allZonesInSector = new();
    private Dictionary<DynamicZone, ZoneGraphNode> zoneGraph = new();
    
    // ════════════════════════════════════════════════════════════════
    // EVENTOS
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>Disparado cuando el player cambia de zona</summary>
    public System.Action<DynamicZone, DynamicZone> OnPlayerZoneChanged;
    
    /// <summary>Disparado cuando una zona se activa</summary>
    public System.Action<DynamicZone> OnZoneActivated;
    
    // ════════════════════════════════════════════════════════════════
    // CICLO DE VIDA
    // ════════════════════════════════════════════════════════════════
    
    private void Start()
    {
        InitializeSector();
        SpawnPlayerIfNeeded();
    }
    
    // ¡NO LLAMAR UpdateZoneVisibility() EN LATEUPDATE!
    // Solo llamarlo cuando el player cambia de zona para evitar parpadeos
    
    // ════════════════════════════════════════════════════════════════
    // INICIALIZACIÓN
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>Inicializa todas las zonas del sector</summary>
    private void InitializeSector()
    {
        // Buscar todas las zonas DENTRO de este prefab (no globalmente)
        allZonesInSector.AddRange(GetComponentsInChildren<DynamicZone>());
        
        if (debugMode)
            Debug.Log($"[CELL1] Inicializadas {allZonesInSector.Count} zonas en Cell1");
        
        // Conectar triggers de zonas a este manager
        foreach (var zone in allZonesInSector)
        {
            // IMPORTANTE: Desuscribirse de ZoneManager GLOBAL
            // Cell1SectorManager es el ÚNICO manager de estas zonas
            var zm = ZoneManager.Instance;
            if (zm != null)
            {
                zone.OnPlayerEntered -= zm.OnPlayerEnteredZone;
                zone.OnPlayerExited -= zm.OnPlayerExitedZone;
                if (debugMode)
                    Debug.Log($"[CELL1] Desuscrito ZoneManager de zona: {zone.Config.zoneName}");
            }
            
            // Subscribirse a eventos de la zona (SOLO Cell1SectorManager)
            zone.OnPlayerEntered += OnPlayerEnteredZone;
            zone.OnPlayerExited += OnPlayerExitedZone;
            zone.OnZoneActivated += (z) => OnZoneActivated?.Invoke(z);
            
            // Inicialmente zona de inicio = Active, otras = Unknown (no descubiertas)
            if (zone.Config.isStartingZone)
            {
                zone.SetZoneState(ZoneState.Active);
                if (debugMode)
                    Debug.Log($"[CELL1] Zona inicio (ACTIVE): {zone.Config.zoneName}");
            }
            else
            {
                zone.SetZoneState(ZoneState.Unknown);
                if (debugMode)
                    Debug.Log($"[CELL1] Zona registrada: {zone.Config.zoneName}");
            }
        }
    }
    
    /// <summary>Spawna el player si no existe</summary>
    private void SpawnPlayerIfNeeded()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("[CELL1] Player no encontrado en la escena (tag 'Player')");
            }
        }
        
        // Inicializar currentZone (buscar zona de inicio)
        if (currentZone == null)
        {
            var startZone = allZonesInSector.Find(z => z.Config.isStartingZone);
            if (startZone != null)
            {
                currentZone = startZone;
                RebuildZoneGraph(startZone);
                if (debugMode)
                    Debug.Log($"[CELL1] Zona inicial establecida: {startZone.Config.zoneName}");
            }
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // MANEJO DE TRANSICIONES DE ZONAS
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>Se llama cuando el player entra a una zona</summary>
    private void OnPlayerEnteredZone(DynamicZone zone)
    {
        DynamicZone oldZone = currentZone;
        currentZone = zone;
        
        if (debugMode)
            Debug.Log($"[CELL1] Player entra en zona: {zone.Config.zoneName}");
        
        // Construir grafo BFS desde la zona actual
        RebuildZoneGraph(zone);
        
        // Actualizar visibilidad de todas las zonas (esto setea Active a currentZone)
        UpdateZoneVisibility();
        
        // Disparar evento
        OnPlayerZoneChanged?.Invoke(oldZone, zone);
    }
    
    /// <summary>Se llama cuando el player sale de una zona</summary>
    private void OnPlayerExitedZone(DynamicZone zone)
    {
        if (debugMode)
            Debug.Log($"[CELL1] Player sale de zona: {zone.Config.zoneName}");
        
        // NO actualizar aquí - currentZone aún es la zona vieja
        // Los estados se actualizan cuando ENTRES en la siguiente zona
    }
    
    // ════════════════════════════════════════════════════════════════
    // GRAFO BFS Y VISIBILIDAD
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>Construye un grafo BFS de zonas partiendo desde la zona dada</summary>
    private void RebuildZoneGraph(DynamicZone startZone)
    {
        zoneGraph.Clear();
        
        // USAR EL BUILDER EXISTENTE (en 01_ZoneGraphNode.cs)
        ZoneGraphNode rootNode = ZoneGraphBuilder.BuildGraph(startZone, maxGraphDepth);
        
        if (rootNode != null)
        {
            // Aplanar el árbol a diccionario para acceso rápido
            List<ZoneGraphNode> flatList = new();
            ZoneGraphBuilder.FlattenGraph(rootNode, flatList);
            
            foreach (var node in flatList)
            {
                zoneGraph[node.zone] = node;
            }
        }
        
        if (debugMode)
            Debug.Log($"[CELL1] Grafo construido con {zoneGraph.Count} zonas");
    }
    
    /// <summary>Actualiza el estado de visibilidad de todas las zonas basado en si fueron visitadas</summary>
    private void UpdateZoneVisibility()
    {
        if (debugMode)
            Debug.Log($"[CELL1 VISIBILITY] Actualizando estados... (Zona actual: {currentZone?.Config.zoneName})");
        
        foreach (var zone in allZonesInSector)
        {
            ZoneState newState;
            ZoneState oldState = zone.CurrentState;
            
            if (zone == currentZone)
            {
                newState = ZoneState.Active;
            }
            else if (zone.IsDiscovered)
            {
                newState = ZoneState.Active;
            }
            else if (zone.CurrentState == ZoneState.Known)
            {
                // Mantener estado Known (puertas abiertas precargan zonas)
                newState = ZoneState.Known;
            }
            else
            {
                newState = ZoneState.Unknown;
            }
            
            zone.SetZoneState(newState);
            
            if (debugMode && oldState != newState)
                Debug.Log($"  {zone.Config.zoneName}: {oldState} → {newState} (discovered={zone.IsDiscovered})");
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // GETTERS
    // ════════════════════════════════════════════════════════════════
    
    public DynamicZone GetCurrentZone() => currentZone;
    public List<DynamicZone> GetAllZones() => allZonesInSector;
    
    /// <summary>Obtiene una zona por su nombre</summary>
    public DynamicZone GetZoneByName(string zoneName)
    {
        return allZonesInSector.Find(z => z.Config.zoneName == zoneName);
    }
}
