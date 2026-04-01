// ============================================================================
// ZONE SYSTEM - SHARED CONFIGURATION
// Cosmic Crew - Datos compartidos entre todos los sistemas de zonas
// ============================================================================

using UnityEngine;
using System.Collections.Generic;


#region DATA STRUCTURES - Configuración

/// <summary>
/// Información sobre conexión entre zonas
/// </summary>
[System.Serializable]
public class ZoneConnection
{
    public DynamicZone connectedZone;
    [Range(0, 10)]
    public float connectionDistance = 1f;  // Distancia en tiles (6m cada uno)
    public Vector3 connectionPoint = Vector3.zero;
}

/// <summary>
/// Configuración completa de una zona dinámica
/// </summary>
[System.Serializable]
public class DynamicZoneConfig
{
    [Header("═ IDENTIFICACIÓN ═")]
    public string zoneName = "Nueva Zona";
    public int zoneID = 0;
    public int floorLevel = 0;
    public bool isStartingZone = false;  // ¿Es la zona inicial donde empieza el player?
    
    [Header("═ GEOMETRÍA ═")]
    public Vector3 zoneCenter = Vector3.zero;
    public Vector3 zoneSize = Vector3.one * 6f;  // 6m x 6m por defecto
    public ZoneType zoneType = ZoneType.ROOM;
    
    [Header("═ CONTENIDO ═")]
    public GameObject zoneRoot;  // GameObject padre de toda la zona
    public ZoneConnection[] connections = new ZoneConnection[0];  // Zonas conectadas
    
    [Header("═ OPTIMIZACIÓN ═")]
    [Range(5, 500)]
    public int maxVisibleDistance = 50;  // En metros
    public bool preloadAdjacent = true;
    public float preloadDelay = 0.2f;    // Segundos para activar zona adyacente
    
    [Header("═ ENEMIGOS ═")]
    public bool spawnEnemiesOnActivate = true;
    public int numEnemies = 2;
    public GameObject enemyPrefab;  // Prefab del enemigo a spawnear
    public Vector3 enemySpawnAreaOffset = Vector3.zero;
    
    [Header("═ EVENTOS ═")]
    public bool triggerCutsceneOnDiscover = false;
    public string cutsceneClipName = "";
}

#endregion

#region INTERFACES - Contrato para componentes

/// <summary>
/// Interface para cualquier componente que pueda streamear (activar/desactivar)
/// </summary>
public interface IStreamable
{
    void SetStreaming(bool active);
}

#endregion

// ============================================================================
// EXTENSION METHODS
// ============================================================================

public static class ZoneStateExtensions
{
    public static bool IsActive(this ZoneState state)
    {
        return state == ZoneState.Active;
    }
    
    public static bool IsVisible(this ZoneState state)
    {
        // Visible = cualquier cosa excepto Unknown y Dormant
        return state != ZoneState.Unknown && state != ZoneState.Dormant;
    }
    
    public static bool IsLoaded(this ZoneState state)
    {
        // Cargada en memoria si no es Unknown
        return state != ZoneState.Unknown;
    }
    
    public static string GetDescription(this ZoneState state)
    {
        return state switch
        {
            ZoneState.Unknown => "Sin descubrir (completamente desactivada)",
            ZoneState.Wireframe => "Visitada, lejana, visible en cámara (solo líneas)",
            ZoneState.Dormant => "Visitada, lejana, fuera de cámara (apagada)",
            ZoneState.Active => "Activa (Player aquí, adyacente, o con acción)",
            _ => "Desconocido"
        };
    }
}

public static class ZoneTypeExtensions
{
    public static bool HasEnemies(this ZoneType type)
    {
        return type switch
        {
            ZoneType.SAFE_ROOM => false,
            ZoneType.PUZZLE_CHAMBER => false,
            _ => true
        };
    }
    
    public static string GetDescription(this ZoneType type)
    {
        return type switch
        {
            ZoneType.ROOM => "Sala normal",
            ZoneType.CORRIDOR => "Corredor",
            ZoneType.STAIRS => "Escalera",
            ZoneType.ELEVATOR => "Ascensor",
            ZoneType.SHORTCUT => "Atajo",
            ZoneType.LABYRINTH => "Laberinto",
            ZoneType.BOSS_ARENA => "Arena de jefe",
            ZoneType.PUZZLE_CHAMBER => "Cámara de puzzle",
            ZoneType.SAFE_ROOM => "Zona segura",
            _ => "Desconocido"
        };
    }
}
