// ============================================================================
// COSMIC CREW - GLOBAL DEFINITIONS
// Archivo: Assets/Scripts/Core/GlobalDefinitions.cs
// Descripción: Enums, tipos y constantes globales del proyecto
// ============================================================================

using UnityEngine;

// ============================================================================
// IMPORTANTE: Este archivo SOLO contiene enums y constantes
// NO debe tener clases que hereden de MonoBehaviour
// NO lo agregues a ningún GameObject
// ============================================================================

#region CHARACTER POWERS

/// <summary>
/// Todos los poderes disponibles en el juego
/// Se dividen en poderes ofensivos y defensivos por personaje
/// </summary>
public enum CharacterPower
{
    // STONE - Piedra (Fuerza física)
    STONE_PUNCH,        // Puñetazo (ofensivo)
    STONE_WALL,         // Muralla defensiva (defensivo)
    
    // LUMEN - Energía (Invisibilidad)
    LUMEN_WAVE,         // Onda de energía (ofensivo)
    LUMEN_ABSORB,       // Absorber energía (defensivo)
    
    // FOLD - Geométrico (Origami)
    FOLD_TELEPORT,      // Teletransporte (ofensivo)
    FOLD_JUMP,          // Salto vertical (defensivo)
    
    // NUBE - Gaseoso (Densidad)
    NUBE_PRESSURE,      // Onda de presión (ofensivo)
    NUBE_GAS,           // Convertirse en gas (defensivo)
    
    // ARAC - Araña Cibernética
    ARAC_NET,           // Lanzar red (ofensivo)
    ARAC_NANOBOTS,      // Nanobots (defensivo)
    
    // Especial
    NONE                // Sin poder
}

/// <summary>
/// Identificador del personaje/carácter
/// </summary>
public enum CharacterType
{
    STONE,              // Piedra - Fuerza
    LUMEN,              // Energía - Invisibilidad
    FOLD,               // Geométrico - Origami
    NUBE,               // Gaseoso - Densidad
    ARAC                // Araña Cibernética
}

/// <summary>
/// Nivel de poder ejecutado (según presión del botón)
/// </summary>
public enum PowerLevel
{
    WEAK,               // Presión corta - bajo costo
    NORMAL,             // Presión media - costo normal
    STRONG              // Presión larga - alto costo/riesgo
}

#endregion

#region GAME STATE

/// <summary>
/// Estados principales del juego
/// </summary>
public enum GameState
{
    MENU,               // Menú principal
    LOADING,            // Cargando nivel
    PLAYING,            // Jugando
    PAUSED,             // Pausado
    GAME_OVER,          // Fin del juego (derrota)
    WIN,                // Victoria
    CUTSCENE            // Cinemática
}

/// <summary>
/// Estados del jugador en el juego
/// </summary>
public enum PlayerState
{
    IDLE,               // Quieto
    MOVING,             // Moviendo
    CASTING_POWER,      // Lanzando poder
    IN_AIR,             // En el aire
    STUNNED,            // Aturdido
    DEAD                // Muerto
}

/// <summary>
/// Estados de salud del jugador
/// </summary>
public enum HealthState
{
    FULL,               // Salud completa
    DAMAGED,            // Dañado
    CRITICAL,           // Crítico (poco health)
    DEAD                // Muerto
}

#endregion

#region LEVEL/SECTOR PROGRESSION

/// <summary>
/// Estados de descubrimiento de zonas
/// </summary>
public enum ZoneState
{
    Unknown,
    Known,
    Active,
    Dormant,
    Wireframe
}

/// <summary>
/// Tipos de zonas/espacios
/// </summary>
public enum ZoneType
{
    ROOM,               // Sala estándar
    CORRIDOR,           // Corredor
    SHORTCUT,           // Atajo (requiere poder específico)
    LABYRINTH,          // Laberinto procedural
    BOSS_ARENA,         // Arena de jefe
    PUZZLE_CHAMBER,      // Cámara de puzzle complejo
    STAIRS,             // Escalera (conecta pisos)
    ELEVATOR,            // Ascensor (conecta pisos)
    SAFE_ROOM            // Zona segura (sin enemigos)
}

/// <summary>
/// Tipos de salas (para generación de contenido)
/// </summary>
public enum RoomType
{
    ENTRANCE,           // Entrada a plano
    COMBAT,             // Sala de combate
    EXPLORATION,        // Exploración
    PUZZLE,             // Puzzle
    BOSS,               // Jefe
    SAFE_ROOM,          // Sala segura
    TREASURE            // Tesoro/reward
}

