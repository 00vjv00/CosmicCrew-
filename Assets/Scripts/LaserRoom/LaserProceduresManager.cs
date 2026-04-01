// ============================================================================
// LASER PROCEDURES MANAGER - Genera la sala procedural de láseres
// Archivo: Assets/Scripts/LaserRoom/LaserProceduresManager.cs
// Descripción: Genera proceduralmente 3 áreas lineales:
//              Corredor 1 → Sala Grande → Corredor 2
// ============================================================================

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor principal que genera proceduralmente 3 áreas alineadas:
/// 1. Corredor 1 (18×42, Z:0-42, X:0-18) - 4 laseres
/// 2. Sala Grande (36×36, Z:0-36, X:18-54) - Ordenador + 4 prismas + 4 emisores
/// 3. Corredor 2 (18×42, Z:0-42, X:54-72) - 4 laseres
/// </summary>
public class LaserProceduresManager : MonoBehaviour
{
    // ────────────────────────────────────────────────────────
    // CONFIGURACIÓN - CORREDORES
    // ────────────────────────────────────────────────────────
    
    [Header("CORREDORES")]
    [SerializeField] private float corridorWidth = 18f;      // Ancho X
    [SerializeField] private float corridorLength = 42f;     // Largo Z
    [SerializeField] private float corridorHeight = 2.5f;
    [SerializeField] private float corridorY = 0f;
    [SerializeField] private float barridoSpeed = 15f;
    [SerializeField] private Material laserMaterial;
    
    // ────────────────────────────────────────────────────────
    // CONFIGURACIÓN - SALA GRANDE
    // ────────────────────────────────────────────────────────
    
    [Header("SALA GRANDE (PRISMAS)")]
    [SerializeField] private float mainRoomSize = 36f;       // 36×36
    [SerializeField] private float mainRoomHeight = 3f;
    [SerializeField] private float mainRoomY = 0f;
    
    [SerializeField] private int prismCount = 4;
    [SerializeField] private float prismRadius = 0.5f;
    [SerializeField] private float prismHeight = 3f;
    
    [SerializeField] private int rayEmitterCount = 4;
    [SerializeField] private float raycastLength = 60f;
    
    // ────────────────────────────────────────────────────────
    // REFERENCIAS
    // ────────────────────────────────────────────────────────
    
    private Transform corridor1Container;
    private Transform mainRoomContainer;
    private Transform corridor2Container;
    
    private List<LaserBarrido> barridos = new List<LaserBarrido>();
    private List<PrismColumn> prisms = new List<PrismColumn>();
    private List<LaserCubeEmitter> emitters = new List<LaserCubeEmitter>();
    
    // ────────────────────────────────────────────────────────
    // INICIO
    // ────────────────────────────────────────────────────────
    
    private void Start()
    {
        Debug.Log("[LASER MANAGER] Iniciando generación procedural...");
        
        CreateContainers();
        GenerateCorridor1();
        GenerateMainRoom();
        GenerateCorridor2();
        
        Debug.Log("[LASER MANAGER] ✓ Generación completada");
        Debug.Log($"  Barridos: {barridos.Count}");
        Debug.Log($"  Prismas: {prisms.Count}");
        Debug.Log($"  Emisores: {emitters.Count}");
    }
    
    // ────────────────────────────────────────────────────────
    // CONTENEDORES
    // ────────────────────────────────────────────────────────
    
    private void CreateContainers()
    {
        // Corredor 1 en el SUR: Z: -60 a -18 (centrado en Z: -39)
        corridor1Container = new GameObject("[CORREDOR 1]").transform;
        corridor1Container.parent = transform;
        corridor1Container.localPosition = new Vector3(0, 0, -39f);
        
        // Sala Grande en CENTRO: Z: -18 a +18 (centrado en Z: 0)
        mainRoomContainer = new GameObject("[SALA GRANDE]").transform;
        mainRoomContainer.parent = transform;
        mainRoomContainer.localPosition = new Vector3(0, 0, 0);
        
        // Corredor 2 en el NORTE: Z: +18 a +60 (centrado en Z: +39)
        corridor2Container = new GameObject("[CORREDOR 2]").transform;
        corridor2Container.parent = transform;
        corridor2Container.localPosition = new Vector3(0, 0, 39f);
    }
    
    // ────────────────────────────────────────────────────────
    // GENERAR CORREDOR 1
    // ────────────────────────────────────────────────────────
    
    private void GenerateCorridor1()
    {
        Debug.Log("[LASER MANAGER] Generando CORREDOR 1 (18×42)...");
        
        GenerateCorridorArea(corridor1Container, "Corredor_1", 4);
        
        Debug.Log($"[LASER MANAGER] ✓ Corredor 1 generado con 4 barridos");
    }
    
    // ────────────────────────────────────────────────────────
    // GENERAR CORREDOR 2
    // ────────────────────────────────────────────────────────
    
    private void GenerateCorridor2()
    {
        Debug.Log("[LASER MANAGER] Generando CORREDOR 2 (18×42)...");
        
        GenerateCorridorArea(corridor2Container, "Corredor_2", 4);
        
        Debug.Log($"[LASER MANAGER] ✓ Corredor 2 generado con 4 barridos");
    }
    
