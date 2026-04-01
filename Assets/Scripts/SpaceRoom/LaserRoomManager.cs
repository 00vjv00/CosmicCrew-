using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Genera la sala de lasers:
///   - Grid de lasers dividida en 4 zonas (NW, NE, SW, SE)
///   - Instancia el prefab IslandPrefab tal cual, sin modificar su escala ni sus hijos
///   - Los ZoneComputer deben estar ya colocados dentro del prefab isla
///   - Obstaculos aleatorios en la sala
///
/// SETUP en Unity:
///   1. Construye el prefab "IslandPrefab" con plataforma, escaleras y los 4
///      ordenadores como hijos, cada uno con el componente ZoneComputer.
///   2. Asigna los prefabs en el Inspector de este Manager.
///   3. Ajusta islandBlockWidth/Depth para que coincidan con el tamano real
///      del prefab — solo se usan para dejar espacio libre de lasers.
/// </summary>
public class LaserRoomManager : MonoBehaviour
{
    // -- Prefabs -----------------------------------------------------------
    [Header("Prefabs")]
    [SerializeField] private GameObject laserPointPrefab;
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject obstaclePrefab;

    // -- Sala --------------------------------------------------------------
    [Header("Room")]
    [SerializeField] private float roomWidth   = 30f;
    [SerializeField] private float roomDepth   = 24f;
    [SerializeField] private float gridSpacing =  2f;
    [SerializeField] private float laserHeight =  1f;

    // -- Isla --------------------------------------------------------------
    [Header("Central Island")]
    [Tooltip("Ancho del prefab isla (eje X) — solo para calcular zona libre de lasers")]
    [SerializeField] private float islandBlockWidth = 16f;
    [Tooltip("Profundidad del prefab isla (eje Z) — solo para calcular zona libre de lasers")]
    [SerializeField] private float islandBlockDepth = 16f;

    // -- Lasers ------------------------------------------------------------
    [Header("Laser Config")]
    [SerializeField] private float cycleMin        = 0.8f;
    [SerializeField] private float cycleMax        = 2.0f;
    [SerializeField] private float damagePerSecond = 20f;
    [SerializeField] private float slowFactor      = 0.4f;
    [Tooltip("Segundos que el laser permanece en escala 0 antes de volver a crecer")]
    [SerializeField] private float restDuration    = 1.5f;

    // -- Obstaculos --------------------------------------------------------
    [Header("Obstacles")]
    [SerializeField] private bool  spawnObstacles = true;
    [SerializeField] private int   obstacleCount  = 5;
    [SerializeField] private float obstacleRadius = 1.3f;

    // -- Runtime -----------------------------------------------------------
    private readonly Dictionary<LaserZoneID, List<LaserPoint>> _zoneMap
        = new Dictionary<LaserZoneID, List<LaserPoint>>
        {
            { LaserZoneID.ZoneA, new List<LaserPoint>() },
            { LaserZoneID.ZoneB, new List<LaserPoint>() },
            { LaserZoneID.ZoneC, new List<LaserPoint>() },
            { LaserZoneID.ZoneD, new List<LaserPoint>() },
        };

    private readonly List<Vector3> _blocked = new List<Vector3>();

    private Vector3 RoomCenter => new Vector3(roomWidth * 0.5f, 0f, roomDepth * 0.5f);

    // -- Unity -------------------------------------------------------------
    private void Start()
    {
        if (laserPointPrefab == null) { Debug.LogError("[LaserRoomManager] Falta laserPointPrefab"); return; }

        BlockIslandArea();
        if (spawnObstacles && obstaclePrefab != null) SpawnObstacles();
        SpawnLaserGrid();
        SpawnIsland();
    }

    // -- Bloquear zona de la isla ------------------------------------------
    private void BlockIslandArea()
    {
        float halfW = islandBlockWidth  * 0.5f + gridSpacing;
        float halfD = islandBlockDepth  * 0.5f + gridSpacing;
        Vector3 c   = RoomCenter;

        for (float x = c.x - halfW; x <= c.x + halfW; x += gridSpacing * 0.5f)
        for (float z = c.z - halfD; z <= c.z + halfD; z += gridSpacing * 0.5f)
            _blocked.Add(new Vector3(x, 0f, z));
    }

