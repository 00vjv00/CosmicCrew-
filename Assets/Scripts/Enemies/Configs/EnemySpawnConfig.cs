// ============================================================================
// ENEMY SPAWN CONFIG - Configuración de tipos de enemigos
// Archivo: Assets/Scripts/Enemies/Configs/EnemySpawnConfig.cs
// Descripción: ScriptableObject con datos de cada tipo de enemigo
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.Configs
{
    /// <summary>
    /// Datos de configuración para un tipo de enemigo
    /// Define stats base, modelos, etc.
    /// </summary>
    [System.Serializable]
    public class EnemyTypeConfig
    {
        public string TypeName;
        public int TotalCount;
        public int InitialDeployed;
        public int InitialInStore;
        public GameObject PrefabInStore;
        public GameObject PrefabDeployed;
        public float MovementSpeed = 5f;
        public float DetectionRange = 35f;
        public float AttackRange = 5f;
        public int Health = 50;
    }

    /// <summary>
    /// ScriptableObject que contiene toda la configuración de enemigos
    /// </summary>
    [CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "CosmicCrew/Enemy/Spawn Config")]
    public class EnemySpawnConfig : ScriptableObject
    {
        [SerializeField] private EnemyTypeConfig[] enemyTypes;

        public EnemyTypeConfig[] EnemyTypes => enemyTypes;

        /// <summary>
        /// Busca la configuración de un tipo de enemigo
        /// </summary>
        public EnemyTypeConfig GetConfigByType(string enemyType)
        {
            foreach (var config in enemyTypes)
            {
                if (config.TypeName == enemyType)
                    return config;
            }
            
            Debug.LogWarning($"[EnemySpawnConfig] No se encontró configuración para tipo: {enemyType}");
            return null;
        }

        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var config in enemyTypes)
            {
                total += config.TotalCount;
            }
            return total;
        }
    }
}
