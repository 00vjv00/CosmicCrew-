// ============================================================================
// CAMERA CONTROLLER
// Cosmic Crew - Cámara isométrica que sigue al player y se ajusta a la zona
// ============================================================================

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;

/// <summary>
/// Sistema de cámara isométrica
/// - Sigue al player con vista isométrica (45°)
/// - Se ajusta en distancia para mostrar toda la zona actual
/// - Mantiene proporciones isométricas
/// </summary>
public partial class CameraController : MonoBehaviour
{
    // ════════════════════════════════════════════════════════════════
    // SINGLETON
    // ════════════════════════════════════════════════════════════════
    
    public static CameraController Instance { get; private set; }
    
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
    // CONFIGURACIÓN
    // ════════════════════════════════════════════════════════════════
    
    // Sistema de zoom dinámico usando Cinemachine FollowZoom
    // Ajusta el ancho visible basado en el tamaño de la zona actual
    // El FollowZoom component debe estar agregado al CinemachineCamera
    
    // ════════════════════════════════════════════════════════════════
    // REFERENCIAS
    // ════════════════════════════════════════════════════════════════
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    
    [SerializeField] private float zoomPadding = 1.2f;
    [SerializeField] private float minOrthoSize = 5f;
    [SerializeField] private float zoomTransitionSpeed = 5f;  // Velocidad de suavizado (más alto = más rápido)
    
    private Transform playerTransform;
    private DynamicZone currentZone;
    private float currentOrthoSize;  // Ortho size actual (interpolado)
    private float targetOrthoSize;   // Ortho size objetivo
    
    // ────────────────────────────────────────────────────────
    // MODO ESCALERA (Corte vertical)
    // ────────────────────────────────────────────────────────
    
    private bool isInStairCutMode = false;
    private float stairCutDistance = 30f;
    private float stairCutHeight = 6f;
    private float stairCutOrthoSize = 12f;
    private float stairCutTransitionSpeed = 2f;
    
    private void Start()
    {
        // Buscar al Player automáticamente
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
            playerObj = GameObject.Find("Player");
        
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("[CAMERA] Player not found!");
        
        // Validaciones
        if (mainCamera == null)
        {
            Debug.LogError("[CAMERA] Main Camera no asignado!");
            return;
        }
        
        if (cinemachineCamera == null)
        {
            Debug.LogError("[CAMERA] CinemachineCamera no asignado!");
            return;
        }
        
        Debug.Log("[CAMERA] ✓ Sistema de zoom Group Framing listo");
        mainCamera.orthographic = true;
        
        // Inicializar zoom actual
        if (cinemachineCamera != null)
        {
            currentOrthoSize = cinemachineCamera.Lens.OrthographicSize;
            targetOrthoSize = currentOrthoSize;
        }
    }
    
    private void LateUpdate()
    {
        if (playerTransform == null || cinemachineCamera == null)
            return;
        
        // MODO ESCALERA: Vista de corte vertical
        if (isInStairCutMode)
        {
            UpdateStairCutView();
            return;
        }
        
        // MODO ISOMÉTRICO: Seguimiento normal
        // Actualizar zona actual
        UpdateCurrentZone();
        
        if (currentZone == null)
            return;
        
        // Calcular Ortho Size objetivo basado en el tamaño de la zona
        Vector3 zoneSize = currentZone.Config.zoneSize;
        float maxDimension = Mathf.Max(zoneSize.x, zoneSize.z);
        targetOrthoSize = (maxDimension / 2f) * zoomPadding;
        targetOrthoSize = Mathf.Max(targetOrthoSize, minOrthoSize);
        
        // Interpolar suavemente hacia el Ortho Size objetivo
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, Time.deltaTime * zoomTransitionSpeed);
        
