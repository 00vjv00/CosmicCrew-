using UnityEngine;

public class PrisonStationGenerator : MonoBehaviour
{
    [Header("Cámara Isométrica")]
/*     public IsometricCamera isometricCamera;
 */
    [Header("Player Manager (asignar en inspector)")]
/*     public PlayerManager playerManager;
 */
    [Header("Sectores")]
    public System.Collections.Generic.List<System.Collections.Generic.List<Transform>> sectores = new System.Collections.Generic.List<System.Collections.Generic.List<Transform>>();

    public SectorManager.TipoSector[] tiposSectores = {
        SectorManager.TipoSector.AltaSeguridad,
        SectorManager.TipoSector.Tripulacion,
        SectorManager.TipoSector.Navegación,
        SectorManager.TipoSector.HangarOrbital,
        SectorManager.TipoSector.Almacen,
        SectorManager.TipoSector.Ingenieria,
    };

    [Header("Centro del círculo")]
    [SerializeField]
    public Vector3 centro = Vector3.zero;

    public System.Collections.Generic.List<Transform> copiasTransforms = new System.Collections.Generic.List<Transform>();

    [Header("Prefab")]
    public GameObject planoPrefab;


    [Header("Configuración")]
    public int numCopias = 120;
    public float espaciamentoZ = 0f;
    public float rotacionXPorCopia = -3f;
    public float alturaPlano = 4f;

    [Header("Contenedor padre (Station)")]
    public Transform contenedor;


    [SerializeField]
    // VAriable numero de plano en cuyo centro se espanea el player
    public int planoInicial = 1;
    public void GenerarEstacion()
    {
        Debug.Log("[PrisonStationGenerator] Iniciando generación de estación...");
        if (contenedor == null)
        {
            Debug.LogError("Asigna el contenedor padre (Station) en el inspector");
            return;
        }
        if (contenedor.GetComponent<StationRotator>() == null)
        {
            Debug.Log("[PrisonStationGenerator] Añadiendo StationRotator al contenedor.");
            contenedor.gameObject.AddComponent<StationRotator>();
        }

        GenerarCopias();
    }

    void GenerarCopias()
    {
        Debug.Log("[PrisonStationGenerator] Limpiando sectores y preparando generación de copias...");
        sectores.Clear();
        int numSectores = tiposSectores.Length;
        // Reparto proporcional para evitar sectores desbalanceados
        // y nunca crear sectores extra

        Transform[] padresSectores = new Transform[numSectores];
        SectorManager[] sectorManagers = new SectorManager[numSectores];

        for (int s = 0; s < numSectores; s++)
        {
            sectores.Add(new System.Collections.Generic.List<Transform>());
            string nombreSector = tiposSectores[s].ToString();
            GameObject sectorGO = new GameObject(nombreSector);
            sectorGO.transform.parent = contenedor;
            padresSectores[s] = sectorGO.transform;
            var sm = sectorGO.AddComponent<SectorManager>();
            sm.tipoSector = tiposSectores[s];
            sectorManagers[s] = sm;
            Debug.Log($"[PrisonStationGenerator] Sector creado: {nombreSector}");
        }

        if (planoPrefab == null)
        {
            Debug.LogError("Asigna un prefab de plano en el inspector");
            return;
        }

        Debug.Log("[PrisonStationGenerator] Calculando posiciones de planos...");
        Vector3 sumaPos = Vector3.zero;
        Vector3[] posicionesPlanos = new Vector3[numCopias];

        float tamPlanoZ = 36f;  // Coincide con la profundidad real del prefab
        float anguloRad = Mathf.Abs(rotacionXPorCopia) * Mathf.Deg2Rad;
        float radio = (tamPlanoZ / 2f) / Mathf.Sin(anguloRad / 2f);

        for (int i = 0; i < numCopias; i++)
        {
            float angulo = i * rotacionXPorCopia;
            float anguloRadPlano = angulo * Mathf.Deg2Rad;
            float y = radio * Mathf.Sin(anguloRadPlano);
            float z = radio * Mathf.Cos(anguloRadPlano);
            posicionesPlanos[i] = new Vector3(0, y + alturaPlano, z);
            sumaPos += posicionesPlanos[i];
        }

        centro = sumaPos / numCopias;
        contenedor.position = centro;
        Debug.Log($"[PrisonStationGenerator] Centro calculado: {centro}");

        Debug.Log("[PrisonStationGenerator] Instanciando planos...");
        for (int i = 0; i < numCopias; i++)
        {
            GameObject copia = Instantiate(planoPrefab);
            copia.name = $"Plano_{i}";
            copia.transform.position = posicionesPlanos[i];

            float angulo = i * rotacionXPorCopia;
            float anguloRadPlano = angulo * Mathf.Deg2Rad;
            Vector3 radial = new Vector3(0, Mathf.Sin(anguloRadPlano), Mathf.Cos(anguloRadPlano));
            Vector3 tangente = new Vector3(0, -Mathf.Cos(anguloRadPlano), Mathf.Sin(anguloRadPlano));
            copia.transform.rotation = Quaternion.LookRotation(tangente, -radial);

            // Reparto proporcional
            int sectorIdx = Mathf.FloorToInt((float)i * numSectores / numCopias);
            copia.transform.parent = padresSectores[sectorIdx];

            var planoController = copia.GetComponent<PlanoController>();
            if (planoController != null)
            {
                planoController.stationGenerator = this;
                planoController.miIndice = i;
            }

            copiasTransforms.Add(copia.transform);
            sectores[sectorIdx].Add(copia.transform);
            sectorManagers[sectorIdx].planos.Add(copia.transform);
            if (i % 50 == 0) Debug.Log($"[PrisonStationGenerator] Instanciado plano {i}/{numCopias}");
        }

        Debug.Log("[PrisonStationGenerator] Poblando sectores...");
        foreach (var sm in sectorManagers)
            sm.PoblarSector();

        Debug.Log($"[PrisonStationGenerator] Estación generada con {numCopias} planos");
    }

    public void ActualizarPlanosActivos(int indicePlanoJugador)
    {
        int total = copiasTransforms.Count;
        bool[] activos = new bool[total];
        for (int offset = -5; offset <= 5; offset++)
        {
            int idx = (indicePlanoJugador + offset + total) % total;
            activos[idx] = true;
        }
        for (int i = 0; i < total; i++)
        {
            var planoController = copiasTransforms[i].GetComponent<PlanoController>();
            if (planoController != null)
            {
                if (activos[i]) planoController.Activar();
                else planoController.Desactivar();
            }
            else
            {
                copiasTransforms[i].gameObject.SetActive(activos[i]);
            }
        }
    }
}