    /// <summary>
    /// Genera un área de corredor con N laseres barriendo
    /// </summary>
    private void GenerateCorridorArea(Transform container, string name, int laserCount)
    {
        Vector3 corridorCenter = container.position + new Vector3(corridorWidth * 0.5f, corridorY, corridorLength * 0.5f);
        
        // Piso
        CreateFloor($"{name}_Piso", corridorCenter, corridorWidth, corridorLength, container);
        
        // Distribuir laseres evenly en el eje Z (uno detrás del otro)
        for (int i = 0; i < laserCount; i++)
        {
            float t = (i + 1f) / (laserCount + 1f);
            float laserZ = corridorCenter.z - corridorLength * 0.5f + t * corridorLength;
            float laserX = corridorCenter.x;  // Centro del corredor en X
            
            GameObject barrido = new GameObject($"Barrido_{name}_{i}");
            barrido.transform.parent = container;
            barrido.transform.position = new Vector3(laserX, corridorHeight + 0.5f, laserZ);
            
            LaserBarrido lb = barrido.AddComponent<LaserBarrido>();
            lb.SetBasePosition(laserX, laserZ);  // Configura posición base
            lb.SetSpeed(barridoSpeed);
            
            barridos.Add(lb);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // GENERAR SALA GRANDE
    // ────────────────────────────────────────────────────────
    
    private void GenerateMainRoom()
    {
        Debug.Log("[LASER MANAGER] Generando SALA GRANDE (36×36)...");
        
        Vector3 roomCenter = mainRoomContainer.position + new Vector3(mainRoomSize * 0.5f, mainRoomY, mainRoomSize * 0.5f);
        
        // Piso
        CreateFloor("MainRoom_Piso", roomCenter, mainRoomSize, mainRoomSize, mainRoomContainer);
        
        // Generar prismas aleatorios
        GeneratePrisms(roomCenter);
        
        // Generar emisores de rayos en los 4 bordes
        GenerateRayEmitters(roomCenter);
        
        // Ordenador central
        GameObject computer = new GameObject("ZoneComputer_MainRoom");
        computer.transform.parent = mainRoomContainer;
        computer.transform.position = roomCenter;
        
        ZoneComputer zc = computer.AddComponent<ZoneComputer>();
        
        Debug.Log($"[LASER MANAGER] ✓ Sala grande generada con {prisms.Count} prismas y {emitters.Count} emisores");
    }
    
    // ────────────────────────────────────────────────────────
    // GENERAR PRISMAS
    // ────────────────────────────────────────────────────────
    
    private void GeneratePrisms(Vector3 roomCenter)
    {
        float roomRadius = mainRoomSize * 0.3f;  // Espacio donde colocar prismas (30% del radio)
        
        for (int i = 0; i < prismCount; i++)
        {
            // Posición aleatoria dentro del área (evitando el centro)
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(roomRadius * 0.4f, roomRadius);
            
            Vector3 prismPos = roomCenter + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance
            );
            
            GameObject prismGO = new GameObject($"Prisma_{i}");
            prismGO.transform.parent = mainRoomContainer;
            prismGO.transform.position = prismPos;
            
            PrismColumn prism = prismGO.AddComponent<PrismColumn>();
            prisms.Add(prism);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // GENERAR EMISORES DE RAYOS
    // ────────────────────────────────────────────────────────
    
    private void GenerateRayEmitters(Vector3 roomCenter)
    {
        Vector3[] emitterPositions = new Vector3[4]
        {
            roomCenter + Vector3.forward * (mainRoomSize * 0.45f),   // Norte
            roomCenter + Vector3.back * (mainRoomSize * 0.45f),     // Sur
            roomCenter + Vector3.right * (mainRoomSize * 0.45f),    // Este
            roomCenter + Vector3.left * (mainRoomSize * 0.45f)      // Oeste
        };
        
        Vector3[] emitterDirections = new Vector3[4]
        {
            Vector3.back,   // Emite hacia sur
            Vector3.forward, // Emite hacia norte
            Vector3.left,   // Emite hacia oeste
            Vector3.right   // Emite hacia este
        };
        
        for (int i = 0; i < rayEmitterCount && i < emitterPositions.Length; i++)
        {
            GameObject emitterGO = new GameObject($"RayEmitter_{i}");
            emitterGO.transform.parent = mainRoomContainer;
            emitterGO.transform.position = emitterPositions[i];
            
            LaserCubeEmitter emitter = emitterGO.AddComponent<LaserCubeEmitter>();
            emitter.SetEmitDirection(emitterDirections[i]);
            
            emitters.Add(emitter);
        }
    }
    
    // ────────────────────────────────────────────────────────
    // UTILIDADES
    // ────────────────────────────────────────────────────────
    
    private void CreateFloor(string name, Vector3 center, float width, float depth, Transform parent)
    {
        GameObject floor = new GameObject(name);
        floor.transform.parent = parent;
        floor.transform.position = center;
        
        MeshFilter mf = floor.AddComponent<MeshFilter>();
        MeshRenderer mr = floor.AddComponent<MeshRenderer>();
        floor.AddComponent<BoxCollider>();
        
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        
        Material floorMat = new Material(Shader.Find("Standard"));
        floorMat.color = new Color(0.3f, 0.3f, 0.3f);
        mr.material = floorMat;
        
        floor.transform.localScale = new Vector3(width, 0.2f, depth);
    }
    
    // ────────────────────────────────────────────────────────
    // GETTERS
    // ────────────────────────────────────────────────────────
    
    public List<PrismColumn> GetPrisms() => prisms;
    public List<LaserCubeEmitter> GetEmitters() => emitters;
    public List<LaserBarrido> GetBarridos() => barridos;
}
