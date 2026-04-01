// ============================================================================
// CRATE KEY - Llave flotante para abrir el crate
// El player debe tocarla para adquirirla
// ============================================================================

using UnityEngine;

/// <summary>
/// Representa una llave dentro del crate.
/// Cuando el player toca este objeto, adquiere la llave en el inventario.
/// </summary>
public class CrateKey : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";      // Tag del jugador
    [SerializeField] private bool destroyOnPickup = true;      // ¿Destruir al recoger?
    
    // ────────────────────────────────────────────────────────────
    // COLISIÓN
    // ────────────────────────────────────────────────────────────
    
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el player
        if (!other.CompareTag(playerTag))
        {
            return;
        }
        
        // Adquirir la llave
        InventorySystem.Instance.AcquireKey();
        
        // Destruir o desactivar el objeto
        if (destroyOnPickup)
        {
            Debug.Log($"[CRATE KEY] Key picked up and destroyed", gameObject);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log($"[CRATE KEY] Key picked up and deactivated", gameObject);
        }
    }
    
    // ────────────────────────────────────────────────────────────
    // VISUAL DEBUG (EDITOR)
    // ────────────────────────────────────────────────────────────
    
    private void OnDrawGizmos()
    {
        // Dibujar esfera roja en el editor para visualizar el trigger
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
