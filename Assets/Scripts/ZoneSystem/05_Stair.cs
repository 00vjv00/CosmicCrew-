// ============================================================================
// STAIR (ESCALERA) - Solo efectos de cámara
// Cosmic Crew - Sistema de Escaleras para Corte Vertical
// ============================================================================

using UnityEngine;

/// <summary>
/// Escalera que activa el modo vista de perfil de la cámara
/// El jugador puede moverse libremente, la cámara pasa a corte vertical
/// </summary>
public class Stair : MonoBehaviour
{
    [SerializeField] private DynamicZoneConfig configFloorA;  // Piso bajo (referencia)
    [SerializeField] private DynamicZoneConfig configFloorB;  // Piso alto (referencia)
    
    [SerializeField] private Collider stairTrigger;  // Trigger de la escalera
    
    private bool isPlayerOnStair = false;
    
    private void Awake()
    {
        // Obtener trigger
        if (stairTrigger == null)
            stairTrigger = GetComponent<Collider>();
        
        if (stairTrigger == null)
        {
            Debug.LogError("[STAIR] Sin collider trigger", gameObject);
            return;
        }
        
        stairTrigger.isTrigger = true;
        
        Debug.Log($"[STAIR] Escalera lista - Cámara en modo perfil cuando el jugador entre");
    }
    
    /// <summary>
    /// Player entra a escalera → activar vista de perfil
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        isPlayerOnStair = true;
        
        // Activar modo escalera en cámara
        if (CameraController.Instance != null)
        {
            CameraController.Instance.EnterStairMode(this);
            Debug.Log("[STAIR] Cámara cambió a vista de perfil");
        }
    }
    
    /// <summary>
    /// Player sale de escalera → volver a vista isométrica
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        isPlayerOnStair = false;
        
        // Desactivar modo escalera en cámara
        if (CameraController.Instance != null)
        {
            CameraController.Instance.ExitStairCutMode();
            Debug.Log("[STAIR] Cámara volvió a vista isométrica");
        }
    }
    
    public bool IsPlayerOnStair => isPlayerOnStair;
}
