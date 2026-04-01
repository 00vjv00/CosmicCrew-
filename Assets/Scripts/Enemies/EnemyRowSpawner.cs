// ============================================================================
// ENEMY ROW SPAWNER - Gestiona las filas visuales de robots
// Archivo: Assets/Scripts/Enemies/EnemyRowSpawner.cs
// Descripción: Controla las 2 filas por tipo de robot en retaguardia
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies
{
    /// <summary>
    /// Gestiona las filas visuales de robots emergentes en Tropas/Ingeniería
    /// Cada tipo de enemigo tiene 2 filas que emergen del suelo
    /// </summary>
    public class EnemyRowSpawner : MonoBehaviour
    {
        /// <summary>
        /// Representa una fila de robots
        /// </summary>
        [System.Serializable]
        public class RobotRow
        {
            public string EnemyType;
            public GameObject RowVisualPrefab;
            public Transform RowSpawnPoint;
            public float EmergenceAnimationTime = 1f;
        }

        [SerializeField] private RobotRow[] rows;
        private float[] emergenceTimers;

        private void Start()
        {
            emergenceTimers = new float[rows.Length];
        }

        private void Update()
        {
            // Actualizar animaciones de emergencia
            for (int i = 0; i < rows.Length; i++)
            {
                if (emergenceTimers[i] > 0)
                {
                    emergenceTimers[i] -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Inicia la animación de emergencia para un tipo de robot
        /// </summary>
        public void TriggerRowEmergence(string enemyType)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].EnemyType == enemyType)
                {
                    emergenceTimers[i] = rows[i].EmergenceAnimationTime;
                    
                    // Aquí iría la lógica de animación
                    // Por ahora es un placeholder
                    Debug.Log($"[EnemyRowSpawner] Activar fila: {enemyType}");
                    
                    return;
                }
            }

            Debug.LogWarning($"[EnemyRowSpawner] No se encontró fila para tipo: {enemyType}");
        }

        /// <summary>
        /// Obtiene el punto de desove de una fila
        /// </summary>
        public Transform GetRowSpawnPoint(string enemyType)
        {
            foreach (var row in rows)
            {
                if (row.EnemyType == enemyType)
                    return row.RowSpawnPoint;
            }

            return null;
        }
    }
}
