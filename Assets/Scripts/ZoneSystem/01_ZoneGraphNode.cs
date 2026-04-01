// ============================================================================
// ZONE GRAPH NODE - Estructura jerárquica de relación entre zonas
// Cosmic Crew - Sistema de Zonas Event-Driven
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Representa un nodo en el grafo de zonas
/// Define la distancia y accesibilidad desde una zona raíz (actual)
/// </summary>
[System.Serializable]
public class ZoneGraphNode
{
    /// <summary>
    /// La zona que este nodo representa
    /// </summary>
    public DynamicZone zone;
    
    /// <summary>
    /// Distancia en "pasos de conexión" desde la zona actual
    /// 0 = zona actual (currentZone)
    /// 1 = adyacente directo
    /// 2 = a través de 1 zona intermedia
    /// 3+ = lejano
    /// </summary>
    public int distanceFromCurrent = 0;
    
    /// <summary>
    /// ¿Esta zona tiene puertas cerradas en el camino desde la actual?
    /// Si hay puertas cerradas, la zona se ve pero no se pre-carga
    /// </summary>
    public List<Door> blockedDoors = new();
    
    /// <summary>
    /// ¿Se puede ver esta zona desde la zona actual?
    /// (importante para fog of war y visibilidad)
    /// </summary>
    public bool isVisibleFromCurrent = true;
    
    /// <summary>
    /// Costo de transición (para pathing de IA o puzzle logic)
    /// Por defecto = 1 (una conexión)
    /// Aumenta si hay obstáculos
    /// </summary>
    public float pathCost = 1f;
    
    /// <summary>
    /// Parent en el árbol BFS (para reconstruir camino)
    /// Útil para debug y para seguimiento de ruta
    /// </summary>
    public ZoneGraphNode parentNode = null;
    
    /// <summary>
    /// Todos los nodos secundarios (zonas que se alcanzan desde este)
    /// </summary>
    public List<ZoneGraphNode> childNodes = new();
    
    // ────────────────────────────────────────────────────────
    // GETTERS - Información útil sobre el nodo
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// ¿Es esta la zona actual (distancia 0)?
    /// </summary>
    public bool IsCurrent => distanceFromCurrent == 0;
    
    /// <summary>
    /// ¿Es esta zona adyacente directa (distancia 1)?
    /// </summary>
    public bool IsAdjacent => distanceFromCurrent == 1;
    
    /// <summary>
    /// ¿Es esta zona cercana (distancia 2-3)?
    /// </summary>
    public bool IsNearby => distanceFromCurrent >= 2 && distanceFromCurrent <= 3;
    
    /// <summary>
    /// ¿Hay puertas cerradas bloqueando esta zona?
    /// </summary>
    public bool IsBlocked => blockedDoors.Count > 0;
    
    /// <summary>
    /// ¿Esta zona está inaccesible (hay puertas cerradas)?
    /// </summary>
    public bool IsInaccessible => blockedDoors.Count > 0 && 
                                  blockedDoors.TrueForAll(d => d != null && !d.IsOpen);
    
    // ────────────────────────────────────────────────────────
    // DEBUG HELPERS
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Mostrar información del nodo en consola
    /// </summary>
    public void DebugPrint(int indent = 0)
    {
        string prefix = new string(' ', indent * 2);
        string state = "";
        
        if (IsCurrent) state = "[CURRENT]";
        else if (IsAdjacent) state = "[ADJACENT]";
        else if (IsNearby) state = "[NEARBY]";
        else state = $"[DIST {distanceFromCurrent}]";
        
        string blocked = IsBlocked ? $" - BLOCKED ({blockedDoors.Count} doors)" : "";
        
        Debug.Log($"{prefix}{zone.Config.zoneName} {state} dist={distanceFromCurrent}{blocked}");
        
        foreach (var child in childNodes)
        {
            child.DebugPrint(indent + 1);
        }
    }
    
    /// <summary>
    /// Obtener cadena legible del nodo
    /// </summary>
    public override string ToString()
    {
        return $"ZoneNode({zone.Config.zoneName}, dist={distanceFromCurrent})";
    }
}

// ============================================================================
// ZONE GRAPH BUILDER - Construye el grafo usando BFS
// ============================================================================

