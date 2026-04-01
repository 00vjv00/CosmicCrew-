// ============================================================================
// CINEMACHINE ZOOM EXTENSION
// Cosmic Crew - Control dinámico de zoom basado en tamaño de zona
// ============================================================================

using UnityEngine;
using System.Reflection;

/// <summary>
/// Componente que ajusta dinámicamente el zoom (orthographic size)
/// basado en el tamaño de la zona actual donde está el jugador.
/// 
/// Se agrega al CinemachineCamera para controlar el zoom en LateUpdate,
/// después de que Cinemachine ha actualizado la cámara.
/// </summary>
public class CinemachineZoomExtension : MonoBehaviour
{
    [SerializeField] private float zoomPadding = 0.75f;
    [SerializeField] private float zoomSmoothness = 0.2f;
    [SerializeField] private float minOrthographicSize = 5f;
    
    private float currentOrthographicSize = 10f;
    private DynamicZone currentZone;
    private Transform playerTransform;
    
    // Reflexión para acceder al Lens del CinemachineCamera
    private FieldInfo lensField;
    private FieldInfo orthographicSizeField;
    private Component cinemachineCamera;
    
    private void Start()
    {
        // Encontrar al jugador
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        
        // Obtener referencia al CinemachineCamera (este componente debe estar en el mismo GameObject)
        cinemachineCamera = GetComponent("Cinemachine.CinemachineCamera") as Component;
        
        if (cinemachineCamera != null)
        {
            // Encontrar el field Lens
            var cinemachineType = cinemachineCamera.GetType();
            lensField = cinemachineType.GetField("Lens", BindingFlags.Instance | BindingFlags.Public);
            
            if (lensField != null)
            {
                // Encontrar el field OrthographicSize dentro de Lens
                object lensObj = lensField.GetValue(cinemachineCamera);
                var lensType = lensObj.GetType();
                orthographicSizeField = lensType.GetField("OrthographicSize", BindingFlags.Instance | BindingFlags.Public);
            }
        }
    }
    
    private void LateUpdate()
    {
        if (playerTransform == null || cinemachineCamera == null)
            return;
        
        // Actualizar zona actual
        UpdateCurrentZone();
        
        // Calcular tamaño ortográfico necesario
        float requiredSize = CalculateRequiredOrthographicSize();
        
        // Suavizar zoom
        currentOrthographicSize = Mathf.Lerp(currentOrthographicSize, requiredSize, zoomSmoothness);
        
        // Aplicar el zoom al Lens de Cinemachine
        if (lensField != null && orthographicSizeField != null)
        {
            try
            {
                object lensObj = lensField.GetValue(cinemachineCamera);
                orthographicSizeField.SetValue(lensObj, currentOrthographicSize);
                lensField.SetValue(cinemachineCamera, lensObj);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[CAMERA] Error setting zoom: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Calcular tamaño ortográfico para mostrar toda la zona
    /// </summary>
    private float CalculateRequiredOrthographicSize()
    {
        if (currentZone == null || currentZone.Config == null)
            return 10f;
        
        Vector3 zoneSize = currentZone.Config.zoneSize;
        
        // Diagonal de la zona en el plano XZ
        float diagonalXZ = new Vector2(zoneSize.x, zoneSize.z).magnitude;
        
        // orthographicSize es la mitad de la altura visible
        float requiredSize = (diagonalXZ / 2f) * zoomPadding;
        
        return Mathf.Max(minOrthographicSize, requiredSize);
    }
    
    /// <summary>
    /// Actualizar cuál es la zona actual del player
    /// </summary>
    private void UpdateCurrentZone()
    {
        if (playerTransform == null)
            return;
        
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
}
