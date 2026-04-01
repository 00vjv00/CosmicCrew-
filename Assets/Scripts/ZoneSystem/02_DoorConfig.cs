// ============================================================================
// DOOR CONFIG - Estados, tipos y configuración de puertas
// Cosmic Crew - Sistema de Zonas Event-Driven
// ============================================================================

using UnityEngine;

/// <summary>
/// Estados posibles de una puerta
/// </summary>
public enum DoorState 
{
    Closed,       // Cerrada (bloqueada)
    Opening,      // Animación de apertura en progreso
    Open,         // Abierta
    Closing,      // Animación de cierre en progreso
    Locked,       // Cerrada con llave (no se puede abrir)
    Broken        // Rota (se abre sola o está destruida)
}

/// <summary>
/// Tipo de puerta (para audio y lógica diferente)
/// </summary>
public enum DoorType
{
    Sliding,      // Puerta de ascensor (rápida)
    Swing,        // Puerta normal (más lenta)
    Blast,        // Puerta de seguridad (lenta y fuerte)
    Emergency,    // Puerta de emergencia (rápida pero visible)
    BreakableWall // Pared que puede romperse
}

/// <summary>
/// Configuración de una puerta
/// </summary>
[System.Serializable]
public class DoorConfig
{
    [Header("Identificación")]
    public string doorName = "Door";
    public int doorID = 0;
    public DoorType doorType = DoorType.Sliding;
    
    [Header("Conexiones")]
    public DynamicZone zoneA;           // Zona de un lado
    public DynamicZone zoneB;           // Zona del otro lado
    [SerializeField]
    public Vector3 positionA = Vector3.zero;  // Puerta lado A
    [SerializeField]
    public Vector3 positionB = Vector3.zero;  // Puerta lado B (generalmente puerta cerrada)
    
    [Header("Física")]
    public float openDuration = 0.3f;   // Tiempo en abrirse
    public float closeDuration = 0.3f;  // Tiempo en cerrarse
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Comportamiento")]
    public float autoCloseDuration = 5f;  // Cerrar auto después N segundos (-1 = no cerrar)
    public bool canBeForcedOpen = false;  // ¿Stone puede romper?
    public bool requiresKey = false;      // ¿Necesita llave?
    public int keyRequired = 0;           // ID de llave requerida
    
    [Header("Audio & FX")]
    public bool playSoundOnOpen = true;
    public bool playSoundOnClose = true;
    public string openSoundClip = "door_open";
    public string closeSoundClip = "door_close";
}
