// ============================================================================
// ZONE COLLIDER OPTIMIZER
// Cosmic Crew - Consolida múltiples colliders en uno solo por zona
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utilidad para optimizar colliders de zonas.
/// Remueve colliders superponiéndose y crea uno solo que engloba la zona completa.
/// 
/// USO: 
/// 1. Selecciona una zona en el Hierarchy (ej: SC_Circular)
/// 2. En el Inspector, agrega este script como componente temporal
/// 3. Haz click en "Optimize Zone Colliders"
/// 4. Elimina este script (ya no es necesario)
/// </summary>
public class ZoneColliderOptimizer : MonoBehaviour
{
    [ContextMenu("Optimize Zone Colliders")]
    public void OptimizeColliders()
    {
        // Obtener la zona
        DynamicZone zone = GetComponent<DynamicZone>();
        if (zone == null)
        {
            Debug.LogError("[COLLIDER OPTIMIZER] No DynamicZone found on this GameObject", gameObject);
            return;
        }
        
        // Obtener todos los colliders existentes
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        if (allColliders.Length == 0)
        {
            Debug.LogWarning("[COLLIDER OPTIMIZER] No colliders found", gameObject);
            return;
        }
        
        Debug.Log($"[COLLIDER OPTIMIZER] Optimizing {zone.Config.zoneName}: Found {allColliders.Length} colliders");
        
        // Calcular bounds combinados
        Bounds combinedBounds = allColliders[0].bounds;
        for (int i = 1; i < allColliders.Length; i++)
        {
            combinedBounds.Encapsulate(allColliders[i].bounds);
        }
        
        Debug.Log($"[COLLIDER OPTIMIZER] Combined bounds: Center={combinedBounds.center}, Size={combinedBounds.size}");
        
        // Eliminar todos los colliders existentes
        foreach (var collider in allColliders)
        {
            Debug.Log($"[COLLIDER OPTIMIZER] Removing {collider.gameObject.name}::{collider.GetType().Name}");
            DestroyImmediate(collider);
        }
        
        // Crear UN SOLO BoxCollider en el GameObject raíz
        BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
        
        // Convertir bounds mundiales a locales
        Vector3 localCenter = gameObject.transform.worldToLocalMatrix.MultiplyPoint(combinedBounds.center);
        Vector3 localSize = combinedBounds.size;
        
        newCollider.center = localCenter;
        newCollider.size = localSize;
        newCollider.isTrigger = true;
        
        Debug.Log($"[COLLIDER OPTIMIZER] Created new BoxCollider on {gameObject.name}:");
        Debug.Log($"  Center: {localCenter}");
        Debug.Log($"  Size: {localSize}");
        Debug.Log($"  isTrigger: true");
        
        // Force DynamicZone to re-sync geometry
        zone.SyncGeometryFromColliders();
        
        Debug.Log($"[COLLIDER OPTIMIZER] ✓ {zone.Config.zoneName} optimized successfully!");
    }
}