    // -- Grid de lasers ----------------------------------------------------
    private void SpawnLaserGrid()
    {
        int cols = Mathf.FloorToInt(roomWidth  / gridSpacing);
        int rows = Mathf.FloorToInt(roomDepth  / gridSpacing);

        float offX = (roomWidth  - (cols - 1) * gridSpacing) * 0.5f;
        float offZ = (roomDepth  - (rows - 1) * gridSpacing) * 0.5f;

        Vector3 center = RoomCenter;

        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
        {
            Vector3 local = new Vector3(
                offX + col * gridSpacing,
                laserHeight,
                offZ + row * gridSpacing
            );

            if (IsBlocked(local)) continue;

            GameObject go = Instantiate(
                laserPointPrefab,
                transform.position + local,
                Quaternion.identity,
                transform
            );
            go.name = $"LP_{col}_{row}";

            LaserPoint lp = go.GetComponent<LaserPoint>() ?? go.AddComponent<LaserPoint>();

            bool isWest  = local.x < center.x;
            bool isSouth = local.z < center.z;
            LaserZoneID zone = isWest  && !isSouth ? LaserZoneID.ZoneA   // NW
                             : !isWest && !isSouth ? LaserZoneID.ZoneB   // NE
                             : isWest  &&  isSouth ? LaserZoneID.ZoneC   // SW
                             :                       LaserZoneID.ZoneD;  // SE

            lp.SetConfig(cycleMin, cycleMax, damagePerSecond, slowFactor, zone, restDuration);
            _zoneMap[zone].Add(lp);
        }

        foreach (var kv in _zoneMap)
            Debug.Log($"[LaserRoomManager] Zona {kv.Key}: {kv.Value.Count} puntos");
    }

    // -- Isla (prefab completo, sin tocar escala ni hijos) -----------------
    private void SpawnIsland()
    {
        if (islandPrefab == null)
        {
            Debug.LogWarning("[LaserRoomManager] islandPrefab no asignado — isla omitida.");
            return;
        }

        // Instanciar en el centro de la sala, Y=0
        // El prefab ya tiene su propia escala, posicion interna y ordenadores
        Vector3 spawnPos = transform.position + RoomCenter;
        spawnPos.y = 0f;

        GameObject island = Instantiate(islandPrefab, spawnPos, Quaternion.identity, transform);
        island.name = "CentralIsland";

        // Buscar ZoneComputers hijos y asignar zonas aleatoriamente
        ZoneComputer[] computers = island.GetComponentsInChildren<ZoneComputer>();

        if (computers.Length == 0)
        {
            Debug.LogWarning("[LaserRoomManager] El prefab isla no tiene hijos con ZoneComputer.");
            return;
        }

        if (computers.Length != 4)
            Debug.LogWarning($"[LaserRoomManager] Se esperaban 4 ZoneComputers, hay {computers.Length}.");

        LaserZoneID[] zones = { LaserZoneID.ZoneA, LaserZoneID.ZoneB,
                                LaserZoneID.ZoneC, LaserZoneID.ZoneD };
        ShuffleArray(zones);

        for (int i = 0; i < computers.Length && i < zones.Length; i++)
        {
            computers[i].Init(zones[i], _zoneMap[zones[i]]);
            computers[i].name = $"Computer_{zones[i]}";
            Debug.Log($"[LaserRoomManager] {computers[i].name} controla {zones[i]}");
        }
    }

    // -- Obstaculos --------------------------------------------------------
    private void SpawnObstacles()
    {
        float safeZ = gridSpacing * 2f;

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 pos   = Vector3.zero;
            bool    ok    = false;
            int     tries = 0;

            do {
                pos = new Vector3(
                    Random.Range(obstacleRadius, roomWidth  - obstacleRadius),
                    0f,
                    Random.Range(safeZ,          roomDepth  - safeZ)
                );
                ok = !IsBlocked(pos);
                tries++;
            } while (!ok && tries < 60);

            if (!ok) continue;

            Instantiate(obstaclePrefab, transform.position + pos, Quaternion.identity, transform);
            _blocked.Add(pos);
        }
    }

    // -- Helpers -----------------------------------------------------------
    private bool IsBlocked(Vector3 pos)
    {
        foreach (Vector3 b in _blocked)
            if (Vector2.Distance(new Vector2(pos.x, pos.z),
                                 new Vector2(b.x,   b.z)) < obstacleRadius)
                return true;
        return false;
    }

    private static void ShuffleArray<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    // -- Gizmos ------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        // Sala
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.DrawWireCube(
            transform.position + RoomCenter + Vector3.up,
            new Vector3(roomWidth, 2f, roomDepth)
        );
        // Zona libre de lasers (cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            transform.position + RoomCenter,
            new Vector3(islandBlockWidth + gridSpacing * 2f, 0.1f,
                        islandBlockDepth + gridSpacing * 2f)
        );
    }
}
