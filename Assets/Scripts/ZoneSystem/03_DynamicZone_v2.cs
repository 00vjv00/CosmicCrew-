// ============================================================================
// DYNAMIC ZONE (VERSIÓN 2) - Event-driven con triggers de entrada/salida
// Cosmic Crew - Sistema de Zonas Event-Driven
// La dynamic zone se asigna a nivel sector, que tiene referencia al zonemanager 
// y hay otos planos / salas instanciados,
// NO EN EL PREFAB DE LA ZONA, donde no se pueden hacer conexiones con otras zonas
// ============================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Modos de renderizado de una zona
/// </summary>
public enum RenderMode
{
    Hidden,    // Completamente oculto
    Wireframe, // Solo líneas/estructura
    Full       // Renderizado completo
}

/// <summary>
/// Representa una zona individual del juego (sala, corredor, escalera)
/// Versión 2: Event-driven, dispara eventos cuando player entra/sale
/// </summary>
public class DynamicZone : MonoBehaviour
{
    [SerializeField] private DynamicZoneConfig config;
    
    // ────────────────────────────────────────────────────────
    // ESTADO INTERNO
    // ────────────────────────────────────────────────────────
    
    private ZoneState currentState = ZoneState.Unknown;
    private bool isDiscovered = false;    // ¿Player visitó alguna vez?
    private bool isLoaded = false;        // ¿Está cargada en memoria?
    private List<Transform> spawnedEnemies = new();
    private float activationTime = 0f;
    
    // ────────────────────────────────────────────────────────
    // REFERENCIAS INTERNAS
    // ────────────────────────────────────────────────────────
    
    private Collider zoneTrigger;         // El trigger principal
    private Collider[] allZoneTriggers;   // Todos los triggers (para salas multi-plano)
    private int activeTriggerCount = 0;   // Contador de triggers activos (player dentro de N triggers)
    private ZoneManager zoneManager;
    private List<Door> connectedDoors = new();
    internal int distanceInCurrentGraph = int.MaxValue;  // Distancia en el grafo del jugador (settable por ZoneManager)
    
    // ────────────────────────────────────────────────────────
    // FADE-IN ANIMATION
    // ────────────────────────────────────────────────────────
    
    private Coroutine fadeInCoroutine;
    private const float FADE_IN_DURATION = 1.5f;  // segundos para desvanecimiento completo
    
    // ────────────────────────────────────────────────────────
    // EVENTOS - Se disparan cuando algo importante ocurre
    // ────────────────────────────────────────────────────────
    
    /// <summary>
    /// Se dispara cuando el state cambia
    /// </summary>
    public System.Action<DynamicZone, ZoneState, ZoneState> OnStateChanged;
    
    /// <summary>
    /// Se dispara cuando se descubre la zona por PRIMERA VEZ
    /// </summary>
    public System.Action<DynamicZone> OnFirstDiscovered;
    
    /// <summary>
    /// Se dispara cuando el player entra a la zona físicamente
    /// </summary>
    public System.Action<DynamicZone> OnPlayerEntered;
    
    /// <summary>
    /// Se dispara cuando el player sale de la zona
    /// </summary>
    public System.Action<DynamicZone> OnPlayerExited;
    
    /// <summary>
    /// Se dispara cuando la zona se activa (state = Active)
    /// </summary>
    public System.Action<DynamicZone> OnZoneActivated;
    
    /// <summary>
    /// Se dispara cuando la zona se desactiva (state != Active)
    /// </summary>
    public System.Action<DynamicZone> OnZoneDeactivated;
    
    // ────────────────────────────────────────────────────────
    // PROPERTIES - Acceso a información
    // ────────────────────────────────────────────────────────
    
    public DynamicZoneConfig Config => config;
    public ZoneState CurrentState => currentState;
    public bool IsActive => currentState == ZoneState.Active;
    public bool IsDiscovered => isDiscovered;
    public bool IsLoaded => isLoaded;
    public bool IsVisible => currentState != ZoneState.Unknown;
    public bool HasActiveEnemies => spawnedEnemies.Count > 0;  // ← Nueva propiedad
    public int GetActiveTriggerCount => activeTriggerCount;    // ← Para debug de salas duales
    public bool IsMultiPlano => allZoneTriggers?.Length > 1;   // ← Detectar si es sala compartida
    public int DistanceInCurrentGraph => distanceInCurrentGraph;  // ← Distancia en el grafo actual
    