/// <summary>
/// Sectores de la estación
/// </summary>
public enum SectorType
{
    ALTA_SEGURIDAD,     // Sector 0
    HABITAT,            // Sector 1
    NAVEGACION,         // Sector 2
    HANGAR,             // Sector 3
    ALMACEN,            // Sector 4
    INGENIERIA          // Sector 5
}

#endregion

#region INTERACTION & GAMEPLAY

/// <summary>
/// Tipos de enemigos en el juego
/// </summary>
public enum EnemyType
{
    ROBOT_GUARD,        // Guardia robótico (patrulla)
    TURRET,             // Torreta (defensiva)
    DRONE,              // Dron (explorador)
    HEAVY_UNIT,         // Unidad pesada (combate fuerte)
    BOSS                // Jefe
}

/// <summary>
/// Estados de un enemigo
/// </summary>
public enum EnemyState
{
    IDLE,               // Inactivo
    PATROL,             // Patrullando
    ALERT,              // Alerta (ve al player)
    PURSUING,           // Persiguiendo
    ATTACKING,          // Atacando
    DEAD                // Muerto
}

/// <summary>
/// Tipos de objetivos/misiones
/// </summary>
public enum ObjectiveType
{
    ESCAPE,             // Escapar del sector
    COLLECT_ITEM,       // Recolectar objeto
    DEFEAT_ENEMIES,     // Derrotar enemigos
    SOLVE_PUZZLE,       // Resolver puzzle
    REACH_LOCATION,     // Llegar a ubicación
    UNLOCK_DOOR,        // Desbloquear puerta
    DISABLE_SYSTEM      // Desactivar sistema
}

/// <summary>
/// Estados de un objetivo
/// </summary>
public enum ObjectiveState
{
    NOT_STARTED,        // No iniciado
    IN_PROGRESS,        // En progreso
    COMPLETED,          // Completado
    FAILED              // Fallido
}

#endregion

#region DIFFICULTY & PROGRESSION

/// <summary>
/// Dificultad del juego
/// </summary>
public enum Difficulty
{
    EASY,               // Fácil
    NORMAL,             // Normal
    HARD,               // Difícil
    IMPOSSIBLE          // Imposible
}

/// <summary>
/// Progresión de desbloqueo de personajes
/// </summary>
public enum CharacterUnlockStatus
{
    LOCKED,             // Bloqueado
    UNLOCKING,          // En proceso de desbloqueo
    UNLOCKED,           // Desbloqueado
    MASTERED            // Completamente dominado
}

#endregion

#region AUDIO & PARTICLES

/// <summary>
/// Canales de audio (para AudioMixer)
/// </summary>
public enum AudioChannel
{
    MASTER,             // Volumen general
    MUSIC,              // Música de fondo
    SFX,                // Efectos de sonido
    AMBIENT,            // Ambiente (maquinaria, etc)
    VOICE               // Voces/diálogos
}

/// <summary>
/// Tipos de efectos visuales
/// </summary>
public enum ParticleEffectType
{
    POWER_CAST,         // Lanzar poder
    POWER_HIT,          // Impacto de poder
    EXPLOSION,          // Explosión
    TELEPORT,           // Teletransporte
    HEAL,               // Curación
    DAMAGE,             // Daño recibido
    SHIELD,             // Escudo
    ENERGY_WAVE         // Onda de energía
}

#endregion

#region CONSTANTS

/// <summary>
/// Constantes globales del juego
/// NO ES MONOBEHAVIOUR - Solo contiene constantes estáticas
/// </summary>
public static class GameConstants
{
    // DIMENSIONES
    public const float TILE_SIZE = 6f;              // Tamaño de un tile (6x6 unidades)
    public const float FLOOR_HEIGHT = 4f;           // Alto de cada piso
    public const int TOTAL_FLOORS = 6;              // 6 pisos por plano
    public const int TOTAL_PLANES = 20;             // 20 planos por sector
    public const int TOTAL_SECTORS = 6;             // 6 sectores
    public const int TOTAL_PLANES_STATION = TOTAL_PLANES * TOTAL_SECTORS;  // 120 planos
    
    // ROTACIÓN
    public const float SECTOR_ROTATION = 60f;       // 360 / 6 = 60° por sector
    public const float PLANE_ROTATION_STEP = 3f;    // 60 / 20 = 3° por plano
    public const float CYLINDER_RADIUS = 30f;       // Radio del cilindro
    