/// <summary>
/// Construye un grafo de zonas a partir de una zona raíz
/// Usa BFS para calcular distancias y relaciones
/// </summary>
public class ZoneGraphBuilder
{
    /// <summary>
    /// Construir grafo jerárquico desde una zona raíz
    /// </summary>
    /// <param name="rootZone">La zona desde la que empezar (zona actual)</param>
    /// <param name="maxDistance">Profundidad máxima del BFS (para limitar performance)</param>
    /// <returns>El nodo raíz del grafo</returns>
    public static ZoneGraphNode BuildGraph(DynamicZone rootZone, int maxDistance = 4)
    {
        if (rootZone == null)
        {
            Debug.LogWarning("[ZONE GRAPH] Intento de buildear grafo con zona null");
            return null;
        }
        
        // Crear nodo raíz
        ZoneGraphNode rootNode = new ZoneGraphNode
        {
            zone = rootZone,
            distanceFromCurrent = 0,
            parentNode = null
        };
        
        // BFS para explorar todas las conexiones
        Queue<(ZoneGraphNode node, int depth)> queue = new();
        HashSet<DynamicZone> visited = new();
        
        queue.Enqueue((rootNode, 0));
        visited.Add(rootZone);
        
        while (queue.Count > 0)
        {
            var (currentNode, currentDepth) = queue.Dequeue();
            
            // No ir más allá de maxDistance
            if (currentDepth >= maxDistance)
                continue;
            
            // Explorar PUERTAS conectadas a esta zona
            DynamicZone[] connectedZones = GetConnectedZonesThroughDoors(currentNode.zone);
            
            foreach (var connectedZone in connectedZones)
            {
                if (connectedZone == null)
                    continue;
                
                // No revisitar zonas ya vistas
                if (visited.Contains(connectedZone))
                    continue;
                
                visited.Add(connectedZone);
                
                // Crear nodo para la zona conectada
                ZoneGraphNode childNode = new ZoneGraphNode
                {
                    zone = connectedZone,
                    distanceFromCurrent = currentDepth + 1,
                    parentNode = currentNode,
                    pathCost = currentNode.pathCost + 1f  // Costo base = 1 por puerta
                };
                
                // Detectar puertas bloqueadas en el camino
                childNode.blockedDoors = GetBlockedDoorsTo(connectedZone, rootZone);
                
                // Agregar como hijo
                currentNode.childNodes.Add(childNode);
                
                // Encolar para continuar BFS
                queue.Enqueue((childNode, currentDepth + 1));
            }
        }
        
        return rootNode;
    }
    
    /// <summary>
    /// Obtener todas las zonas conectadas a través de puertas
    /// </summary>
    private static DynamicZone[] GetConnectedZonesThroughDoors(DynamicZone zone)
    {
        List<DynamicZone> connectedZones = new();
        
        // Buscar TODAS las puertas en la escena
        Door[] allDoors = GameObject.FindObjectsByType<Door>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var door in allDoors)
        {
            if (door == null || door.Config == null)
                continue;
            
            DynamicZone otherZone = null;
            
            // Verificar si esta puerta conecta con la zona actual
            if (door.Config.zoneA == zone)
            {
                otherZone = door.Config.zoneB;
            }
            else if (door.Config.zoneB == zone)
            {
                otherZone = door.Config.zoneA;
            }
            else
            {
                continue;  // Esta puerta no conecta con la zona actual
            }
            
            // Agregar la zona conectada (sin duplicados)
            if (otherZone != null && !connectedZones.Contains(otherZone))
            {
                connectedZones.Add(otherZone);
                Debug.Log($"[ZONE GRAPH] Found connected zone: {zone.Config.zoneName} → {otherZone.Config.zoneName}");
            }
        }
        
        return connectedZones.ToArray();
    }
    
    /// <summary>
    /// Detectar puertas cerradas entre zona origen y destino
    /// </summary>
    private static List<Door> GetBlockedDoorsTo(DynamicZone destinationZone, DynamicZone sourceZone)
    {
        List<Door> blockedDoors = new();
        
        // Buscar todas las puertas en la zona destino
        Door[] doorsInZone = destinationZone.GetComponentsInChildren<Door>();
        
        foreach (var door in doorsInZone)
        {
            // Si la puerta está cerrada, agregarla a bloqueadas
            if (door != null && !door.IsOpen)
            {
                blockedDoors.Add(door);
            }
        }
        
        return blockedDoors;
    }
    
    /// <summary>
    /// Aplanar el árbol a una lista para iteración fácil
    /// </summary>
    public static void FlattenGraph(ZoneGraphNode rootNode, List<ZoneGraphNode> output)
    {
        if (rootNode == null)
            return;
        
        output.Add(rootNode);
        
        foreach (var child in rootNode.childNodes)
        {
            FlattenGraph(child, output);
        }
    }
    
    /// <summary>
    /// Obtener solo los nodos a una distancia específica
    /// </summary>
    public static List<ZoneGraphNode> GetNodesAtDistance(ZoneGraphNode rootNode, int distance)
    {
        List<ZoneGraphNode> result = new();
        GetNodesAtDistanceRecursive(rootNode, distance, result);
        return result;
    }
    
    private static void GetNodesAtDistanceRecursive(ZoneGraphNode node, int targetDistance, List<ZoneGraphNode> output)
    {
        if (node == null)
            return;
        
        if (node.distanceFromCurrent == targetDistance)
        {
            output.Add(node);
        }
        
        foreach (var child in node.childNodes)
        {
            GetNodesAtDistanceRecursive(child, targetDistance, output);
        }
    }
    
    /// <summary>
    /// Obtener todos los nodos hasta una distancia máxima
    /// </summary>
    public static List<ZoneGraphNode> GetNodesWithinDistance(ZoneGraphNode rootNode, int maxDistance)
    {
        List<ZoneGraphNode> result = new();
        GetNodesWithinDistanceRecursive(rootNode, maxDistance, result);
        return result;
    }
    
    private static void GetNodesWithinDistanceRecursive(ZoneGraphNode node, int maxDistance, List<ZoneGraphNode> output)
    {
        if (node == null)
            return;
        
        if (node.distanceFromCurrent <= maxDistance)
        {
            output.Add(node);
        }
        
        foreach (var child in node.childNodes)
        {
            GetNodesWithinDistanceRecursive(child, maxDistance, output);
        }
    }
}