    // ────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────
    
    private void Awake()
    {
        // Configurar referencias
        if (config.zoneRoot == null)
            config.zoneRoot = gameObject;
        
        // Crear/obtener triggers de zona (puede haber múltiples para salas compartidas)
        // IMPORTANTE: Solo buscar en el GameObject actual y sus hijos DIRECTOS
        // Evita marcar colliders de objetos anidados más profundamente (como suelos)
        List<Collider> triggerList = new();
        
        // 1. Colliders en el GameObject actual
        triggerList.AddRange(GetComponents<Collider>());
        
        // 2. Colliders en hijos directos (ej: ColliderNorte, ColliderSur, etc.)
        foreach (Transform child in transform)
        {
            triggerList.AddRange(child.GetComponents<Collider>());
        }
        
        allZoneTriggers = triggerList.ToArray();
        
        if (allZoneTriggers.Length == 0)
        {
            zoneTrigger = gameObject.AddComponent<BoxCollider>();
            allZoneTriggers = new Collider[] { zoneTrigger };
            Debug.LogWarning($"[ZONE] {config.zoneName} sin collider, se creó uno", gameObject);
        }
        else
        {
            zoneTrigger = allZoneTriggers[0];
        }
        
        // Asegurar que TODOS los triggers están configurados correctamente
        foreach (var trigger in allZoneTriggers)
        {
            trigger.isTrigger = true;
        }
        
        // Debug: Mostrar colliders configurados
        Debug.Log($"[ZONE COLLIDERS] {config.zoneName}: Found {allZoneTriggers.Length} colliders");
        foreach (var trigger in allZoneTriggers)
        {
            Debug.Log($"  - {trigger.gameObject.name}: isTrigger={trigger.isTrigger}, bounds={trigger.bounds}", gameObject);
        }
        
        if (allZoneTriggers.Length > 1)
        {
            Debug.Log($"[ZONE DUAL] {config.zoneName} has {allZoneTriggers.Length} triggers (multi-plane zone)", gameObject);
        }
        else if (allZoneTriggers.Length == 1)
        {
            Debug.Log($"[ZONE] {config.zoneName} configured with 1 trigger", gameObject);
        }
        
        // Obtener ZoneManager
        // PRIORITARIO: Buscar primero Cell1SectorManager (local)
        var localSectorManager = GetComponentInParent<Cell1SectorManager>();
        if (localSectorManager != null)
        {
            zoneManager = null;  // No usar ZoneManager global
            Debug.Log($"[ZONE] {config.zoneName} usando Cell1SectorManager local (no ZoneManager global)", gameObject);
        }
        else
        {
            // Fallback: Buscar ZoneManager global
            zoneManager = FindAnyObjectByType<ZoneManager>();
            if (zoneManager == null)
            {
                Debug.LogWarning("[ZONE] No ZoneManager found in scene!", gameObject);
            }
        }
        
        // SINCRONIZAR GEOMETRÍA: Asegurar que zoneCenter y zoneSize coincidan con colliders
        SyncGeometryFromColliders();
        
        // Buscar puertas conectadas
        FindConnectedDoors();
        
        // Estado inicial según si es zona de inicio
        if (config.isStartingZone)
        {
            isDiscovered = true;
            Debug.Log($"[ZONE] {config.zoneName} marked as STARTING ZONE (always discovered)", gameObject);
            // La zona inicial debe estar ACTIVE desde el principio
            SetZoneState(ZoneState.Active);
        }
        else
        {
            // Zonas desconocidas: invisible, pero triggers activos para descubrimiento
            Debug.Log($"[ZONE INIT] {config.zoneName}: Initializing to Unknown state (will be discovered when player enters)");
            // forceApply=true: asegurar que se aplique aunque currentState ya sea Unknown
            SetZoneState(ZoneState.Unknown, forceApply: true);
        }
    }
    
