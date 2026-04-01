// ============================================================================
// CELL1 RUNTIME SETUP
// Cosmic Crew - Inicializa Cell1 en tiempo de juego si no existe
// ============================================================================

using UnityEngine;

/// <summary>
/// Script que se ejecuta en tiempo de juego para inicializar Cell1.
/// Si Cell1 no existe en la escena, busca el prefab y lo instancia.
/// </summary>
public class Cell1RuntimeSetup : MonoBehaviour
{
    private static bool hasInitialized = false;
    
    private void Awake()
    {
        // Solo hacer esto UNA VEZ
        if (hasInitialized)
            return;
        
        hasInitialized = true;
        
        // Verificar si Cell1 ya existe en la escena
        var existingCell1 = FindObjectOfType<Cell1SectorManager>();
        if (existingCell1 != null)
        {
            Debug.Log("[CELL1] Cell1SectorManager ya existe en la escena");
            return;
        }
        
        // Intentar cargar el prefab Cell1
        GameObject cell1Prefab = Resources.Load<GameObject>("Prefabs/Sectors/Cell1");
        if (cell1Prefab == null)
        {
            // Si no está en Resources, crear estructura en tiempo de juego (fallback)
            Debug.Log("[CELL1] Cell1.prefab no encontrado en Resources, creando estructura en runtime...");
            CreateCell1RuntimeStructure();
        }
        else
        {
            // Instanciar el prefab
            Instantiate(cell1Prefab);
            Debug.Log("[CELL1] Cell1.prefab instanciado exitosamente");
        }
    }
    
    /// <summary>
    /// Crea la estructura Cell1 completa en tiempo de juego (fallback)
    /// </summary>
    private void CreateCell1RuntimeStructure()
    {
        // Crear root
        GameObject cell1Root = new GameObject("Cell1_Runtime");
        cell1Root.AddComponent<Cell1SectorManager>();
        
        // Crear zonas
        CreateZoneRuntime(cell1Root, "CellMain", new Vector3(0, 0, 0), new Vector3(10, 4, 10), isStarting: true);
        CreateZoneRuntime(cell1Root, "Corridor1", new Vector3(10, 0, 0), new Vector3(5, 4, 8), isStarting: false);
        CreateZoneRuntime(cell1Root, "Corridor2", new Vector3(15, 0, 0), new Vector3(8, 4, 10), isStarting: false);
        
        Debug.Log("[CELL1] Estructura de Cell1 creada en runtime");
    }
    
    private void CreateZoneRuntime(GameObject parent, string zoneName, Vector3 localPos, Vector3 size, bool isStarting)
    {
        GameObject zoneGO = new GameObject(zoneName);
        zoneGO.transform.parent = parent.transform;
        zoneGO.transform.localPosition = localPos;
        
        // Agregar componentes
        DynamicZone zone = zoneGO.AddComponent<DynamicZone>();
        BoxCollider collider = zoneGO.AddComponent<BoxCollider>();
        Rigidbody rb = zoneGO.AddComponent<Rigidbody>();
        
        collider.isTrigger = true;
        collider.size = size;
        
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        // Configurar zona (por reflexión)
        DynamicZoneConfig config = new DynamicZoneConfig
        {
            zoneName = zoneName,
            zoneID = zoneName == "CellMain" ? 1 : (zoneName == "Corridor1" ? 2 : 3),
            floorLevel = 0,
            isStartingZone = isStarting,
            zoneCenter = Vector3.zero,
            zoneSize = size,
            zoneType = ZoneType.ROOM,
            zoneRoot = zoneGO,
            spawnEnemiesOnActivate = (zoneName == "Corridor1"),
            numEnemies = 1,
            connections = new ZoneConnection[0]
        };
        
        var field = typeof(DynamicZone).GetField("config", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(zone, config);
    }
}