// ============================================================================
// ZONE GRAPH - Wrapper que gestiona el grafo actual
// ============================================================================

/// <summary>
/// Mantiene el grafo actual de zonas construido desde la zona actual
/// Se actualiza cada vez que el player entra a una nueva zona
/// </summary>
public class ZoneGraph
{
    /// <summary>
    /// Nodo raíz (zona actual)
    /// </summary>
    public ZoneGraphNode rootNode { get; private set; }
    
    /// <summary>
    /// Versión cacheada del grafo aplanado (para iteración rápida)
    /// </summary>
    private List<ZoneGraphNode> flatNodes = new();
    
    /// <summary>
    /// Construir el grafo desde una zona
    /// </summary>
    public void Build(DynamicZone sourceZone, int maxDistance = 4)
    {
        rootNode = ZoneGraphBuilder.BuildGraph(sourceZone, maxDistance);
        
        // Aplanar para fácil acceso
        flatNodes.Clear();
        ZoneGraphBuilder.FlattenGraph(rootNode, flatNodes);
        
        Debug.Log($"[ZONE GRAPH] Constructed from {sourceZone.Config.zoneName} " +
                  $"({flatNodes.Count} zones, max distance {maxDistance})");
    }
    
    /// <summary>
    /// Obtener nodo para una zona específica
    /// </summary>
    public ZoneGraphNode GetNode(DynamicZone zone)
    {
        foreach (var node in flatNodes)
        {
            if (node.zone == zone)
                return node;
        }
        return null;
    }
    
    /// <summary>
    /// Obtener todas las zonas adyacentes (distancia 1)
    /// </summary>
    public List<ZoneGraphNode> GetAdjacentNodes()
    {
        return ZoneGraphBuilder.GetNodesAtDistance(rootNode, 1);
    }
    
    /// <summary>
    /// Obtener todas las zonas cercanas (distancia 2-3)
    /// </summary>
    public List<ZoneGraphNode> GetNearbyNodes()
    {
        List<ZoneGraphNode> result = new();
        result.AddRange(ZoneGraphBuilder.GetNodesAtDistance(rootNode, 2));
        result.AddRange(ZoneGraphBuilder.GetNodesAtDistance(rootNode, 3));
        return result;
    }
    
    /// <summary>
    /// Obtener todas las zonas conocidas (dentro de maxDistance)
    /// </summary>
    public List<ZoneGraphNode> GetAllKnownZones(int maxDistance = 3)
    {
        return ZoneGraphBuilder.GetNodesWithinDistance(rootNode, maxDistance);
    }
    
    /// <summary>
    /// Obtener todos los nodos aplanados
    /// </summary>
    public List<ZoneGraphNode> GetAllNodes()
    {
        return new List<ZoneGraphNode>(flatNodes);
    }
    
    /// <summary>
    /// Debuggear: mostrar estructura del grafo
    /// </summary>
    public void DebugPrintStructure()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("ZONE GRAPH STRUCTURE");
        Debug.Log("═══════════════════════════════════════════");
        rootNode?.DebugPrint();
        Debug.Log("═══════════════════════════════════════════");
    }
    
    /// <summary>
    /// Debuggear: listar todas las zonas por distancia
    /// </summary>
    public void DebugPrintZonesByDistance()
    {
        Debug.Log("ZONES BY DISTANCE:");
        
        for (int dist = 0; dist <= 4; dist++)
        {
            var nodes = ZoneGraphBuilder.GetNodesAtDistance(rootNode, dist);
            if (nodes.Count > 0)
            {
                Debug.Log($"  Distance {dist}: {string.Join(", ", nodes.ConvertAll(n => n.zone.Config.zoneName))}");
            }
        }
    }
}
