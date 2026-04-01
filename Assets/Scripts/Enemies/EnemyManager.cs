// ============================================================================
// ENEMY MANAGER - GESTOR CENTRAL DE ENEMIGOS
// Archivo: Assets/Scripts/Enemies/EnemyManager.cs
// Descripción: Singleton que coordina todo el sistema de enemigos
// ============================================================================

using UnityEngine;
using System.Collections.Generic;
using CosmicCrew.Enemies.Configs;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// EnemyManager es el Singleton central que coordina:
    /// - Pool total de enemigos (248 robots)
    /// - Distribución en zonas (desplegados vs store)
    /// - Sistema de reemplazos cuando un robot muere
    /// - Integración con retaguardia (Tropas/Ingeniería)
    /// - Navegación invisible desde retaguardia → zona asignada
    /// 
    /// FLUJO PRINCIPAL:
    /// 1. Inicializar pool de 248 robots (166 desplegados, 82 en store)
    /// 2. Robots en zonas patrullan en IdleState
    /// 3. Si mueren → DeadState → EnemyManager detecta destrucción
    /// 4. EnemyManager busca reemplazo en retaguardia
    /// 5. Fila visual emerge, robot real se despliega en ReplacingState
    /// 6. Viaja invisible hasta zona (si player no está cerca)
    /// 7. Aparece cuando es cercano al player
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        // ====================================================================
        // SINGLETON
        // ====================================================================

        public static EnemyManager Instance { get; private set; }

        // ====================================================================
        // INSPECTABLES
        // ====================================================================

        [SerializeField] private EnemySpawnConfig spawnConfig;
        [SerializeField] private EnemyDeploymentConfig deploymentConfig;
        [SerializeField] private EnemyRetaguardiaSystem retaguardiaSystem;
        
        [Header("Pool Prefabs")]
        [SerializeField] private GameObject[] enemyPrefabs; // Prefabs de cada tipo

        // ====================================================================
        // POOLS DE ENEMIGOS
        // ====================================================================

        private Dictionary<string, List<Enemy>> enemyPool;        // Pool por tipo
        private Dictionary<string, int> deployedCount;            // Tracking de desplegados
        private List<Enemy> allEnemies;                           // Todos los enemigos

        // ====================================================================
        // EVENTOS DEL MANAGER
        // ====================================================================

        public delegate void EnemyManagerEventHandler(Enemy enemy, string enemyType);
        public event EnemyManagerEventHandler OnEnemyDeployed;
        public event EnemyManagerEventHandler OnEnemyDestroyed;
        public event EnemyManagerEventHandler OnReplacementRequested;

        // ====================================================================
        // SINGLETON SETUP
        // ====================================================================

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeManager();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        // ====================================================================
        // INICIALIZACIÓN
        // ====================================================================

        /// <summary>
        /// Inicializa el pool de enemigos según la configuración
        /// </summary>
        private void InitializeManager()
        {
            if (spawnConfig == null)
            {
                Debug.LogError("[EnemyManager] spawnConfig no está asignado");
                return;
            }

            enemyPool = new Dictionary<string, List<Enemy>>();
            deployedCount = new Dictionary<string, int>();
            allEnemies = new List<Enemy>();

            Debug.Log("[EnemyManager] Inicializando pool de enemigos...");

            // Crear pool para cada tipo
            foreach (var config in spawnConfig.EnemyTypes)
            {
                enemyPool[config.TypeName] = new List<Enemy>();
                deployedCount[config.TypeName] = 0;

                // Instanciar la cantidad total de robots
                for (int i = 0; i < config.TotalCount; i++)
                {
                    Enemy enemy = CreateEnemyInstance(config.TypeName, i);
                    enemyPool[config.TypeName].Add(enemy);
                    allEnemies.Add(enemy);

                    // Suscribirse a eventos del enemigo
                    enemy.OnEnemyDestroyed += OnEnemyDestroyedHandler;
                }

                Debug.Log($"[EnemyManager] Pool creado: {config.TypeName} x{config.TotalCount}");
            }

            // Iniciar deployment según configuración
            DeployInitialEnemies();

            Debug.Log($"[EnemyManager] Total enemigos en juego: {allEnemies.Count}");
        }

        /// <summary>
        /// Crea una instancia de robot
        /// </summary>
        private Enemy CreateEnemyInstance(string enemyType, int index)
        {
            var config = spawnConfig.GetConfigByType(enemyType);
            if (config == null)
            {
                Debug.LogError($"[EnemyManager] Configuración no encontrada para: {enemyType}");
                return null;
            }

            // TODO: Usar pool prefab correcto según tipo
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Enemies/{enemyType}");
            if (prefab == null)
            {
                Debug.LogWarning($"[EnemyManager] Prefab no encontrado: {enemyType}. Usando placeholder.");
                prefab = new GameObject($"Enemy_{enemyType}_{index}");
            }

            GameObject instance = Instantiate(prefab);
            instance.name = $"{enemyType}_{index}";
            
            Enemy enemy = instance.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = instance.AddComponent<Enemy>();
            }

            enemy.SetEnemyType(enemyType);
            instance.SetActive(false); // Inactivo inicialmente

            return enemy;
        }

        // ====================================================================
        // DEPLOYMENT INICIAL
        // ====================================================================

        /// <summary>
        /// Despliega el número inicial de enemigos según configuración
        /// </summary>
        private void DeployInitialEnemies()
        {
            foreach (var config in spawnConfig.EnemyTypes)
            {
                for (int i = 0; i < config.InitialDeployed; i++)
                {
                    Enemy enemy = GetEnemyFromPool(config.TypeName);
                    if (enemy != null)
                    {
                        DeployEnemy(enemy, config.TypeName);
                        // Suscribirse a eventos del enemigo
                        enemy.OnEnemyDestroyed += ProcessEnemyDestruction;
                    }
                }

                // Los restantes van a store automáticamente
                int inStore = config.TotalCount - config.InitialDeployed;
                for (int i = 0; i < inStore; i++)
                {
                    Enemy enemy = GetEnemyFromPool(config.TypeName);
                    if (enemy != null && retaguardiaSystem != null)
                    {
                        retaguardiaSystem.StoreEnemy(enemy);
                    }
                }
            }
        }

        /// <summary>
        /// Despliega un enemigo específico en su zona asignada
        /// </summary>
        private void DeployEnemy(Enemy enemy, string enemyType)
        {
            if (enemy == null)
                return;

            // TODO: Asignar zona según lógica del juego
            // Por ahora es un placeholder
            Transform spawnZone = gameObject.transform; // Placeholder

            enemy.SetAssignedZone(spawnZone);
            enemy.gameObject.SetActive(true);
            enemy.ResetEnemy();

            deployedCount[enemyType]++;

            OnEnemyDeployed?.Invoke(enemy, enemyType);
            Debug.Log($"[EnemyManager] Desplegado: {enemyType} en zona");
        }

        // ====================================================================
        // OBTENCIÓN DE ENEMIGOS DEL POOL
        // ====================================================================

        /// <summary>
        /// Obtiene el próximo enemigo disponible del pool
        /// </summary>
        private Enemy GetEnemyFromPool(string enemyType)
        {
            if (!enemyPool.ContainsKey(enemyType) || enemyPool[enemyType].Count == 0)
            {
                Debug.LogWarning($"[EnemyManager] No hay enemigos disponibles en pool: {enemyType}");
                return null;
            }

            Enemy enemy = enemyPool[enemyType][0];
            enemyPool[enemyType].RemoveAt(0);
            return enemy;
        }

        // ====================================================================
        // MANEJO DE DESTRUCCIÓN & REEMPLAZOS
        // ====================================================================

        /// <summary>
        /// Procesa la destrucción de un robot
        /// Se llama automáticamente desde Enemy.cs
        /// </summary>
        public void ProcessEnemyDestruction(Enemy enemy)
        {
            OnEnemyDestroyedHandler(enemy);
        }

        /// <summary>
        /// Handler interno para destrucción de enemigos
        /// </summary>
        private void OnEnemyDestroyedHandler(Enemy enemy)
        {
            string enemyType = enemy.EnemyType;
            deployedCount[enemyType]--;

            Debug.Log($"[EnemyManager] {enemyType} destruido. Buscando reemplazo...");

            // Solicitar reemplazo desde retaguardia
            if (retaguardiaSystem != null)
            {
                Enemy replacement = retaguardiaSystem.GetReplacement(enemyType);
                
                if (replacement != null)
                {
                    // Cambiar a ReplacingState para navegar invisiblemente
                    DeployReplacement(replacement, enemy.AssignedZone);
                    OnReplacementRequested?.Invoke(replacement, enemyType);
                }
                else
                {
                    Debug.LogWarning($"[EnemyManager] No hay reemplazos disponibles para {enemyType}");
                }
            }

            OnEnemyDestroyed?.Invoke(enemy, enemyType);
        }

        /// <summary>
        /// Despliega un reemplazo desde retaguardia hacia la zona donde murió su predecesor
        /// </summary>
        private void DeployReplacement(Enemy replacement, Transform targetZone)
        {
            replacement.gameObject.SetActive(true);
            replacement.SetAssignedZone(targetZone);
            replacement.ResetEnemy();

            // Cambiar a ReplacingState para navegar
            var replacingState = new CosmicCrew.Enemies.States.ReplacingState();
            replacement.ChangeState(replacingState);

            Debug.Log($"[EnemyManager] Reemplazo {replacement.EnemyType} en camino a {targetZone.name}");
        }

        // ====================================================================
        // QUERIES Y DEBUGGING
        // ====================================================================

        /// <summary>
        /// Obtiene el recuento de enemigos desplegados de un tipo
        /// </summary>
        public int GetDeployedEnemyCount(string enemyType)
        {
            return deployedCount.ContainsKey(enemyType) ? deployedCount[enemyType] : 0;
        }

        /// <summary>
        /// Obtiene el recuento total de enemigos activos
        /// </summary>
        public int GetTotalDeployedEnemyCount()
        {
            int total = 0;
            foreach (var count in deployedCount.Values)
            {
                total += count;
            }
            return total;
        }

        /// <summary>
        /// Imprime estado del manager
        /// </summary>
        public void DebugPrintStatus()
        {
            Debug.Log("[EnemyManager] === ESTADO DEL SISTEMA ===");
            foreach (var kvp in deployedCount)
            {
                int poolCount = enemyPool.ContainsKey(kvp.Key) ? enemyPool[kvp.Key].Count : 0;
                Debug.Log($"  {kvp.Key}: {kvp.Value} desplegados, {poolCount} en pool");
            }

            if (retaguardiaSystem != null)
            {
                retaguardiaSystem.DebugPrintStatus();
            }
        }
    }
}