    /// <summary>
    /// Limpiar recursos cuando la zona se desactiva o destruye
    /// </summary>
    private void OnDisable()
    {
        // Detener fade-in si está en curso
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }
    }
    
    /// <summary>
    /// Sincronizar zoneCenter y zoneSize desde los colliders del GameObject
    /// Asegura que la geometría lógica de la zona coincida con los triggers físicos
    /// </summary>
    public void SyncGeometryFromColliders()
    {
        if (allZoneTriggers.Length == 0)
        {
            Debug.LogWarning($"[ZONE] {config.zoneName} has no colliders to sync from", gameObject);
            return;
        }
        
        // Calcular los bounds combinados de todos los colliders
        Bounds combinedBounds = allZoneTriggers[0].bounds;
        
        for (int i = 1; i < allZoneTriggers.Length; i++)
        {
            combinedBounds.Encapsulate(allZoneTriggers[i].bounds);
        }
        
        // Asignar center y size desde los bounds calculados
        Vector3 oldCenter = config.zoneCenter;
        Vector3 oldSize = config.zoneSize;
        
        config.zoneCenter = combinedBounds.center;
        config.zoneSize = combinedBounds.size;
        
        Debug.Log($"[ZONE SYNC] {config.zoneName}: Geometry synced from colliders" +
            $"\n  Center: {oldCenter} → {config.zoneCenter}" +
            $"\n  Size:   {oldSize} → {config.zoneSize}", gameObject);
    }
    
    private void FindConnectedDoors()
    {
        connectedDoors.Clear();
        
        Door[] doorsInScene = FindObjectsByType<Door>(FindObjectsSortMode.None);
        foreach (var door in doorsInScene)
        {
            if (door.Config.zoneA == this || door.Config.zoneB == this)
            {
                connectedDoors.Add(door);
                
                // Subscribirse a eventos de la puerta
                door.OnDoorOpened += OnConnectedDoorOpened;
                door.OnDoorClosed += OnConnectedDoorClosed;
            }
        }
        
        if (connectedDoors.Count > 0)
        {
            Debug.Log($"[ZONE] {config.zoneName} found {connectedDoors.Count} connected doors");
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // EVENT HANDLERS - Triggers de Unity
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Se dispara cuando algo entra al trigger de la zona
    /// Para salas multi-plano, esto puede dispararse múltiples veces
    /// (una por cada trigger). Usamos un contador para saber si es la PRIMERA entrada.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ZONE TRIGGER] {config.zoneName}: OnTriggerEnter called by {other.gameObject.name} (tag: {other.tag})", gameObject);
        
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"[ZONE TRIGGER] NOT PLAYER - tag is '{other.tag}', not 'Player'", gameObject);
            return;
        }
        
        activeTriggerCount++;
        
        // Solo procesar cuando ENTRA al PRIMER trigger
        if (activeTriggerCount == 1)
        {
            OnPlayerEnteredZone();
        }
        else
        {
            // Player está en múltiples triggers a la vez (zona multi-plano)
            Debug.Log($"[ZONE DUAL] {config.zoneName}: Player in trigger {activeTriggerCount}", gameObject);
        }
    }
    
    /// <summary>
    /// Se dispara cuando el player sale de un trigger
    /// Para salas multi-plano, solo notificamos cuando sale de TODOS
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[ZONE TRIGGER] {config.zoneName}: OnTriggerExit called by {other.gameObject.name} (tag: {other.tag})", gameObject);
        
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"[ZONE TRIGGER] NOT PLAYER - tag is '{other.tag}', not 'Player'", gameObject);
            return;
        }
        
        activeTriggerCount--;
        
        // Solo procesar cuando SALE del ÚLTIMO trigger
        if (activeTriggerCount == 0)
        {
            OnPlayerExitedZone();
        }
        else
        {
            // Player sigue dentro de otros triggers (zona multi-plano)
            Debug.Log($"[ZONE DUAL] {config.zoneName}: Player still in {activeTriggerCount} triggers", gameObject);
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // LÓGICA: Player entra/sale
    // ════════════════════════════════════════════════════════════════
    
    private void OnPlayerEnteredZone()
    {
        // Marcar como descubierta si es primera vez
        if (!isDiscovered)
        {
            isDiscovered = true;
            Debug.Log($"[ZONE] {config.zoneName} marked as DISCOVERED", gameObject);
            
            // Iniciar fade-in animado
            // NO notificamos al ZoneManager aún - esperamos a que el fade termine
            StartFadeIn();
            
            OnFirstDiscovered?.Invoke(this);
            
            Debug.Log($"[ZONE] Player entered {config.zoneName} (fade-in starting, ZoneManager notified when fade completes)", gameObject);
        }
        else
        {
            // La zona ya fue descubierta antes - notificar al ZoneManager INMEDIATAMENTE
            // No hay fade-in pendiente
            zoneManager?.OnPlayerEnteredZone(this);
            
            // Invocar evento local
            OnPlayerEntered?.Invoke(this);
            
            Debug.Log($"[ZONE] Player re-entered {config.zoneName}", gameObject);
        }
    }
    
    /// <summary>
    /// Se llama cuando el fade-in ha completado
    /// Ahora es seguro notificar al ZoneManager y otros sistemas
    /// que la zona está visualmente lista
    /// </summary>
    private void OnFadeInCompleted()
    {
        Debug.Log($"[FADE-IN] Fade completed for {config.zoneName}, notifying ZoneManager...", gameObject);
        
        // Avisar al ZoneManager (ahora sí, cuando el fade visual está completo)
        zoneManager?.OnPlayerEnteredZone(this);
        
        // Invocar evento local
        OnPlayerEntered?.Invoke(this);
    }
    
    private void OnPlayerExitedZone()
    {
        // Avisar al ZoneManager
        zoneManager?.OnPlayerExitedZone(this);
        
        // Invocar evento local
        OnPlayerExited?.Invoke(this);
        
        Debug.Log($"[ZONE] Player exited {config.zoneName}", gameObject);
    }
    
    private void OnConnectedDoorOpened(Door door)
    {
        // Cuando una puerta conectada a esta zona se abre
        // Re-construir grafo (puertas abiertas = nuevas rutas)
        zoneManager?.OnDoorStateChanged(door, true);
    }
    
    private void OnConnectedDoorClosed(Door door)
    {
        // Cuando una puerta conectada se cierra
        zoneManager?.OnDoorStateChanged(door, false);
    }
    
    // ════════════════════════════════════════════════════════════════
    // ESTADO: Cambiar estado de la zona
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Cambiar el estado de la zona
    /// Se llama desde ZoneManager.UpdateZoneVisibility()
    /// </summary>
    public void SetZoneState(ZoneState newState, bool forceApply = false)
    {
        // Sin cambios, no hacer nada (EXCEPTO si forceApply está activo)
        if (currentState == newState && !forceApply)
            return;
        
        ZoneState oldState = currentState;
        
        // Si transición Unknown→Active, iniciar fade-in PRIMERO
        if (oldState == ZoneState.Unknown && newState == ZoneState.Active)
        {
            if (!isDiscovered)
            {
                isDiscovered = true;
                Debug.Log($"[ZONE FADE] Starting fade-in for {config.zoneName}");
                StartFadeIn();
            }
        }
        
        currentState = newState;
        
        // Aplicar cambios visuales/lógicos (pasar estado anterior para transiciones suaves)
        ApplyZoneState(newState, oldState);
        
        // Eventos
        OnStateChanged?.Invoke(this, oldState, newState);
        
        if (newState == ZoneState.Active)
        {
            OnZoneActivated?.Invoke(this);
            activationTime = Time.time;
        }
        else if (oldState == ZoneState.Active)
        {
            OnZoneDeactivated?.Invoke(this);
        }
        
        Debug.Log($"[ZONE] {config.zoneName}: {oldState} → {newState}", gameObject);
    }
    
    /// <summary>
    /// Aplicar cambios según el nuevo estado
    /// 2 estados: Unknown, Active
    /// 
    /// IMPORTANTE: Durante un fade-in activo o transición de descubrimiento,
    /// preservamos el estado de renderers para no interrumpir la animación
    /// </summary>
    private void ApplyZoneState(ZoneState state, ZoneState oldState = ZoneState.Unknown)
    {
        // Si hay un fade-in en progreso, no cambiar render mode - dejar que continúe
        // O si recién fue descubierta (Unknown→Active), preservar para que el fade-in comience suavemente
        const bool preserveRenderersWhileFading = true;
        bool isBeingDiscovered = (oldState == ZoneState.Unknown && state == ZoneState.Active);
        
        if ((fadeInCoroutine != null || isBeingDiscovered) && preserveRenderersWhileFading)
        {
            // Solo cambiar estado no-visual mientras estamos con fade-in
            switch (state)
            {
                case ZoneState.Unknown:
                    DespawnEnemies();
                    isLoaded = false;
                    break;
                
                case ZoneState.Known:
                    DespawnEnemies();
                    isLoaded = true;
                    break;
                
                case ZoneState.Wireframe:
                    DespawnEnemies();
                    isLoaded = true;
                    break;
                
                case ZoneState.Dormant:
                    DespawnEnemies();
                    isLoaded = true;
                    break;
                
                case ZoneState.Active:
                    SpawnEnemies();
                    isLoaded = true;
                    break;
            }
            
            Debug.Log($"[ZONE] {config.zoneName}: Applying non-visual changes for {state} (fade-in in progress)", gameObject);
            return;  // NO cambiar render mode mientras hay fade-in
        }
        
        // Aplicación normal (sin fade-in activo)
        switch (state)
        {
            case ZoneState.Unknown:
                // Nunca visitada - completamente desactivada
                SetGameObjectActive(false, renderMode: RenderMode.Hidden);
                DespawnEnemies();
                isLoaded = false;
                break;
            
            case ZoneState.Known:
                // Preloading - zona conocida siendo cargada
                // Estado intermedio: cargada en memoria pero dormida
                SetGameObjectActive(false, renderMode: RenderMode.Hidden);
                DespawnEnemies();
                isLoaded = true;
                break;
            
            case ZoneState.Wireframe:
                // Visitada, lejana, VISIBLE en cámara
                // Mostrar solo líneas/wireframe, collideres SÍ, enemigos dormidos
                SetGameObjectActive(true, renderMode: RenderMode.Wireframe);
                DespawnEnemies();
                isLoaded = true;
                break;
            
            case ZoneState.Dormant:
                // Visitada, lejana, NO VISIBLE en cámara
                // Completamente apagada pero con collideres activos
                SetGameObjectActive(false, renderMode: RenderMode.Hidden);
                DespawnEnemies();
                isLoaded = true;
                break;
            
            case ZoneState.Active:
                // Player aquí O adyacente O con acción activa
                // Renderizado full, enemigos activos
                SetGameObjectActive(true, renderMode: RenderMode.Full);
                SpawnEnemies();
                isLoaded = true;
                break;
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // HELPERS: Activar/desactivar componentes selectivamente
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Activar/desactivar componentes de forma selectiva
    /// - Mantener triggers activos SIEMPRE (para detección de entrada)
    /// - Desactivar collideres físicos según necesidad
    /// - Desactivar scripts según necesidad
    /// - Aplicar modo de renderizado
    /// </summary>
    private void SetGameObjectActive(bool active, RenderMode renderMode = RenderMode.Full)
    {
        if (config.zoneRoot == null)
        {
            Debug.LogWarning($"[ZONE] {config.zoneName}: zoneRoot is NULL, cannot apply state");
            return;
        }
        
        Debug.Log($"[ZONE APPLY] {config.zoneName}: active={active}, renderMode={renderMode}, distance={distanceInCurrentGraph}");
        
        // NUNCA desactivar el GameObject root - necesitamos que el trigger siempre esté activo
        // para que el player pueda descubrir zonas Unknown
        // config.zoneRoot.SetActive(active);  // ← NO HACER ESTO
        
        // Aplicar modo de renderizado (esto desactiva renderers si es necesario)
        ApplyRenderMode(renderMode);
        
        // Collideres: mantener TODOS siempre activos
        // Triggers para detección de entrada/salida
        // Físicos para suelo, paredes, etc.
        foreach (Collider col in config.zoneRoot.GetComponentsInChildren<Collider>(includeInactive: true))
        {
            col.enabled = true;  // SIEMPRE ACTIVO
        }
        
        // Scripts que se pueden desactivar
        foreach (MonoBehaviour behaviour in config.zoneRoot.GetComponentsInChildren<MonoBehaviour>())
        {
            // NO desactivar componentes críticos
            if (behaviour is DynamicZone or ZoneManager or Door)
                continue;
            
            behaviour.enabled = active;
        }
    }
    
    /// <summary>
    /// Aplicar modo de renderizado a todos los renderers
    /// Preserva el alpha actual para no interferir con animaciones de fade-in
    /// </summary>
    private void ApplyRenderMode(RenderMode mode)
    {
        foreach (var renderer in config.zoneRoot.GetComponentsInChildren<Renderer>(includeInactive: true))
        {
            switch (mode)
            {
                case RenderMode.Hidden:
                    renderer.enabled = false;
                    break;
                
                case RenderMode.Wireframe:
                    renderer.enabled = true;
                    // Aplicar shader wireframe a todos los materiales
                    Shader wireframeShader = Shader.Find("Custom/Wireframe");
                    if (wireframeShader != null)
                    {
                        foreach (var material in renderer.materials)
                        {
                            material.shader = wireframeShader;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[ZONE] Wireframe shader not found at 'Custom/Wireframe'");
                    }
                    break;
                
                case RenderMode.Full:
                    renderer.enabled = true;
                    
                    // Si NO estamos en medio de un fade-in, restaurar alpha a 1 (opaco)
                    // Si estamos en fade-in, preservar el alpha actual
                    if (fadeInCoroutine == null)
                    {
                        // No hay fade en progreso - asegurar que el alpha esté en 1
                        SetZoneAlpha(1f);
                    }
                    // Si hay fade en progreso, no tocar el alpha - dejar que la corrutina lo manejo
                    
                    // Restaurar shaders originales (asumiendo nombres estándar)
                    foreach (var material in renderer.materials)
                    {
                        // Si tiene un nombre específico, restaurar, sino mantener
                        if (material.name.Contains("Standard"))
                        {
                            material.shader = Shader.Find("Standard");
                        }
                    }
                    break;
            }
        }
    }
    
    // ════════════════════════════════════════════════════════════════
    // FADE-IN ANIMATION - Aparecer gradualmente cuando se descubre
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Iniciar animación de fade-in para esta zona
    /// Se llama cuando el player descubre la zona por PRIMERA VEZ
    /// </summary>
    private void StartFadeIn()
    {
        // Si ya hay un fade en curso, detenerlo
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        
        // CRÍTICO: Unity no renderiza con GameObjects inactivos
        // Si el zoneRoot está inactivo, sus renderers nunca renderizarán, sin importar el alpha
        // SOLUCIÓN: 
        // 1. Activar temporalmente el zoneRoot para buscar renderers
        // 2. Desactivar solo los GameObjects hijos (no el root)
        // 3. El root sigue activo = los renderers pueden renderizar
        
        bool rootWasActive = config.zoneRoot.activeSelf;
        config.zoneRoot.SetActive(true);  // Necesario para que GetComponentsInChildren encuentre todo
        
        // Asegurar que los renderers estén habilitados para poder mostrar el fade
        int rendererCount = 0;
        foreach (var renderer in config.zoneRoot.GetComponentsInChildren<Renderer>(includeInactive: true))
        {
            renderer.enabled = true;
            rendererCount++;
            
            // IMPORTANTE: Instanciar todos los materiales para que los cambios de alpha
            // sean locales a estos renderers y no afecten otros GameObjects
            Material[] instancedMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                instancedMaterials[i] = new Material(renderer.materials[i]);
                
                // FADE MODE: Para desvanecimiento correcto
                // Según la documentación de Unity, uso "Fade" mode para objetos opacos que se desvanecen
                instancedMaterials[i].shader = Shader.Find("Standard");
                
                // Cambiar a Fade rendering mode (no Transparent)
                // Fade mode = opaque que se desvanece
                // Transparent mode = reflectivo transparente
                instancedMaterials[i].SetFloat("_Mode", 1f);  // 1 = Fade mode (0=Opaque, 2=Cutout, 3=Transparent)
                instancedMaterials[i].SetOverrideTag("RenderType", "Fade");
                instancedMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                instancedMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                instancedMaterials[i].SetInt("_ZWrite", 0);
                instancedMaterials[i].renderQueue = 3000;
                instancedMaterials[i].EnableKeyword("_ALPHABLEND_ON");
            }
            renderer.materials = instancedMaterials;
        }
        
        Debug.Log($"[FADE-IN] Found {rendererCount} renderers in {config.zoneName} (Fade mode), starting animation", gameObject);
        
        // Si el root no estaba activo, NO lo reactivamos de golpe pero lo dejamos ACTIVO para que los renderers funcionen
        // Solo desactivamos los hijos directos para mantener la estructura inactiva
        if (!rootWasActive)
        {
            // Desactivar los hijos, pero NO el root
            foreach (Transform child in config.zoneRoot.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        // Inicializar alpha en 0 antes de empezar la animación
        SetZoneAlpha(0f);
        
        // Iniciar corrutina de fade-in
        fadeInCoroutine = StartCoroutine(FadeInZone());
        Debug.Log($"[FADE-IN] Starting fade-in for zone: {config.zoneName} (duration: {FADE_IN_DURATION}s)", gameObject);
    }
    
    /// <summary>
    /// Corrutina que anima gradualmente el alpha de todos los materiales de la zona
    /// desde 0 (invisible) hasta 1 (opaco) en FADE_IN_DURATION segundos
    /// </summary>
    private IEnumerator FadeInZone()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < FADE_IN_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / FADE_IN_DURATION);
            
            SetZoneAlpha(alpha);
            
            yield return null;
        }
        
        // Asegurar que termina exactamente en alpha = 1
        SetZoneAlpha(1f);
        fadeInCoroutine = null;
        
        Debug.Log($"[FADE-IN] Completed fade-in for zone: {config.zoneName}", gameObject);
        
        // AHORA notificar que el fade visual está completo
        OnFadeInCompleted();
    }
    
    /// <summary>
    /// Establecer el alpha (transparencia) de todos los materiales de la zona
    /// alpha = 0 → invisible, alpha = 1 → opaco
    /// </summary>
    private void SetZoneAlpha(float alpha)
    {
        foreach (var renderer in config.zoneRoot.GetComponentsInChildren<Renderer>(includeInactive: true))
        {
            foreach (var material in renderer.materials)
            {
                // Obtener color actual y cambiar solo el alpha
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }
    
    /// <summary>
    /// Spawnar enemigos de esta zona
    /// </summary>
    private void SpawnEnemies()
    {
        if (!config.spawnEnemiesOnActivate)
            return;
        
        if (spawnedEnemies.Count > 0)
            return;  // Ya spawneados
        
        // Validar que tenemos un prefab
        if (config.enemyPrefab == null)
        {
            Debug.LogWarning($"[ZONE] Zone '{config.zoneName}' has spawnEnemiesOnActivate=true but no enemyPrefab assigned!", gameObject);
            return;
        }
        
        for (int i = 0; i < config.numEnemies; i++)
        {
            Vector3 spawnPos = config.zoneCenter + Random.insideUnitSphere * 2f;
            spawnPos.y = 0.5f;  // Altura correcta
            
            // Usar object pool si disponible
            GameObject enemy = EnemyPool.Instance?.GetEnemy(spawnPos, Quaternion.identity);
            
            if (enemy == null)
            {
                // Fallback: crear enemigo nuevo desde el prefab configurado
                enemy = Instantiate(
                    config.enemyPrefab,
                    spawnPos,
                    Quaternion.identity,
                    config.zoneRoot.transform
                );
            }
            
            if (enemy != null)
            {
                spawnedEnemies.Add(enemy.transform);
                
                // Configurar comportamiento del enemigo
                // TODO: Implementar cuando EnemyController esté disponible
                /*
                EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
                if (enemyCtrl != null)
                {
                    enemyCtrl.SetPatrolArea(config.zoneCenter, config.zoneSize);
                }
                */
            }
        }
        
        Debug.Log($"[ZONE] Spawned {spawnedEnemies.Count} enemies in {config.zoneName}");
    }
    
    /// <summary>
    /// Desactivar/remover enemigos
    /// </summary>
    private void DespawnEnemies()
    {
        foreach (Transform enemy in spawnedEnemies)
        {
            if (enemy == null)
                continue;
            
            // Devolver al pool si es posible
            EnemyPool.Instance?.ReturnEnemy(enemy.gameObject);
            
            // Fallback: destruir
            Destroy(enemy.gameObject);
        }
        
        spawnedEnemies.Clear();
    }
    
    // ════════════════════════════════════════════════════════════════
    // INFORMATION QUERIES
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// ¿El punto está dentro de esta zona?
    /// </summary>
    public bool ContainsPoint(Vector3 point)
    {
        Vector3 relPos = point - config.zoneCenter;
        
        return Mathf.Abs(relPos.x) <= config.zoneSize.x / 2f &&
               Mathf.Abs(relPos.z) <= config.zoneSize.z / 2f;
    }
    
    /// <summary>
    /// Distancia desde un punto a esta zona
    /// </summary>
    public float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Distance(point, config.zoneCenter);
    }
    
    /// <summary>
    /// Obtener las puertas conectadas a otra zona
    /// </summary>
    public List<Door> GetDoorsTo(DynamicZone otherZone)
    {
        List<Door> result = new();
        
        foreach (var door in connectedDoors)
        {
            if (door.GetDestinationZone(this) == otherZone)
                result.Add(door);
        }
        
        return result;
    }
    
    // ════════════════════════════════════════════════════════════════
    // DEBUG & GIZMOS
    // ════════════════════════════════════════════════════════════════
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        // Color basado en estado
        Color stateColor = currentState switch
        {
            ZoneState.Unknown => Color.gray,      // Gris - no visitada
            ZoneState.Wireframe => Color.cyan,    // Cian - visible wireframe
            ZoneState.Dormant => Color.red,       // Rojo - apagada
            ZoneState.Active => Color.green,      // Verde - activa
            _ => Color.white
        };
        
        // Dibujar bounding box
        Gizmos.color = stateColor;
        Gizmos.DrawWireCube(config.zoneCenter, config.zoneSize);
        
        // Label
        UnityEditor.Handles.Label(
            config.zoneCenter + Vector3.up * (config.zoneSize.y / 2 + 1),
            $"{config.zoneName}\n{currentState}\n{(isDiscovered ? "Discovered" : "Unknown")}"
        );
        
        // Dibujar conexiones a otras zonas
        if (config.connections != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var conn in config.connections)
            {
                if (conn.connectedZone != null)
                {
                    Gizmos.DrawLine(
                        config.zoneCenter,
                        conn.connectedZone.Config.zoneCenter
                    );
                }
            }
        }
        
        // Dibujar puertas conectadas
        Gizmos.color = Color.magenta;
        foreach (var door in connectedDoors)
        {
            Gizmos.DrawWireSphere(door.Config.positionA, 0.3f);
            Gizmos.DrawWireSphere(door.Config.positionB, 0.3f);
        }
    }
    
    /// <summary>
    /// [EDITOR ONLY] Sincronizar geometría manualmente
    /// Útil para ajustar colliders y luego actualizar los valores de zona
    /// </summary>
    [UnityEditor.MenuItem("CONTEXT/DynamicZone/Sync Geometry from Colliders")]
    public static void SyncGeometryMenuItem(UnityEditor.MenuCommand command)
    {
        DynamicZone zone = (DynamicZone)command.context;
        if (zone != null)
        {
            zone.allZoneTriggers = zone.GetComponents<Collider>();
            zone.SyncGeometryFromColliders();
            UnityEditor.EditorUtility.SetDirty(zone);
            Debug.Log($"[EDITOR] Geometry synced for {zone.config.zoneName}");
        }
    }
    
    #endif
}

// ════════════════════════════════════════════════════════════════════════════════
// ENEMY POOL - Pool de enemigos reutilizable
// ════════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Pool global de enemigos para reutilización eficiente
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 50;
    
    private Queue<GameObject> availableEnemies = new();
    private HashSet<GameObject> activeEnemies = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Pre-crear enemigos
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            availableEnemies.Enqueue(enemy);
        }
    }
    
    /// <summary>
    /// Obtener enemigo del pool
    /// </summary>
    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemy;
        
        if (availableEnemies.Count > 0)
        {
            enemy = availableEnemies.Dequeue();
        }
        else
        {
            // Crear más si se agota el pool
            enemy = Instantiate(enemyPrefab);
        }
        
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        enemy.SetActive(true);
        activeEnemies.Add(enemy);
        
        return enemy;
    }
    
    /// <summary>
    /// Devolver enemigo al pool
    /// </summary>
    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;
        
        activeEnemies.Remove(enemy);
        enemy.SetActive(false);
        availableEnemies.Enqueue(enemy);
    }
    
    /// <summary>
    /// Estadísticas del pool
    /// </summary>
    public void DebugStats()
    {
        Debug.Log($"[POOL] Active: {activeEnemies.Count}, Available: {availableEnemies.Count}");
    }
}
