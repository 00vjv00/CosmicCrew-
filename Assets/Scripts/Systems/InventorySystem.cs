// ============================================================================
// INVENTORY SYSTEM - Sistema centralizado de inventario
// Gestiona el estado de objetos y recursos del player
// ============================================================================

using UnityEngine;

/// <summary>
/// Singleton que gestiona el inventario del jugador.
/// Persiste entre escenas.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    // ────────────────────────────────────────────────────────────
    // SINGLETON
    // ────────────────────────────────────────────────────────────
    
    private static InventorySystem instance;
    
    public static InventorySystem Instance
    {
        get
        {
            if (instance == null)
            {
                // Buscar en la escena
                instance = FindObjectOfType<InventorySystem>();
                
                if (instance == null)
                {
                    // Crear si no existe
                    GameObject obj = new GameObject("InventorySystem");
                    instance = obj.AddComponent<InventorySystem>();
                    DontDestroyOnLoad(obj);
                    Debug.Log("[INVENTORY] InventorySystem created (singleton)", obj);
                }
            }
            return instance;
        }
    }
    
    
    // ────────────────────────────────────────────────────────────
    // ESTADO INTERNO - Visible en Inspector para DEBUG
    // ────────────────────────────────────────────────────────────
    
    [Header("═ ITEMS ═")]
    [SerializeField] private bool hasCrateKey = false;  // Llave para puerta de zona
    
    // ────────────────────────────────────────────────────────────
    // PROPIEDADES
    // ────────────────────────────────────────────────────────────
    
    public bool HasCrateKey
    {
        get => hasCrateKey;
        private set => hasCrateKey = value;
    }
    
    // ────────────────────────────────────────────────────────────
    // INICIALIZACIÓN
    // ────────────────────────────────────────────────────────────
    
    private void Awake()
    {
        // Singleton pattern - evitar duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // ────────────────────────────────────────────────────────────
    // MÉTODOS PÚBLICOS
    // ────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Adquirir la llave del crate
    /// </summary>
    public void AcquireKey()
    {
        if (hasCrateKey)
        {
            Debug.LogWarning("[INVENTORY] Already has crate key!");
            return;
        }
        
        hasCrateKey = true;
        Debug.Log("[INVENTORY] Crate key acquired!", gameObject);
    }
    
    /// <summary>
    /// Consumir la llave del crate (usarla)
    /// </summary>
    public void ConsumeCrateKey()
    {
        if (!hasCrateKey)
        {
            Debug.LogWarning("[INVENTORY] Trying to consume key but doesn't have one!");
            return;
        }
        
        hasCrateKey = false;
        Debug.Log("[INVENTORY] Crate key consumed!", gameObject);
    }
    
    /// <summary>
    /// Verificar si tiene la llave sin consumirla
    /// </summary>
    public bool CheckCrateKey()
    {
        return hasCrateKey;
    }
}
