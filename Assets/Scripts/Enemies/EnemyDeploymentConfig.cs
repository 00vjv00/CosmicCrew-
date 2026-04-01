// ============================================================================
// ENEMY DEPLOYMENT CONFIG - Mapeo tipos de enemigos → retaguardia
// Archivo: Assets/Scripts/Enemies/EnemyDeploymentConfig.cs
// Descripción: Configuración de qué tipo de robot va a qué retaguardia
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// Define el mapeo entre tipos de enemigos y sus zonas de retaguardia
    /// </summary>
    [System.Serializable]
    public class EnemyDeploymentMap
    {
        public string EnemyType;
        public EnemyRetaguardiaType RetaguardiaZone;
    }

    /// <summary>
    /// Zonas de retaguardia disponibles
    /// </summary>
    public enum EnemyRetaguardiaType
    {
        TROPAS,      // Para robots militares (ALTA_SEGURIDAD)
        INGENIERIA   // Para robots de mantenimiento/almacén (INGENIERIA)
    }

    /// <summary>
    /// ScriptableObject que mapea enemigos a retaguardias
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyDeploymentConfig", menuName = "CosmicCrew/Enemy/Deployment Config")]
    public class EnemyDeploymentConfig : ScriptableObject
    {
        [SerializeField] private EnemyDeploymentMap[] deploymentMap;

        public EnemyDeploymentMap[] DeploymentMap => deploymentMap;

        /// <summary>
        /// Obtiene la zona de retaguardia para un tipo de enemigo
        /// </summary>
        public EnemyRetaguardiaType GetRetaguardiaZone(string enemyType)
        {
            foreach (var map in deploymentMap)
            {
                if (map.EnemyType == enemyType)
                    return map.RetaguardiaZone;
            }

            Debug.LogWarning($"[EnemyDeploymentConfig] No se encontró retaguardia para tipo: {enemyType}");
            return EnemyRetaguardiaType.INGENIERIA; // Default
        }
    }
}
