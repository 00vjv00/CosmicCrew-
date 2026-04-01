using UnityEngine;
using System.Collections.Generic;

public class SectorManager : MonoBehaviour
{
    public enum TipoSector
    {
        AltaSeguridad,
        Tripulacion,
        Navegación,
        HangarOrbital,
        Almacen,
        Ingenieria
    
    }

    [Header("Tipo de sector")]
    public TipoSector tipoSector;

    [Header("Planos generados (asignados por PrisonStationGenerator)")]
    public List<Transform> planos = new List<Transform>();

    [Header("Pool de diseño manual")]
    [Tooltip("Nombre del GameObject raíz del pool en escena")]
    public string nombrePool = "Pool";

    // Llamado por PrisonStationGenerator al terminar la generación
    public void PoblarSector()
    {
        // Buscar Pool en escena
        GameObject pool = GameObject.Find(nombrePool);
        if (pool == null)
        {
            Debug.LogWarning($"[{tipoSector}] No se encontró el GameObject '{nombrePool}' en escena.");
            return;
        }

        // Buscar el hijo con el nombre del sector
        Transform poolSector = pool.transform.Find(tipoSector.ToString());
        if (poolSector == null)
        {
            Debug.LogWarning($"[{tipoSector}] No se encontró '{tipoSector}' como hijo de '{nombrePool}'.");
            return;
        }

        int numPlanos = Mathf.Min(planos.Count, poolSector.childCount);

        for (int i = 0; i < numPlanos; i++)
        {
            Transform fuentePlano = poolSector.GetChild(i);
            Transform destinoPlano = planos[i];

            // Instanciar copia del plano del pool
            GameObject copia = Instantiate(fuentePlano.gameObject);
            copia.name = fuentePlano.name + "_instancia";

            // Colocar como hijo del plano generado y resetear transform local
            copia.transform.SetParent(destinoPlano, worldPositionStays: false);
            copia.transform.localPosition = Vector3.zero;
            copia.transform.localRotation = Quaternion.identity;
            copia.transform.localScale = Vector3.one;
        }

        if (poolSector.childCount < planos.Count)
            Debug.LogWarning($"[{tipoSector}] El pool tiene {poolSector.childCount} planos pero el sector tiene {planos.Count}. Los planos sobrantes quedarán vacíos.");

    }
}