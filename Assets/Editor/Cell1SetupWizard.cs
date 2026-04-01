// ============================================================================
// CELL1 SETUP WIZARD
// Cosmic Crew - Editor utility para crear la estructura de Cell1 automáticamente
// ============================================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class Cell1SetupWizard : EditorWindow
{
    [MenuItem("CosmicCrew/Setup/Create Cell1 Structure")]
    public static void CreateCell1Structure()
    {
        // Crear el GameObject root de Cell1
        GameObject cell1Root = new GameObject("Cell1");
        cell1Root.transform.position = Vector3.zero;
        
        // Agregar el Cell1SectorManager al root
        cell1Root.AddComponent<Cell1SectorManager>();
        
        // Crear contenedor de zonas
        GameObject zonesContainer = new GameObject("Zones");
        zonesContainer.transform.parent = cell1Root.transform;
        zonesContainer.transform.localPosition = Vector3.zero;
        
        // Crear 3 zonas
        CreateZone(zonesContainer, "CellMain", new Vector3(0, 0, 0), new Vector3(10, 4, 10), isStarting: true);
        CreateZone(zonesContainer, "Corridor1", new Vector3(10, 0, 0), new Vector3(5, 4, 8), isStarting: false);
        CreateZone(zonesContainer, "Corridor2", new Vector3(15, 0, 0), new Vector3(8, 4, 10), isStarting: false);
        
        // Crear contenedor de puertas
        GameObject doorsContainer = new GameObject("Doors");
        doorsContainer.transform.parent = cell1Root.transform;
        doorsContainer.transform.localPosition = Vector3.zero;
        
        // Crear contenedor de puzzles/computadoras
        GameObject computersContainer = new GameObject("Computers");
        computersContainer.transform.parent = cell1Root.transform;
        computersContainer.transform.localPosition = Vector3.zero;
        
        // Crear punto de referencia para la cámara
        GameObject cameraCenter = new GameObject("CameraCenter");
        cameraCenter.transform.parent = cell1Root.transform;
        cameraCenter.transform.localPosition = new Vector3(7.5f, 0, 0);  // Centro aproximado
        
        // Crear punto para spawn de player
        GameObject spawnArea = new GameObject("SpawnArea");
        spawnArea.transform.parent = zonesContainer.transform.Find("CellMain");
        spawnArea.transform.localPosition = Vector3.zero;
        
        // Marcar como prefab preparado
        EditorUtility.DisplayDialog("Cell1 Setup", 
            "Estructura de Cell1 creada:\n" +
            "- Cell1 (root)\n" +
            "  - Zones/ (CellMain, Corridor1, Corridor2)\n" +
            "  - Doors/\n" +
            "  - Computers/\n" +
            "  - CameraCenter\n\n" +
            "Ahora:\n" +
            "1. Crea un prefab de Cell1 (guardar como Assets/Prefabs/Sectors/Cell1.prefab)\n" +
            "2. Conecta las puertas entre zonas en el inspector\n" +
            "3. Agrega computadoras\n" +
            "4. Configura spawn points en CellMain", 
            "OK"
        );
        
        Selection.activeGameObject = cell1Root;
    }
    
    private static void CreateZone(GameObject parent, string zoneName, Vector3 localPos, Vector3 size, bool isStarting = false)
    {
        // Crear GameObject de zona
        GameObject zoneGO = new GameObject(zoneName);
        zoneGO.transform.parent = parent.transform;
        zoneGO.transform.localPosition = localPos;
        
        // Agregar DynamicZone component
        DynamicZone zone = zoneGO.AddComponent<DynamicZone>();
        
        // Crear y asignar DynamicZoneConfig
        DynamicZoneConfig config = new DynamicZoneConfig
        {
            zoneName = zoneName,
            zoneID = GetZoneID(zoneName),
            floorLevel = 0,
            isStartingZone = isStarting,
            zoneCenter = Vector3.zero,  // Local space
            zoneSize = size,
            zoneType = ZoneType.ROOM,
            zoneRoot = zoneGO,
            spawnEnemiesOnActivate = (zoneName == "Corridor1"), // Solo Corridor1 spawn enemigos
            numEnemies = 1,
            connections = new ZoneConnection[0]
        };
        
        // Usar reflexión para asignar el config (es private)
        var field = typeof(DynamicZone).GetField("config", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(zone, config);
        
        // Agregar collider trigger
        BoxCollider collider = zoneGO.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = size;
        collider.center = Vector3.zero;
        
        // Agregar Rigidbody (kinematic) para triggers
        Rigidbody rb = zoneGO.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        Debug.Log($"[SETUP] Zona creada: {zoneName} en posición local {localPos}");
    }
    
    private static int GetZoneID(string zoneName)
    {
        return zoneName switch
        {
            "CellMain" => 1,
            "Corridor1" => 2,
            "Corridor2" => 3,
            _ => 0
        };
    }
}
#endif
