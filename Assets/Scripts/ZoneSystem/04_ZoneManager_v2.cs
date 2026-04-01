// ============================================================================
// ZONE MANAGER (VERSIÓN 2) - Orquestador central event-driven
// Cosmic Crew - Sistema de Zonas Event-Driven
// ============================================================================

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manager central que controla toda la lógica de zonas
/// Versión 2: Event-driven, sin polling
/// 
/// RESPONSABILIDADES:
/// 1. Recibir eventos de entrada/salida de zonas
/// 2. Construir grafo jerárquico de zonas
/// 3. Determinar qué zonas cargar/descargar
/// 4. Manejar eventos de puertas
/// 5. Coordinar con otros sistemas (cámara, audio, etc.)
/// </summary>
public class ZoneManager : MonoBehaviour
{
    // ────────────────────────────────────────────────────────
    // SINGLETON
    // ────────────────────────────────────────────────────────
    
    public static ZoneManager Instance { get; private set; }
    
    // ────────────────────────────────────────────────────────
    // ESTADO GLOBAL
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// La zona donde está el player actualmente
    /// </summary>
    private DynamicZone currentZone;
    
    /// <summary>
    /// La zona anterior donde estaba el player
    /// </summary>
    private DynamicZone previousZone;
    
    /// <summary>
    /// Grafo jerárquico de zonas desde la zona actual
    /// </summary>
    private ZoneGraph currentZoneGraph;
    
    /// <summary>
    /// Todas las zonas en el juego (por ID para búsqueda rápida)
    /// </summary>
    private Dictionary<int, DynamicZone> allZonesById = new();
    
    // ────────────────────────────────────────────────────────
    // CONFIGURACIÓN
    // ────────────────────────────────────────────────────────
    
    [SerializeField]
    [Range(2, 6)]
    private int maxGraphDepth = 4;  // Máxima profundidad de BFS
    
    [SerializeField]
    private bool debugMode = false;
    
    [SerializeField]
    private bool drawGizmos = false;
    
    // ────────────────────────────────────────────────────────
    // EVENTOS GLOBALES - Puede subscribirse cualquier sistema
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Se dispara cuando el player cambia de zona
    /// </summary>
    public System.Action<DynamicZone, DynamicZone> OnPlayerZoneChanged;
    
    /// <summary>
    /// Se dispara cuando se actualiza el grafo de zonas
    /// </summary>
    public System.Action<ZoneGraph> OnZoneGraphUpdated;
    
    /// <summary>
    /// Se dispara cuando se completa una pre-carga de zona
    /// </summary>
    public System.Action<DynamicZone> OnZonePreloaded;
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Encontrar todas las zonas en la escena
        FindAllZones();
        
