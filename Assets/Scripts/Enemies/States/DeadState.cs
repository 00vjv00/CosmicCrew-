// ============================================================================
// DEAD STATE - Robot destruido
// Archivo: Assets/Scripts/Enemies/States/DeadState.cs
// Descripción: Estado final - robot muerto, será reemplazado
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot ha sido destruido
    /// Notifica al EnemyManager para desplegar un reemplazo
    /// Se desactiva pasado un tiempo
    /// </summary>
    public class DeadState : IEnemyState
    {
        private float deactivationTimer = 0f;
        private const float DEACTIVATION_TIME = 2f; // Mantener visible 2 segundos después de morir

        public void OnEnter(Enemy enemy)
        {
            enemy.SetVisibility(true);
            deactivationTimer = 0f;

            // Desactivar colisionadores
            var colliders = enemy.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            // Notificar al EnemyManager que fue destruido
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.ProcessEnemyDestruction(enemy);
            }

            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} DESTRUIDO - Desplegando reemplazo");
        }

        public void OnUpdate(Enemy enemy)
        {
            deactivationTimer += Time.deltaTime;
            
            // Después del tiempo, desactivar el GameObject
            if (deactivationTimer >= DEACTIVATION_TIME)
            {
                // Volver a StoreState para reutilización
                enemy.ChangeState(new StoreState());
                enemy.gameObject.SetActive(false);
            }
        }

        public void OnExit(Enemy enemy)
        {
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} siendo reutilizado");
        }
    }
}
