// ============================================================================
// REPLACING STATE - Viajando hacia su zona asignada
// Archivo: Assets/Scripts/Enemies/States/ReplacingState.cs
// Descripción: Estado de viaje - invisible hasta cerca del player
// ============================================================================

using UnityEngine;

namespace CosmicCrew.Enemies.States
{
    /// <summary>
    /// Estado donde el robot viaja desde retaguardia hasta su zona asignada
    /// Sigue waypoints y es invisible hasta que está cerca del player
    /// </summary>
    public class ReplacingState : IEnemyState
    {
        private float visibilityCheckTimer = 0f;
        private const float VISIBILITY_CHECK_INTERVAL = 0.5f; // Revisar cada 0.5 segundos

        public void OnEnter(Enemy enemy)
        {
            // Inicialmente invisible
            enemy.SetVisibility(false);
            
            // Inicializar navegación
            if (enemy.NavigationSystem != null)
            {
                enemy.NavigationSystem.InitializeRoute(enemy);
            }

            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} en ReplacingState (viajando a zona asignada)");
        }

        public void OnUpdate(Enemy enemy)
        {
            // Actualizar navegación
            if (enemy.NavigationSystem != null)
            {
                enemy.NavigationSystem.UpdateMovement(enemy);
            }

            // Revisar periódicamente visibilidad
            visibilityCheckTimer += Time.deltaTime;
            if (visibilityCheckTimer >= VISIBILITY_CHECK_INTERVAL)
            {
                visibilityCheckTimer = 0f;
                CheckVisibility(enemy);
            }

            // Si llegó a destino, cambiar a IdleState
            if (enemy.NavigationSystem != null && enemy.NavigationSystem.HasReachedDestination(enemy))
            {
                enemy.ChangeState(new IdleState());
            }
        }

        public void OnExit(Enemy enemy)
        {
            Debug.Log($"[Enemy] {enemy.EnemyType} {enemy.gameObject.name} sale de ReplacingState (llegó a zona asignada)");
        }

        /// <summary>
        /// Revisa si el robot debe hacerse visible basado en proximidad al player
        /// </summary>
        private void CheckVisibility(Enemy enemy)
        {
            if (enemy.PlayerTransform == null)
                return;

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position);
            
            // Umbral de visibilidad: radio de descubrimiento del player
            const float VISIBILITY_DISTANCE = 40f; // Ajustar según necesidad
            
            if (distanceToPlayer <= VISIBILITY_DISTANCE)
            {
                enemy.SetVisibility(true);
            }
            else if (distanceToPlayer > VISIBILITY_DISTANCE + 10f) // Hysteresis
            {
                enemy.SetVisibility(false);
            }
        }
    }
}