        Debug.Log($"[ZONE MANAGER] Initialized with {allZonesById.Count} zones");
    }
    
    private void FindAllZones()
    {
        DynamicZone[] zonesInScene = FindObjectsByType<DynamicZone>(FindObjectsSortMode.None);
        
        foreach (var zone in zonesInScene)
        {
            int zoneId = zone.Config.zoneID;
            
            if (allZonesById.ContainsKey(zoneId))
            {
                Debug.LogWarning($"[ZONE MANAGER] Duplicate zone ID {zoneId}", zone.gameObject);
            }
            else
            {
                allZonesById[zoneId] = zone;
                
                // CRÍTICO: Suscribirse a eventos de esta zona
                zone.OnPlayerEntered += OnPlayerEnteredZone;
                zone.OnPlayerExited += OnPlayerExitedZone;
                
                Debug.Log($"[ZONE MANAGER] Registered event listeners for zone: {zone.Config.zoneName}");
            }
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // EVENT HANDLERS - Recibidos desde DynamicZone y Door
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// El player acaba de entrar a una zona
    /// ← DISPARA DESDE: DynamicZone.OnTriggerEnter
    /// </summary>
    public void OnPlayerEnteredZone(DynamicZone zone)
    {
        if (zone == currentZone)
            return;  // Ya estamos en esta zona
        
        if (zone == null)
        {
            Debug.LogError("[ZONE MANAGER] OnPlayerEnteredZone called with null zone");
            return;
        }
        
        previousZone = currentZone;
        currentZone = zone;
        
        if (debugMode)
            Debug.Log($"[ZONE] Player ENTERED {zone.Config.zoneName}", zone.gameObject);
        
        // 1. Re-construir grafo desde nueva zona
        RebuildZoneGraph(zone);
        
        // 2. Actualizar visibilidad de todas las zonas
        UpdateZoneVisibility();
        
        // 3. Disparar eventos
        OnPlayerZoneChanged?.Invoke(previousZone, currentZone);
    }
    
    /// <summary>
    /// El player acaba de salir de una zona
    /// ← DISPARA DESDE: DynamicZone.OnTriggerExit
    /// </summary>
    public void OnPlayerExitedZone(DynamicZone zone)
    {
        if (zone != currentZone)
            return;  // No era nuestra zona actual
        
        if (debugMode)
            Debug.Log($"[ZONE] Player exited {zone.Config.zoneName}", zone.gameObject);
        
        // Solo esperamos que OnPlayerEnteredZone se dispare desde la otra zona
        // No hacemos nada aquí - dejar que el OnTriggerEnter de la nueva zona trigger the update
    }
    
    /// <summary>
    /// Buscar la zona en la que está el jugador ahora
    /// Fallback si hay gaps en los colliders
    /// </summary>
    private DynamicZone FindCurrentZone()
    {
        // Obtener posición del jugador
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null)
            return null;
        
        Vector3 playerPos = player.transform.position;
        DynamicZone closestZone = null;
        float closestDistance = float.MaxValue;
        
        foreach (var kvp in allZonesById)
        {
            DynamicZone zone = kvp.Value;
            if (zone.Config == null) continue;
            
            // Primero: verificar si está exactamente dentro de los bounds de esta zona
            Bounds zoneBounds = new Bounds(zone.Config.zoneCenter, zone.Config.zoneSize);
            if (zoneBounds.Contains(playerPos))
            {
                return zone;  // Match exacto, retornar inmediatamente
            }
            
            // Fallback: encontrar la zona más cercana por distancia
            float dist = Vector3.Distance(playerPos, zone.Config.zoneCenter);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestZone = zone;
            }
        }
        
        // Si no está dentro de ningún bounds, usar la zona más cercana
        if (closestZone != null && debugMode)
        {
            Debug.LogWarning($"[ZONE] Player is in gap, using closest zone: {closestZone.Config.zoneName} (distance: {closestDistance:F2}m)");
        }
        
        return closestZone;
    }
    
    /// <summary>
    /// Una puerta comenzó a abrirse
    /// ← DISPARA DESDE: Door.Open()
    /// </summary>
    public void OnDoorStartedOpening(Door door)
    {
        if (door == null)
            return;
        
        DynamicZone destZone = door.GetDestinationZone(currentZone);
        
        if (destZone != null)
        {
            if (debugMode)
                Debug.Log($"[ZONE] Door opened to {destZone.Config.zoneName}, pre-loading...");
            
            // PRE-CARGAR: Cambiar a estado Adjacent
            destZone.SetZoneState(ZoneState.Known);
            
            OnZonePreloaded?.Invoke(destZone);
        }
    }
    
    /// <summary>
    /// Un ascensor fue llamado
    /// ← DISPARA DESDE: Elevator.CallFloor()
    /// </summary>
    public void OnElevatorCalled(Elevator elevator, int floorNumber)
    {
        if (elevator == null)
            return;
        
        List<DynamicZone> floorZones = elevator.GetZonesOnFloor(floorNumber);
        
        if (debugMode)
            Debug.Log($"[ZONE] Elevator called to floor {floorNumber}, pre-loading {floorZones.Count} zones");
        
        // Pre-cargar todas las zonas del piso destino
        foreach (var zone in floorZones)
        {
            zone.SetZoneState(ZoneState.Known);
        }
    }
    
    /// <summary>
    /// El estado de una puerta cambió (abierta/cerrada)
    /// ← DISPARA DESDE: Door.OnDoorOpened / OnDoorClosed
    /// </summary>
    public void OnDoorStateChanged(Door door, bool isNowOpen)
    {
        if (debugMode)
            Debug.Log($"[ZONE] Door state changed: {door.Config.doorName} is now {(isNowOpen ? "OPEN" : "CLOSED")}");
        
        // Si la puerta está en una zona visible, reconstruir grafo
        if (currentZone != null)
        {
            RebuildZoneGraph(currentZone);
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // LÓGICA CENTRAL: Construir y actualizar grafo
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Reconstruir el grafo de zonas desde la zona actual
    /// </summary>
    private void RebuildZoneGraph(DynamicZone fromZone)
    {
        // Reconstruir cuando:
        // 1. El jugador entra en una zona nueva
        // 2. Cambian las puertas (invalidando el cache)
        if (fromZone == null)
            return;
        
        if (debugMode)
            Debug.Log($"[GRAPH REBUILD] from {fromZone.Config.zoneName}", fromZone.gameObject);
        
        currentZoneGraph = new ZoneGraph();
        currentZoneGraph.Build(fromZone, maxGraphDepth);
        
        if (debugMode)
        {
            currentZoneGraph.DebugPrintZonesByDistance();
        }
        
        OnZoneGraphUpdated?.Invoke(currentZoneGraph);
    }
    
    /// <summary>
    /// Actualizar estado de TODAS las zonas basado en el grafo actual
    /// ESTA ES LA OPERACIÓN CRÍTICA: decide qué zonas cargar/descargar
    /// </summary>
    private void UpdateZoneVisibility()
    {
        if (currentZoneGraph == null)
            return;
        
        List<ZoneGraphNode> allNodes = currentZoneGraph.GetAllNodes();
        
        foreach (var node in allNodes)
        {
            ZoneState newState = DetermineZoneState(node);
            
            if (debugMode)
                Debug.Log($"[ZONE STATE] {node.zone.Config.zoneName}: dist={node.distanceFromCurrent}, visited={node.zone.IsDiscovered}, state={newState}");
            
            // Force apply even if state hasn't changed - ensures visibility is correctly applied
            node.zone.SetZoneState(newState, forceApply: true);
        }
        
        // Desactivar zonas NO EN el grafo (muy lejanas)
        foreach (var kvp in allZonesById)
        {
            DynamicZone zone = kvp.Value;
            
            if (currentZoneGraph.GetNode(zone) == null)
            {
                // Esta zona no está en el grafo → muy lejana
                zone.SetZoneState(ZoneState.Unknown, forceApply: true);
            }
        }
        
        if (debugMode)
            Debug.Log($"[ZONE] Updated visibility for {allNodes.Count} zones");
    }
    
    /// <summary>
    /// Determinar el estado correcto para una zona
    /// LÓGICA SIMPLIFICADA:
    /// - Starting zones: SIEMPRE Active
    /// - No visitada: SIEMPRE Unknown (desactivada)
    /// - Zona actual: SIEMPRE Active
    /// - Visitada + adyacente: Active
    /// - Visitada + lejana + visible: Wireframe
    /// - Visitada + lejana + no visible: Dormant
    /// </summary>
    private ZoneState DetermineZoneState(ZoneGraphNode node)
    {
        // REGLA 0: ¿Es una zona inicial?
        // Las zonas iniciales SIEMPRE están activas
        if (node.zone.Config.isStartingZone)
        {
            return ZoneState.Active;  // Zona inicial SIEMPRE activa
        }
        
        // REGLA 1: ¿Nunca fue visitada?
        // TODAS las zonas no visitadas están desactivadas, sin importar distancia
        if (!node.zone.IsDiscovered)
        {
            return ZoneState.Unknown;  // Oculta completamente
        }
        
        // REGLA 2: ¿Es la zona actual (Player está aquí)?
        if (node.IsCurrent)
        {
            return ZoneState.Active;
        }
        
        // REGLA 3: ¿Es adyacente (distancia <= 2) Y fue visitada?
        // Las zonas cercanas visitadas SIEMPRE están activas, incluso si salen del frustum
        // (el zoom OUT podría hacer desaparecer el corredor si usamos solo frustum)
        if (node.distanceFromCurrent <= 2 && node.zone.IsDiscovered)
        {
            if (debugMode)
                Debug.Log($"[ZONE STATE] {node.zone.Config.zoneName}: Distance={node.distanceFromCurrent} + Visited = ACTIVE (visible siempre)");
            return ZoneState.Active;
        }
        
        // Zonas cercanas NO visitadas = Unknown
        if (node.distanceFromCurrent <= 2 && !node.zone.IsDiscovered)
        {
            if (debugMode)
                Debug.Log($"[ZONE STATE] {node.zone.Config.zoneName}: Distance={node.distanceFromCurrent} but NOT visited = UNKNOWN");
            return ZoneState.Unknown;
        }
        
        // REGLA 4: ¿Tiene acción activa (enemigos vivos)?
        if (node.zone.HasActiveEnemies)
        {
            return ZoneState.Active;  // Mantener activa mientras haya enemigos
        }
        
        // REGLA 5: Visitada, lejana (dist >= 2) - elegir entre Wireframe o Dormant
        // Wireframe si está visible en cámara, Dormant si no
        if (IsZoneInCamera(node.zone))
        {
            return ZoneState.Wireframe;
        }
        else
        {
            return ZoneState.Dormant;
        }
    }
    
    /// <summary>
    /// ¿La zona está visible en la cámara?
    /// </summary>
    private bool IsZoneInCamera(DynamicZone zone)
    {
        CameraController cam = CameraController.Instance;
        if (cam == null)
            return false;
        
        return cam.IsZoneInCamera(zone);
    }
    
    // ════════════════════════════════════════════════════════════════
    // QUERIES - Información sobre zonas
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Obtener la zona actual
    /// </summary>
    public DynamicZone GetCurrentZone()
    {
        return currentZone;
    }
    
    /// <summary>
    /// Obtener el grafo actual
    /// </summary>
    public ZoneGraph GetCurrentGraph()
    {
        return currentZoneGraph;
    }
    
    /// <summary>
    /// Obtener zona por ID
    /// </summary>
    public DynamicZone GetZoneById(int zoneId)
    {
        allZonesById.TryGetValue(zoneId, out var zone);
        return zone;
    }
    
    /// <summary>
    /// Obtener zona por nombre
    /// </summary>
    public DynamicZone GetZoneByName(string zoneName)
    {
        return allZonesById.Values.FirstOrDefault(z => z.Config.zoneName == zoneName);
    }
    
    /// <summary>
    /// Obtener la zona que contiene un punto
    /// </summary>
    public DynamicZone GetZoneAtPoint(Vector3 point)
    {
        foreach (var zone in allZonesById.Values)
        {
            if (zone.ContainsPoint(point))
                return zone;
        }
        return null;
    }
    
    /// <summary>
    /// ¿Hay línea de vista entre dos zonas?
    /// (útil para puzzles, visibilidad IA, etc.)
    /// </summary>
    public bool HasLineOfSight(DynamicZone zoneA, DynamicZone zoneB)
    {
        ZoneGraphNode nodeB = currentZoneGraph?.GetNode(zoneB);
        if (nodeB == null)
            return false;
        
        // Hay línea de vista si la zona está accesible (sin puertas cerradas)
        return !nodeB.IsInaccessible;
    }
    
    // ════════════════════════════════════════════════════════════════
    // DEBUG
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Mostrar toda la información de zonas en consola
    /// </summary>
    public void DebugPrintState()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════╗");
        Debug.Log("║          ZONE MANAGER STATE                           ║");
        Debug.Log("╚═══════════════════════════════════════════════════════╝");
        
        Debug.Log($"Current Zone: {(currentZone != null ? currentZone.Config.zoneName : "NONE")}");
        Debug.Log($"Previous Zone: {(previousZone != null ? previousZone.Config.zoneName : "NONE")}");
        Debug.Log($"Total Zones: {allZonesById.Count}");
        
        if (currentZoneGraph != null)
        {
            Debug.Log($"Graph Nodes: {currentZoneGraph.GetAllNodes().Count}");
            
            var adjacent = currentZoneGraph.GetAdjacentNodes();
            if (adjacent.Count > 0)
            {
                Debug.Log($"Adjacent Zones: {string.Join(", ", adjacent.ConvertAll(n => n.zone.Config.zoneName))}");
            }
            
            var nearby = currentZoneGraph.GetNearbyNodes();
            if (nearby.Count > 0)
            {
                Debug.Log($"Nearby Zones: {string.Join(", ", nearby.ConvertAll(n => n.zone.Config.zoneName))}");
            }
        }
        
        Debug.Log("══════════════════════════════════════════════════════════");
    }
    
    /// <summary>
    /// Mostrar estructura completa del grafo
    /// </summary>
    public void DebugPrintGraphStructure()
    {
        if (currentZoneGraph != null)
        {
            currentZoneGraph.DebugPrintStructure();
        }
    }
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (!drawGizmos || currentZoneGraph == null)
            return;
        
        // Dibujar líneas entre zonas relacionadas
        List<ZoneGraphNode> nodes = currentZoneGraph.GetAllNodes();
        
        foreach (var node in nodes)
        {
            if (node.parentNode != null)
            {
                Gizmos.color = node.IsAdjacent ? Color.green : Color.yellow;
                Gizmos.DrawLine(
                    node.zone.Config.zoneCenter,
                    node.parentNode.zone.Config.zoneCenter
                );
            }
        }
    }
    
    #endif
}

