// ============================================================================
// ENEMY RETAGUARDIA SYSTEM - Sistema de almacenamiento en retaguardia
// Archivo: Assets/Scripts/Enemies/EnemyRetaguardiaSystem.cs
// Descripción: Gestiona Tropas e Ingeniería (zonas de retaguardia)
// ============================================================================

using UnityEngine;
using System.Collections.Generic;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// Gestiona las zonas de retaguardia: Tropas e Ingeniería
    /// Almacena robots en "Store" y los prepara para despliegue
    /// </summary>
    public class EnemyRetaguardiaSystem : MonoBehaviour
    {
        // ====================================================================
        // TIPOS DE RETAGUARDIA
        // ====================================================================

        [System.Serializable]
        public class RetaguardiaZone
        {
            public EnemyRetaguardiaType ZoneType;
            public Transform ZoneTransform;
            public EnemyRowSpawner RowSpawner;
            public List<Enemy> StoredEnemies = new List<Enemy>();
            public List<Enemy> DeployedEnemies = new List<Enemy>();
        }

        // ====================================================================
        // INSPECTABLES
        // ====================================================================

        [SerializeField] private RetaguardiaZone[] retaguardiaZones;
        [SerializeField] private EnemyDeploymentConfig deploymentConfig;

        // ====================================================================
        // PROPIEDADES PÚBLICAS
        // ====================================================================

        public RetaguardiaZone[] RetaguardiaZones => retaguardiaZones;

        // ====================================================================
        // LIFECYCLE
        // ====================================================================

        private void OnEnable()
        {
            InitializeRetaguardia();
        }

        // ====================================================================
        // INICIALIZACIÓN
        // ====================================================================

        /// <summary>
        /// Inicializa las zonas de retaguardia
        /// </summary>
        private void InitializeRetaguardia()
        {
            foreach (var zone in retaguardiaZones)
            {
                Debug.Log($"[RetaguardiaSystem] Inicializando zona: {zone.ZoneType}");
                // Las listas de enemigos se llenarán desde EnemyManager
            }
        }

        // ====================================================================
        // ALMACENAMIENTO & DISTRIBUCIÓN
        // ====================================================================

        /// <summary>
        /// Almacena un robot en su zona de retaguardia correspondiente
        /// </summary>
        public void StoreEnemy(Enemy enemy)
        {
            EnemyRetaguardiaType targetZone = deploymentConfig.GetRetaguardiaZone(enemy.EnemyType);
            
            foreach (var zone in retaguardiaZones)
            {
                if (zone.ZoneType == targetZone)
                {
                    zone.StoredEnemies.Add(enemy);
                    enemy.SetVisibility(false);
                    Debug.Log($"[RetaguardiaSystem] {enemy.EnemyType} almacenado en {targetZone}");
                    return;
                }
            }

            Debug.LogWarning($"[RetaguardiaSystem] No se encontró zona para {enemy.EnemyType}");
        }

        /// <summary>
        /// Obtiene un reemplazo de una zona de retaguardia
        /// </summary>
        public Enemy GetReplacement(string enemyType)
        {
            EnemyRetaguardiaType targetZone = deploymentConfig.GetRetaguardiaZone(enemyType);

            foreach (var zone in retaguardiaZones)
            {
                if (zone.ZoneType == targetZone && zone.StoredEnemies.Count > 0)
                {
                    // Obtener el primer robot disponible
                    Enemy replacement = zone.StoredEnemies[0];
                    zone.StoredEnemies.RemoveAt(0);
                    zone.DeployedEnemies.Add(replacement);

                    // Activar animación de fila
                    if (zone.RowSpawner != null)
                    {
                        zone.RowSpawner.TriggerRowEmergence(enemyType);
                    }

                    Debug.Log($"[RetaguardiaSystem] Reemplazo desplegado: {enemyType} desde {targetZone}");
                    return replacement;
                }
            }

            Debug.LogWarning($"[RetaguardiaSystem] No hay reemplazos disponibles para {enemyType}");
            return null;
        }

        /// <summary>
        /// Obtiene el recuento de robots en una zone de retaguardia
        /// </summary>
        public int GetStoredEnemyCount(EnemyRetaguardiaType zoneType, string enemyType = "")
        {
            foreach (var zone in retaguardiaZones)
            {
                if (zone.ZoneType == zoneType)
                {
                    if (string.IsNullOrEmpty(enemyType))
                        return zone.StoredEnemies.Count;

                    // Contar solo del tipo específico
                    int count = 0;
                    foreach (var enemy in zone.StoredEnemies)
                    {
                        if (enemy.EnemyType == enemyType)
                            count++;
                    }
                    return count;
                }
            }

            return 0;
        }

        /// <summary>
        /// Debug: imprime el estado de todas las retaguardias
        /// </summary>
        public void DebugPrintStatus()
        {
            foreach (var zone in retaguardiaZones)
            {
                Debug.Log($"[RetaguardiaSystem] {zone.ZoneType}: {zone.StoredEnemies.Count} en store, {zone.DeployedEnemies.Count} desplegados");
            }
        }
    }
}