    // FÍSICA
    public const float GRAVITY_STRENGTH = 9.81f;    // Aceleración de gravedad artificial
    public const float PLAYER_MOVE_SPEED = 5f;      // Velocidad de movimiento del player
    public const float PLAYER_CAPSULE_RADIUS = 0.3f; // Radio del capsule collider
    public const float PLAYER_CAPSULE_HEIGHT = 2f;  // Alto del capsule collider
    
    // CÁMARA
    public const float CAMERA_FOV = 60f;            // Field of view de cámara isométrica
    public const float CAMERA_DISTANCE = 10f;       // Distancia base de la cámara
    public const float CAMERA_HEIGHT_OFFSET = 5.2f; // Altura de la cámara
    
    // ROTACIÓN ESTACIÓN
    public const float STATION_BASE_ROTATION_SPEED = 10f;    // °/segundo
    public const float STATION_MAX_ROTATION_SPEED = 30f;     // Máxima rotación
    
    // ZONAS
    public const float ZONE_VISIBILITY_DISTANCE = 50f;       // Distancia de visibilidad
    public const float ZONE_PRELOAD_DISTANCE = 30f;          // Distancia de pre-carga
    
    // PODERES
    public const float POWER_COOLDOWN = 0.5f;      // Cooldown base entre poderes
    public const float POWER_CAST_TIME = 0.3f;     // Tiempo de lanzamiento
    
    // ENEMIGOS
    public const float ENEMY_PATROL_SPEED = 3f;    // Velocidad de patrulla
    public const float ENEMY_PURSUE_SPEED = 6f;    // Velocidad de persecución
    public const float ENEMY_DETECT_RANGE = 20f;   // Rango de detección
}

#endregion

#region HELPER EXTENSIONS

/// <summary>
/// Extensiones útiles para enums
/// NO MONOBEHAVIOUR - Solo métodos estáticos
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Obtener nombre legible del poder
    /// </summary>
    public static string GetDisplayName(this CharacterPower power)
    {
        return power switch
        {
            CharacterPower.STONE_PUNCH => "Puñetazo",
            CharacterPower.STONE_WALL => "Muralla",
            CharacterPower.LUMEN_WAVE => "Onda de Energía",
            CharacterPower.LUMEN_ABSORB => "Absorber Energía",
            CharacterPower.FOLD_TELEPORT => "Teletransporte",
            CharacterPower.FOLD_JUMP => "Salto Vertical",
            CharacterPower.NUBE_PRESSURE => "Onda de Presión",
            CharacterPower.NUBE_GAS => "Forma Gaseosa",
            CharacterPower.ARAC_NET => "Lanzar Red",
            CharacterPower.ARAC_NANOBOTS => "Nanobots",
            _ => "Desconocido"
        };
    }
    
    /// <summary>
    /// Obtener personaje asociado a un poder
    /// </summary>
    public static CharacterType GetCharacter(this CharacterPower power)
    {
        return power switch
        {
            CharacterPower.STONE_PUNCH or CharacterPower.STONE_WALL => CharacterType.STONE,
            CharacterPower.LUMEN_WAVE or CharacterPower.LUMEN_ABSORB => CharacterType.LUMEN,
            CharacterPower.FOLD_TELEPORT or CharacterPower.FOLD_JUMP => CharacterType.FOLD,
            CharacterPower.NUBE_PRESSURE or CharacterPower.NUBE_GAS => CharacterType.NUBE,
            CharacterPower.ARAC_NET or CharacterPower.ARAC_NANOBOTS => CharacterType.ARAC,
            _ => CharacterType.STONE
        };
    }
    
    /// <summary>
    /// Comprobar si es un poder ofensivo
    /// </summary>
    public static bool IsOffensive(this CharacterPower power)
    {
        return power switch
        {
            CharacterPower.STONE_PUNCH or
            CharacterPower.LUMEN_WAVE or
            CharacterPower.FOLD_TELEPORT or
            CharacterPower.NUBE_PRESSURE or
            CharacterPower.ARAC_NET => true,
            _ => false
        };
    }
    
    /// <summary>
    /// Comprobar si es un poder defensivo
    /// </summary>
    public static bool IsDefensive(this CharacterPower power)
    {
        return power switch
        {
            CharacterPower.STONE_WALL or
            CharacterPower.LUMEN_ABSORB or
            CharacterPower.FOLD_JUMP or
            CharacterPower.NUBE_GAS or
            CharacterPower.ARAC_NANOBOTS => true,
            _ => false
        };
    }
}

#endregion