// ════════════════════════════════════════════════════════════════════════════════
// ZONE MANAGER VISUALIZER - Para debugging en editor
// ════════════════════════════════════════════════════════════════════════════════

[ExecuteInEditMode]
public class ZoneManagerVisualizer : MonoBehaviour
{
    [SerializeField] private bool showZoneInfo = false;
    [SerializeField] private bool showGraphStructure = false;
    [SerializeField] private bool autoRefresh = true;
    
    private float lastUpdateTime = 0f;
    [SerializeField]
    private float updateInterval = 0.5f;
    
    private void Update()
    {
        if (!Application.isPlaying)
            return;
        
        if (!autoRefresh)
            return;
        
        if (Time.realtimeSinceStartup - lastUpdateTime < updateInterval)
            return;
        
        lastUpdateTime = Time.realtimeSinceStartup;
        
        if (showZoneInfo)
        {
            ZoneManager.Instance?.DebugPrintState();
        }
        
        if (showGraphStructure)
        {
            ZoneManager.Instance?.DebugPrintGraphStructure();
        }
    }
    
    public void PrintState()
    {
        ZoneManager.Instance?.DebugPrintState();
    }
    
    public void PrintGraphStructure()
    {
        ZoneManager.Instance?.DebugPrintGraphStructure();
    }
}