        // Aplicar el Ortho Size (Cinemachine 3: Lens es un struct)
        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = currentOrthoSize;
        cinemachineCamera.Lens = lens;
    }
    
    /// <summary>
    /// Actualizar cuál es la zona actual del player
    /// </summary>
    private void UpdateCurrentZone()
    {
        DynamicZone[] allZones = FindObjectsByType<DynamicZone>(FindObjectsSortMode.None);
        DynamicZone closestZone = null;
        float closestDistance = float.MaxValue;
        
        foreach (var zone in allZones)
        {
            // Verificar si el player está dentro del trigger de esta zona
            if (zone.Config.zoneRoot != null)
            {
                Collider[] colliders = zone.Config.zoneRoot.GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                {
                    if (col.isTrigger && col.bounds.Contains(playerTransform.position))
                    {
                        currentZone = zone;
                        return;
                    }
                }
                
                // Rastrear la zona más cercana si no estamos dentro de ninguna
                float dist = Vector3.Distance(playerTransform.position, zone.Config.zoneCenter);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestZone = zone;
                }
            }
        }
        
        if (closestZone != null)
            currentZone = closestZone;
    }
    
    /// <summary>
    /// ¿Una zona está visible en el frustum de la cámara?
    /// </summary>
    public bool IsZoneInCamera(DynamicZone zone)
    {
        if (zone == null || zone.Config == null || mainCamera == null)
            return false;
        
        Bounds zoneBounds = new Bounds(zone.Config.zoneCenter, zone.Config.zoneSize);
        return GeometryUtility.TestPlanesAABB(
            GeometryUtility.CalculateFrustumPlanes(mainCamera),
            zoneBounds
        );
    }
    
    /// <summary>
    /// ¿Un punto está visible en la cámara?
    /// </summary>
    public bool IsPointInCamera(Vector3 point)
    {
        if (mainCamera == null)
            return false;
        
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(point);
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z > 0;
    }
    
    /// <summary>
    /// Obtener todas las zonas visibles
    /// </summary>
    public List<DynamicZone> GetVisibleZones()
    {
        List<DynamicZone> visibleZones = new();
        DynamicZone[] allZones = FindObjectsByType<DynamicZone>(FindObjectsSortMode.None);
        
        foreach (var zone in allZones)
        {
            if (IsZoneInCamera(zone))
                visibleZones.Add(zone);
        }
        
        return visibleZones;
    }
    
    /// <summary>
    /// Distancia desde cámara a zona
    /// </summary>
    public float GetDistanceFromCamera(DynamicZone zone)
    {
        if (zone == null || mainCamera == null)
            return float.MaxValue;
        
        return Vector3.Distance(mainCamera.transform.position, zone.Config.zoneCenter);
    }
    
    // ════════════════════════════════════════════════════════════════
    // MODO ESCALERA: Vista de corte vertical
    // ════════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Entrar a modo escalera desde una escalera
    /// Se llama desde Stair.OnPlayerStartClimbing
    /// </summary>
    public void EnterStairMode(Stair stair)
    {
        if (stair == null)
            return;
        
        SetStairCutMode(
            distance: stairCutDistance,
            height: stairCutHeight,
            orthoSize: stairCutOrthoSize,
            transitionSpeed: stairCutTransitionSpeed
        );
    }
    
    /// <summary>
    /// Cambiar a modo CORTE VERTICAL (vista de perfil de escaleras)
    /// Se llama desde StairZone.OnPlayerEntered
    /// </summary>
    public void SetStairCutMode(float distance, float height, float orthoSize, float transitionSpeed = 2f)
    {
        isInStairCutMode = true;
        stairCutDistance = distance;
        stairCutHeight = height;
        stairCutOrthoSize = orthoSize;
        stairCutTransitionSpeed = transitionSpeed;
        
        mainCamera.orthographic = true;
        
        Debug.Log($"[CAMERA] ✓ Stair Cut Mode enabled - Distance: {distance}, Height: {height}, Ortho: {orthoSize}");
    }
    
    /// <summary>
    /// Salir del modo corte vertical y volver al modo isométrico
    /// Se llama desde StairZone.OnPlayerExited
    /// </summary>
    public void ExitStairCutMode()
    {
        isInStairCutMode = false;
        
        Debug.Log("[CAMERA] ✓ Stair Cut Mode disabled - Returning to isometric view");
    }
    
    /// <summary>
    /// Lógica de corte vertical para escaleras
    /// Mantiene la cámara con vista lateral del edificio
    /// </summary>
    private void UpdateStairCutView()
    {
        if (playerTransform == null)
            return;
        
        // Posición de cámara para vista de perfil:
        // X = mismo que player (costado de escalera)
        // Y = altura media fija (para ver todos los pisos)
        // Z = alejada atrás del player (para ver el perfil)
        
        Vector3 targetCameraPos = new Vector3(
            playerTransform.position.x,                    // Mismo X (costado)
            stairCutHeight,                                // Altura fija (eje medio)
            playerTransform.position.z - stairCutDistance  // Alejada atrás
        );
        
        // Mover cámara suavemente hacia posición objetivo
        cinemachineCamera.transform.position = Vector3.Lerp(
            cinemachineCamera.transform.position,
            targetCameraPos,
            Time.deltaTime * stairCutTransitionSpeed
        );
        
        // Mirar directamente al jugador (rotación de perfil puro)
        cinemachineCamera.transform.LookAt(playerTransform.position + Vector3.up * (stairCutHeight * 0.5f));
        
        // Aplicar tamaño ortho para captar todos los pisos
        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, stairCutOrthoSize, Time.deltaTime * stairCutTransitionSpeed);
        cinemachineCamera.Lens = lens;
        
        // Debug
        Debug.Log($"[CAMERA STAIR] Position: {cinemachineCamera.transform.position}, OrthSize: {lens.OrthographicSize:F2}");
    }
}